use SutekiShop


insert [role] (RoleId, [Name]) values(1, 'Administrator')
insert [role] (RoleId, [Name]) values(2, 'Order Processor')
insert [role] (RoleId, [Name]) values(3, 'Customer')
insert [role] (RoleId, [Name]) values(4, 'Guest')

insert [user] (Email, [Password], RoleId, IsEnabled) 
	values('admin@sutekishop.co.uk', 'D033E22AE348AEB5660FC2140AEC35850C4DA997', 1, 1)

insert category ([Name], ParentId, Position) values('- Root', null, 1)

insert CardType values(1,	'Visa / Delta / Electron',	0)
insert CardType values(2,	'Master Card / Euro Card',	0)
insert CardType values(3,	'American Express',	0)
insert CardType values(4,	'Switch / Solo / Maestro',	1)

insert OrderStatus values(1, 'Created')
insert OrderStatus values(2, 'Dispatched')
insert OrderStatus values(3, 'Rejected')

insert ContentType values(1, 'Text')
insert ContentType values(2, 'Action')

set identity_insert menu on
insert menu(menuId, parentMenuId, [Name], Position, IsActive) values(1, null, 'Main', 1, 1)
set identity_insert menu off

set identity_insert [content] on
insert [content](contentId, menuId, contentTypeId, [name], UrlName, [text], controller, [action], position, isActive)
values(1, 1, 2, 'Online Shop', null, null, 'Home', 'Index', 1, 1)
insert [content](contentId, menuId, contentTypeId, [name], UrlName, [text], controller, [action], position, isActive)
values(2, 1, 1, 'Home', 'Home', 'Homepage Content', null, null, 2, 1)
set identity_insert [content] off