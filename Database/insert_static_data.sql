
insert [role] values(1, 'Administrator')
insert [role] values(2, 'Order Processor')
insert [role] values(3, 'Customer')
insert [role] values(4, 'Guest')

insert [user] (Email, [Password], RoleId, IsEnabled) 
	values('admin@sutekishop.co.uk', '', 1, 1)