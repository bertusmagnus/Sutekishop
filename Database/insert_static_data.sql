use SutekiShop


insert [role] (RoleId, [Name]) values(1, 'Administrator')
insert [role] (RoleId, [Name]) values(2, 'Order Processor')
insert [role] (RoleId, [Name]) values(3, 'Customer')
insert [role] (RoleId, [Name]) values(4, 'Guest')

insert [user] (Email, [Password], RoleId, IsEnabled) 
	values('admin@sutekishop.co.uk', 'D033E22AE348AEB5660FC2140AEC35850C4DA997', 1, 1)

insert category ([Name], ParentId) values('- Root', null)

insert CardType values(1,	'Visa / Delta / Electron',	0)
insert CardType values(2,	'Master Card / Euro Card',	0)
insert CardType values(3,	'American Express',	0)
insert CardType values(4,	'Switch / Solo / Maestro',	1)


