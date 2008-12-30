using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Suteki.Common.Extensions;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Models;
using Suteki.Common.Tests.TestModel;

namespace Suteki.Common.Tests.Models
{
    [TestFixture]
    public class EntityGraphVisitorSpike
    {
        private Customer customer;

        [SetUp]
        public void SetUp()
        {
            customer = new CustomerBuilder().CreateCustomer();
        }

        [Test]
        public void Should_Enumerate_Every_Entity_In_Graph()
        {
            var entityGraphEnumerator = new EntityGraphEnumerator(customer);
            var result = PrintGraph(entityGraphEnumerator);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(expectedCustomerGraphOutput));
        }

        [Test]
        public void Should_Enumerate_From_Any_Root()
        {
            var entityGraphEnumerator = new EntityGraphEnumerator(customer.Orders[0].OrderLines[0]);
            var result = PrintGraph(entityGraphEnumerator);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(expectedOrderLineGraph));
        }

        [Test, Explicit]
        public void Should_Be_Able_To_Build_A_MetaEntity_Tree()
        {
            var metaEntityList = new Dictionary<IEntity, MetaEntity>();
            var entityGraphEnumerator = new EntityGraphEnumerator(customer);
            MetaEntity rootMetaEntity = null;

            foreach (var entityInfo in entityGraphEnumerator.Entities)
            {
                // get parent metaEntity
                MetaEntity parentMetaEntity = null;
                if (entityInfo.Parent != null && metaEntityList.ContainsKey(entityInfo.Parent))
                {
                    parentMetaEntity = metaEntityList[entityInfo.Parent];
                }

                // get the metaEntity
                MetaEntity metaEntity;
                if (metaEntityList.ContainsKey(entityInfo.Entity))
                {
                    metaEntity = metaEntityList[entityInfo.Entity];
                }
                else
                {
                    if (parentMetaEntity == null)
                    {
                        metaEntity = rootMetaEntity = new RootMetaEntity(entityInfo.Entity);
                    }
                    else
                    {
                        metaEntity = new MetaEntity(entityInfo.Entity, parentMetaEntity);
                    }
                    metaEntityList.Add(entityInfo.Entity, metaEntity);
                }

                // add the metaEntity to its parent
                if (parentMetaEntity != null)
                {
                    parentMetaEntity.Children.Add(metaEntity);
                }
            }

            var html = new TreeRenderer<MetaEntity>(new[] { rootMetaEntity }, metaEntity => metaEntity.ToString()).Render();
            Console.WriteLine(html);
        }

        private static string PrintGraph(EntityGraphEnumerator entityGraphVisitor)
        {
            var output = new StringBuilder();

            foreach (var entityInfo in entityGraphVisitor.Entities.Where(i => i.Level < 6))
            {
                var tab = new string('\t', entityInfo.Level);
                if(entityInfo.Entity is INamedEntity)
                    output.AppendFormat(("{0}{1}:{2}\r\n"), tab, entityInfo.Entity.GetType().Name, ((INamedEntity)entityInfo.Entity).Name);
                else
                    output.AppendFormat("{0}{1}\r\n", tab, entityInfo.Entity.GetType().Name);
            }
            return output.ToString();
        }

        private const string expectedCustomerGraphOutput =
@"Customer:Mike
	Order
		OrderLine
			Product:Relayer
				Supplier:Disco Hits
		OrderLine
			Product:Fragile
				Supplier:Old Vinyl
	Order
		OrderLine
			Product:Relayer
				Supplier:Disco Hits
		OrderLine
			Product:Close to the Edge
				Supplier:Old Vinyl
		OrderLine
";

        private const string expectedOrderLineGraph =
@"OrderLine
	Product:Relayer
		Supplier:Disco Hits
	Order
		Customer:Mike
";

    }

    public class EntityGraphEnumerator
    {
        private readonly IEntity root;
        private readonly ICircularReferenceDetector circularReferenceDetector;
        private readonly INestingLevel nestingLevel;
        private readonly IPropertyVisitor[] propertyVisitors;

        public EntityGraphEnumerator(IEntity root) 
            : this(root, new CircularReferenceDetector(), new NestingLevel(), BuildPropertyVisitors())
        {
        }

        private static IPropertyVisitor[] BuildPropertyVisitors()
        {
            return new IPropertyVisitor[]
            {
                new EntityPropertyVisitor(),
                new CollectionPropertyVisitor()
            };
        }

        public EntityGraphEnumerator(
            IEntity root, 
            ICircularReferenceDetector circularReferenceDetector, 
            INestingLevel nestingLevel,
            IPropertyVisitor[] propertyVisitors)
        {
            this.root = root;
            this.circularReferenceDetector = circularReferenceDetector;
            this.nestingLevel = nestingLevel;
            this.propertyVisitors = propertyVisitors;
        }

        public IEnumerable<IEntityInfo> Entities
        {
            get
            {
                circularReferenceDetector.Reset();
                nestingLevel.Reset();
                return RecurseEntityGraph(null, root, null);
            }
        }

        private IEnumerable<IEntityInfo> RecurseEntityGraph(IEntity parent, IEntity entity, PropertyInfo propertyInfo)
        {
            nestingLevel.Push();

            yield return new EntityInfo(entity, nestingLevel.Level, parent, propertyInfo, false);

            foreach (var property in entity.GetType().GetProperties())
            {
                IEnumerable<IEntity> childEntities = new IEntity[] {  };
                foreach (var visitor in propertyVisitors)
                {
                    childEntities = childEntities.Concat(visitor.Visit(property, entity));
                }

                foreach (var childEntity in childEntities)
                {
                    if (circularReferenceDetector.DetectedOn(entity, childEntity)) continue;
                    foreach (var entityInfo in RecurseEntityGraph(entity, childEntity, property))
                    {
                        yield return entityInfo;
                    }
                }
            }

            nestingLevel.Pop();
        }
    }

    public interface IPropertyVisitor
    {
        IEnumerable<IEntity> Visit(PropertyInfo property, IEntity entity);
    }

    public class EntityPropertyVisitor : IPropertyVisitor
    {
        public IEnumerable<IEntity> Visit(PropertyInfo property, IEntity entity)
        {
            if (property.PropertyType.IsEntity())
            {
                var childEntity = property.GetValue(entity, null) as IEntity;
                if (childEntity != null)
                {
                    yield return childEntity;
                }
            }
        }
    }

    public class CollectionPropertyVisitor : IPropertyVisitor
    {
        public IEnumerable<IEntity> Visit(PropertyInfo property, IEntity entity)
        {
            if (property.PropertyType.IsEntityCollection())
            {
                var collection = (IEnumerable)property.GetValue(entity, null);
                foreach (var obj in collection)
                {
                    var childEntity = (IEntity)obj;
                    yield return childEntity;
                }
            }
        }
    }

    public interface ICircularReferenceDetector
    {
        bool DetectedOn(IEntity parent, IEntity child);
        void Reset();
    }

    public class CircularReferenceDetector : ICircularReferenceDetector
    {
        private readonly HashSet<string> relationshipTokens = new HashSet<string>();

        public bool DetectedOn(IEntity parent, IEntity child)
        {
            // make a simple token from the parent and child types
            var relationshipToken = "{0}:{1}".With(parent.GetType().Name, child.GetType().Name);

            if (relationshipTokens.Contains(relationshipToken)) return true;

            // make a reverse token.
            var testToken = "{0}:{1}".With(child.GetType().Name, parent.GetType().Name);

            // add the new reverse token to the list.
            relationshipTokens.Add(testToken);

            return false;
        }

        public void Reset()
        {
            relationshipTokens.Clear();
        }
    }

    public interface INestingLevel
    {
        int Level { get; }
        void Push();
        void Pop();
        void Reset();
    }

    public class NestingLevel : INestingLevel
    {
        public NestingLevel()
        {
            Reset();
        }

        public int Level { get; private set; }

        public void Push()
        {
            Level++;
        }

        public void Pop()
        {
            Level--;
        }

        public void Reset()
        {
            Level = -1;
        }
    }

    public interface IEntityInfo
    {
        IEntity Entity { get; }
        int Level { get; }
        IEntity Parent { get; }
        PropertyInfo PropertyInfo { get; }
        bool IsPartOfCollection { get; }
    }

    public class EntityInfo : IEntityInfo
    {
        public EntityInfo(IEntity entity, int level, IEntity parent, PropertyInfo propertyInfo, bool isPartOfCollection)
        {
            Entity = entity;
            Level = level;
            Parent = parent;
            PropertyInfo = propertyInfo;
            IsPartOfCollection = isPartOfCollection;
        }

        public IEntity Entity { get; private set; }
        public int Level { get; private set; }
        public IEntity Parent { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public bool IsPartOfCollection { get; private set; }
    }

    
}