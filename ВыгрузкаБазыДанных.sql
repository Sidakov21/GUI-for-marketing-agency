USE [База данных для маркетингового агентства]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCampaignCTR]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
------ Процедура для получения данных по ID кампании

----CREATE PROCEDURE GetCampaignDetails
----    @CampaignId INT
----AS
----BEGIN
----    SET NOCOUNT ON;

----    SELECT 
----        c.campaign_id,
----        c.name AS campaign_name,
----        cl.name AS client_name,
----        c.start_date,
----        c.end_date,
----        c.budget,
----        c.status,
----        ISNULL(SUM(p.amount), 0) AS total_payments,
----        ISNULL(SUM(cr.impressions), 0) AS total_impressions,
----        ISNULL(SUM(cr.clicks), 0) AS total_clicks,
----        ISNULL(AVG(cr.roi), 0) AS avg_roi
----    FROM Campaigns c
----    JOIN Clients cl ON c.client_id = cl.client_id
----    LEFT JOIN Payments p ON c.campaign_id = p.campaign_id
----    LEFT JOIN Campaign_Results cr ON c.campaign_id = cr.campaign_id
----    WHERE c.campaign_id = @CampaignId
----    GROUP BY 
----        c.campaign_id, c.name, cl.name, c.start_date, c.end_date, c.budget, c.status;
----END

--EXEC GetCampaignDetails @CampaignId = 2;


------ Представление показывает общую информацию по кампаниям

----CREATE VIEW vw_CampaignSummary AS
----SELECT 
----    c.campaign_id,
----    c.name AS campaign_name,
----    cl.name AS client_name,
----    c.start_date,
----    c.end_date,
----    c.budget,
----    c.status,
----    ISNULL(SUM(p.amount), 0) AS total_payments
----FROM Campaigns c
----JOIN Clients cl ON c.client_id = cl.client_id
----LEFT JOIN Payments p ON c.campaign_id = p.campaign_id
----GROUP BY 
----    c.campaign_id, c.name, cl.name, c.start_date, c.end_date, c.budget, c.status;

--SELECT * FROM vw_CampaignSummary;




-- функция для вычисления эффективности кампании

CREATE FUNCTION [dbo].[fn_GetCampaignCTR] (@CampaignId INT)
RETURNS DECIMAL (5,2)
AS
BEGIN
	DECLARE @CTR DECIMAL(5,2)

	SELECT @CTR = 
			CASE 
				WHEN SUM(impressions) > 0 THEN CAST(SUM(clicks) * 100.0 / SUM(impressions) AS DECIMAL(5,2))
				ELSE 0.00
			END
    FROM Campaign_Results
    WHERE campaign_id = @CampaignId

    RETURN @CTR
END;
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[client_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[contact_info] [nvarchar](255) NOT NULL,
	[industry] [nvarchar](100) NOT NULL,
	[contract_date] [date] NOT NULL,
	[contract_status] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[client_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Campaigns]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Campaigns](
	[campaign_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[start_date] [date] NOT NULL,
	[end_date] [date] NOT NULL,
	[budget] [decimal](18, 2) NOT NULL,
	[status] [nvarchar](50) NOT NULL,
	[client_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[campaign_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[payment_id] [int] IDENTITY(1,1) NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[payment_data] [date] NOT NULL,
	[payment_type] [nvarchar](50) NOT NULL,
	[campaign_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_CampaignSummary]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
------ Процедура для получения данных по ID кампании

----CREATE PROCEDURE GetCampaignDetails
----    @CampaignId INT
----AS
----BEGIN
----    SET NOCOUNT ON;

----    SELECT 
----        c.campaign_id,
----        c.name AS campaign_name,
----        cl.name AS client_name,
----        c.start_date,
----        c.end_date,
----        c.budget,
----        c.status,
----        ISNULL(SUM(p.amount), 0) AS total_payments,
----        ISNULL(SUM(cr.impressions), 0) AS total_impressions,
----        ISNULL(SUM(cr.clicks), 0) AS total_clicks,
----        ISNULL(AVG(cr.roi), 0) AS avg_roi
----    FROM Campaigns c
----    JOIN Clients cl ON c.client_id = cl.client_id
----    LEFT JOIN Payments p ON c.campaign_id = p.campaign_id
----    LEFT JOIN Campaign_Results cr ON c.campaign_id = cr.campaign_id
----    WHERE c.campaign_id = @CampaignId
----    GROUP BY 
----        c.campaign_id, c.name, cl.name, c.start_date, c.end_date, c.budget, c.status;
----END

--EXEC GetCampaignDetails @CampaignId = 2;


-- Представление показывает общую информацию по кампаниям

CREATE VIEW [dbo].[vw_CampaignSummary] AS
SELECT 
    c.campaign_id,
    c.name AS campaign_name,
    cl.name AS client_name,
    c.start_date,
    c.end_date,
    c.budget,
    c.status,
    ISNULL(SUM(p.amount), 0) AS total_payments
FROM Campaigns c
JOIN Clients cl ON c.client_id = cl.client_id
LEFT JOIN Payments p ON c.campaign_id = p.campaign_id
GROUP BY 
    c.campaign_id, c.name, cl.name, c.start_date, c.end_date, c.budget, c.status;
GO
/****** Object:  Table [dbo].[Campaign_Channels]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Campaign_Channels](
	[campaign_id] [int] NOT NULL,
	[channel_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[campaign_id] ASC,
	[channel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Campaign_Results]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Campaign_Results](
	[result_id] [int] IDENTITY(1,1) NOT NULL,
	[impressions] [int] NOT NULL,
	[clicks] [int] NOT NULL,
	[conversions] [int] NOT NULL,
	[lead_count] [int] NOT NULL,
	[roi] [decimal](8, 4) NOT NULL,
	[cost] [decimal](18, 2) NOT NULL,
	[end_month] [date] NOT NULL,
	[campaign_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[result_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Channels]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channels](
	[channel_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[type] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[channel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee_Campaigns]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee_Campaigns](
	[employee_id] [int] NOT NULL,
	[campaign_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[employee_id] ASC,
	[campaign_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[employee_id] [int] IDENTITY(1,1) NOT NULL,
	[full_name] [nvarchar](100) NOT NULL,
	[position] [nvarchar](100) NOT NULL,
	[contact_info] [nvarchar](200) NOT NULL,
	[salary] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[employee_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendor_Campaigns]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendor_Campaigns](
	[vendor_id] [int] NOT NULL,
	[campaign_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[vendor_id] ASC,
	[campaign_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 14.05.2025 6:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[vendor_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[service_type] [nvarchar](100) NOT NULL,
	[contact_info] [nvarchar](255) NOT NULL,
	[contract_terms] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[vendor_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[Campaign_Channels] ([campaign_id], [channel_id]) VALUES (1, 1)
INSERT [dbo].[Campaign_Channels] ([campaign_id], [channel_id]) VALUES (2, 2)
GO
SET IDENTITY_INSERT [dbo].[Campaign_Results] ON 

INSERT [dbo].[Campaign_Results] ([result_id], [impressions], [clicks], [conversions], [lead_count], [roi], [cost], [end_month], [campaign_id]) VALUES (1, 100000, 1000, 100, 50, CAST(5.0000 AS Decimal(8, 4)), CAST(5000.00 AS Decimal(18, 2)), CAST(N'2023-03-31' AS Date), 1)
INSERT [dbo].[Campaign_Results] ([result_id], [impressions], [clicks], [conversions], [lead_count], [roi], [cost], [end_month], [campaign_id]) VALUES (2, 150000, 1500, 150, 75, CAST(6.0000 AS Decimal(8, 4)), CAST(7500.00 AS Decimal(18, 2)), CAST(N'2023-04-30' AS Date), 2)
INSERT [dbo].[Campaign_Results] ([result_id], [impressions], [clicks], [conversions], [lead_count], [roi], [cost], [end_month], [campaign_id]) VALUES (3, 200000, 23455, 200, 200, CAST(70.0000 AS Decimal(8, 4)), CAST(7000.00 AS Decimal(18, 2)), CAST(N'2025-05-31' AS Date), 15)
SET IDENTITY_INSERT [dbo].[Campaign_Results] OFF
GO
SET IDENTITY_INSERT [dbo].[Campaigns] ON 

INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (1, N'Campaign 1', CAST(N'2023-03-01' AS Date), CAST(N'2023-03-31' AS Date), CAST(10000.00 AS Decimal(18, 2)), N'Active', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (2, N'Campaign 2', CAST(N'2023-04-01' AS Date), CAST(N'2023-04-30' AS Date), CAST(15000.00 AS Decimal(18, 2)), N'Приостановлена', 2)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (9, N'Campaign 4', CAST(N'2025-03-14' AS Date), CAST(N'2025-03-22' AS Date), CAST(13000.00 AS Decimal(18, 2)), N'Приостановлена', 2)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (11, N'Campaign 5', CAST(N'2025-03-21' AS Date), CAST(N'2025-03-30' AS Date), CAST(0.00 AS Decimal(18, 2)), N'Активна', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (14, N'Campaign 6', CAST(N'2025-03-15' AS Date), CAST(N'2025-03-30' AS Date), CAST(0.00 AS Decimal(18, 2)), N'Активна', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (15, N'Campaign 7', CAST(N'2025-03-08' AS Date), CAST(N'2025-03-27' AS Date), CAST(15000.00 AS Decimal(18, 2)), N'Активна', 2)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (17, N'Campaign 8', CAST(N'2025-03-16' AS Date), CAST(N'2025-03-28' AS Date), CAST(15000.00 AS Decimal(18, 2)), N'Приостановлена', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (19, N'Camp', CAST(N'2025-03-23' AS Date), CAST(N'2025-03-30' AS Date), CAST(0.00 AS Decimal(18, 2)), N'Активна', 2)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (23, N'Сamp 4', CAST(N'2025-02-28' AS Date), CAST(N'2025-03-22' AS Date), CAST(12000.00 AS Decimal(18, 2)), N'Приостановлена', 2)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (25, N'С 1', CAST(N'2025-03-05' AS Date), CAST(N'2025-03-29' AS Date), CAST(1122.00 AS Decimal(18, 2)), N'Активна', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (27, N'С 3', CAST(N'2025-03-27' AS Date), CAST(N'2025-03-30' AS Date), CAST(0.00 AS Decimal(18, 2)), N'Активна', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (1003, N'M1', CAST(N'2025-05-16' AS Date), CAST(N'2025-05-24' AS Date), CAST(11000.00 AS Decimal(18, 2)), N'Приостановлена', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (1004, N'M2', CAST(N'2025-05-23' AS Date), CAST(N'2025-05-29' AS Date), CAST(150000.00 AS Decimal(18, 2)), N'Активна', 1)
INSERT [dbo].[Campaigns] ([campaign_id], [name], [start_date], [end_date], [budget], [status], [client_id]) VALUES (1005, N'Z1', CAST(N'2025-05-17' AS Date), CAST(N'2025-05-23' AS Date), CAST(12300.30 AS Decimal(18, 2)), N'Завершена', 2)
SET IDENTITY_INSERT [dbo].[Campaigns] OFF
GO
SET IDENTITY_INSERT [dbo].[Channels] ON 

INSERT [dbo].[Channels] ([channel_id], [name], [type]) VALUES (1, N'Channel A', N'Social Media')
INSERT [dbo].[Channels] ([channel_id], [name], [type]) VALUES (2, N'Channel B', N'Email')
SET IDENTITY_INSERT [dbo].[Channels] OFF
GO
SET IDENTITY_INSERT [dbo].[Clients] ON 

INSERT [dbo].[Clients] ([client_id], [name], [contact_info], [industry], [contract_date], [contract_status]) VALUES (1, N'Client X', N'contact@clientX.com', N'Retail', CAST(N'2023-01-01' AS Date), N'Active')
INSERT [dbo].[Clients] ([client_id], [name], [contact_info], [industry], [contract_date], [contract_status]) VALUES (2, N'Client Y', N'contact@clientY.com', N'Technology', CAST(N'2023-03-05' AS Date), N'Активен')
INSERT [dbo].[Clients] ([client_id], [name], [contact_info], [industry], [contract_date], [contract_status]) VALUES (4, N'Client Z', N'999 999 99 99', N'Retail', CAST(N'2025-05-13' AS Date), N'Активен')
INSERT [dbo].[Clients] ([client_id], [name], [contact_info], [industry], [contract_date], [contract_status]) VALUES (5, N'Client G', N'contact@clientG.com', N'Technology', CAST(N'2025-05-14' AS Date), N'Завершен')
SET IDENTITY_INSERT [dbo].[Clients] OFF
GO
INSERT [dbo].[Employee_Campaigns] ([employee_id], [campaign_id]) VALUES (1, 1)
INSERT [dbo].[Employee_Campaigns] ([employee_id], [campaign_id]) VALUES (2, 2)
GO
SET IDENTITY_INSERT [dbo].[Employees] ON 

INSERT [dbo].[Employees] ([employee_id], [full_name], [position], [contact_info], [salary]) VALUES (1, N'John Doe', N'Manager', N'john.doe@company.com', CAST(5000.00 AS Decimal(18, 2)))
INSERT [dbo].[Employees] ([employee_id], [full_name], [position], [contact_info], [salary]) VALUES (2, N'Jane Smith', N'Analyst', N'jane.smith@company.com', CAST(4000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[Employees] OFF
GO
SET IDENTITY_INSERT [dbo].[Payments] ON 

INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (1, CAST(5000.00 AS Decimal(18, 2)), CAST(N'2023-03-15' AS Date), N'Credit Card', 1)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (2, CAST(7500.00 AS Decimal(18, 2)), CAST(N'2023-04-15' AS Date), N'Bank Transfer', 2)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (6, CAST(1200.32 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Наличные', 1005)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (7, CAST(1200.33 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Наличные', 1004)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (8, CAST(133.44 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Безналичные', 19)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (9, CAST(12.00 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Безналичные', 1004)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (10, CAST(1.00 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Наличные', 1003)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (11, CAST(1.00 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Перевод', 11)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (13, CAST(111.00 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Безналичные', 25)
INSERT [dbo].[Payments] ([payment_id], [amount], [payment_data], [payment_type], [campaign_id]) VALUES (14, CAST(15000.00 AS Decimal(18, 2)), CAST(N'2025-05-13' AS Date), N'Перевод', 1005)
SET IDENTITY_INSERT [dbo].[Payments] OFF
GO
INSERT [dbo].[Vendor_Campaigns] ([vendor_id], [campaign_id]) VALUES (1, 1)
INSERT [dbo].[Vendor_Campaigns] ([vendor_id], [campaign_id]) VALUES (2, 2)
GO
SET IDENTITY_INSERT [dbo].[Vendors] ON 

INSERT [dbo].[Vendors] ([vendor_id], [name], [service_type], [contact_info], [contract_terms]) VALUES (1, N'Vendor A', N'Marketing', N'contact@vendorA.com', N'1 year contract')
INSERT [dbo].[Vendors] ([vendor_id], [name], [service_type], [contact_info], [contract_terms]) VALUES (2, N'Vendor B', N'Advertising', N'contact@vendorB.com', N'2 years contract')
SET IDENTITY_INSERT [dbo].[Vendors] OFF
GO
ALTER TABLE [dbo].[Campaign_Channels]  WITH CHECK ADD FOREIGN KEY([campaign_id])
REFERENCES [dbo].[Campaigns] ([campaign_id])
GO
ALTER TABLE [dbo].[Campaign_Channels]  WITH CHECK ADD FOREIGN KEY([channel_id])
REFERENCES [dbo].[Channels] ([channel_id])
GO
ALTER TABLE [dbo].[Campaign_Results]  WITH CHECK ADD FOREIGN KEY([campaign_id])
REFERENCES [dbo].[Campaigns] ([campaign_id])
GO
ALTER TABLE [dbo].[Campaigns]  WITH CHECK ADD FOREIGN KEY([client_id])
REFERENCES [dbo].[Clients] ([client_id])
GO
ALTER TABLE [dbo].[Employee_Campaigns]  WITH CHECK ADD FOREIGN KEY([campaign_id])
REFERENCES [dbo].[Campaigns] ([campaign_id])
GO
ALTER TABLE [dbo].[Employee_Campaigns]  WITH CHECK ADD FOREIGN KEY([employee_id])
REFERENCES [dbo].[Employees] ([employee_id])
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD FOREIGN KEY([campaign_id])
REFERENCES [dbo].[Campaigns] ([campaign_id])
GO
ALTER TABLE [dbo].[Vendor_Campaigns]  WITH CHECK ADD FOREIGN KEY([campaign_id])
REFERENCES [dbo].[Campaigns] ([campaign_id])
GO
ALTER TABLE [dbo].[Vendor_Campaigns]  WITH CHECK ADD FOREIGN KEY([vendor_id])
REFERENCES [dbo].[Vendors] ([vendor_id])
GO
/****** Object:  StoredProcedure [dbo].[GetCampaignDetails]    Script Date: 14.05.2025 6:23:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Процедура для получения данных по ID кампании

CREATE PROCEDURE [dbo].[GetCampaignDetails]
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.campaign_id,
        c.name AS campaign_name,
        cl.name AS client_name,
        c.start_date,
        c.end_date,
        c.budget,
        c.status,
        ISNULL(SUM(p.amount), 0) AS total_payments,
        ISNULL(SUM(cr.impressions), 0) AS total_impressions,
        ISNULL(SUM(cr.clicks), 0) AS total_clicks,
        ISNULL(AVG(cr.roi), 0) AS avg_roi
    FROM Campaigns c
    JOIN Clients cl ON c.client_id = cl.client_id
    LEFT JOIN Payments p ON c.campaign_id = p.campaign_id
    LEFT JOIN Campaign_Results cr ON c.campaign_id = cr.campaign_id
    WHERE c.campaign_id = @CampaignId
    GROUP BY 
        c.campaign_id, c.name, cl.name, c.start_date, c.end_date, c.budget, c.status;
END

--EXEC GetCampaignDetails @CampaignId = 1;

GO
