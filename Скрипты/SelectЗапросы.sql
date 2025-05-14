----Вывод всех кампаний с названием клиента вместо client_id

--SELECT 
--    c.campaign_id,
--    c.name AS campaign_name,
--    cl.name AS client_name,
--    c.budget,
--    c.status,
--    c.start_date,
--    c.end_date
--FROM Campaigns c
--JOIN Clients cl ON c.client_id = cl.client_id
--ORDER BY c.start_date;


----Подсчет общего количества кликов и среднего ROI по каждой кампании

--SELECT 
--    cr.campaign_id,
--    c.name AS campaign_name,
--    SUM(cr.clicks) AS total_clicks,
--    AVG(cr.roi) AS average_roi
--FROM Campaign_Results cr
--JOIN Campaigns c ON cr.campaign_id = c.campaign_id
--GROUP BY cr.campaign_id, c.name
--ORDER BY total_clicks DESC;

---- Вывод платежей по кампании с фильтром по типу оплаты и датам

--SELECT 
--    p.payment_id,
--    c.name AS campaign_name,
--    p.amount,
--    p.payment_data,
--    p.payment_type
--FROM Payments p
--JOIN Campaigns c 
--ON p.campaign_id = c.campaign_id WHERE p.payment_type = 'Безналичные'
--	AND p.payment_data BETWEEN '2025-01-01' AND '2025-12-31'
--ORDER BY p.amount DESC;




