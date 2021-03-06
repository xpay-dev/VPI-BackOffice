USE [VeritasPay]
GO
SET IDENTITY_INSERT [dbo].[TransactionEntryTypes] ON 

INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (9, N'EMV')
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (10, N'Contactless')
SET IDENTITY_INSERT [dbo].[TransactionEntryTypes] OFF

Update [dbo].[AccountType] set AccountName = 'Chequing' where AccountTypeId = 1
Update [dbo].[AccountType] set AccountName = 'Savings' where AccountTypeId = 2
Update [dbo].[AccountType] set AccountName = 'Current' where AccountTypeId = 3

Update dbo.CardTypes set TypeName = 'ChinaUnionPay' where CardTypeId = 7

Update [dbo].[PaymentTypes] set TypeName = 'Credit' where PaymentTypeId = 1
Update [dbo].[PaymentTypes] set TypeName = 'Debit' where PaymentTypeId = 2
Update [dbo].[PaymentTypes]] set TypeName = 'Cash' where PaymentTypeId = 3
Update [dbo].[PaymentTypes] set TypeName = 'EMV' where PaymentTypeId = 4
Update [dbo].[PaymentTypes] set TypeName = 'ACH' where PaymentTypeId = 5
Update [dbo].[PaymentTypes]] set TypeName = 'ALL' where PaymentTypeId = 6


USE [VeritasPay]
GO
SET IDENTITY_INSERT [dbo].[Currencies] ON 

INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyCode], [CurrencyName], [IsoCode], [IsEnabled]) VALUES (22, N'CNY', N'Chinese Yuan', N'156', 1)
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyCode], [CurrencyName], [IsoCode], [IsEnabled]) VALUES (23, N'MOP', N'Macau Pataca', N'446', 1)
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyCode], [CurrencyName], [IsoCode], [IsEnabled]) VALUES (24, N'NKW', N'North Korean Won', N'000', 1)
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyCode], [CurrencyName], [IsoCode], [IsEnabled]) VALUES (25, N'THB', N'Thai Baht', N'000', 1)

SET IDENTITY_INSERT [dbo].[Currencies] OFF

USE [VeritasPay]
GO
SET IDENTITY_INSERT [dbo].[TransactionTypes] ON 

INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (9, N'Reversed')
SET IDENTITY_INSERT [dbo].[TransactionTypes] OFF


USE [VeritasPay]
GO
SET IDENTITY_INSERT [dbo].[Switches] ON 

INSERT [dbo].[Switches] ([SwitchId], [SwitchName], [SwitchCode], [IsActive], [IsAddressRequired], [DateCreated]) VALUES (34, N'Visa Direct Offline', N'MVisaOffline', 1, 0, CAST(N'2017-04-18 01:16:06.690' AS DateTime))
SET IDENTITY_INSERT [dbo].[Switches] OFF


