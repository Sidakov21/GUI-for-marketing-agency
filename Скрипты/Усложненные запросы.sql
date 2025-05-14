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




------ функция для вычисления эффективности кампании

----CREATE FUNCTION dbo.fn_GetCampaignCTR (@CampaignId INT)
----RETURNS DECIMAL (5,2)
----AS
----BEGIN
----	DECLARE @CTR DECIMAL(5,2)

----	SELECT @CTR = 
----			CASE 
----				WHEN SUM(impressions) > 0 THEN CAST(SUM(clicks) * 100.0 / SUM(impressions) AS DECIMAL(5,2))
----				ELSE 0.00
----			END
----    FROM Campaign_Results
----    WHERE campaign_id = @CampaignId

----    RETURN @CTR
----END;

--SELECT dbo.fn_GetCampaignCTR(1) AS CTR_Percent;
