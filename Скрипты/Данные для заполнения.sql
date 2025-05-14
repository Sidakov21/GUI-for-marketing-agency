-- ������� ������ � ������� Vendors
INSERT INTO Vendors (name, service_type, contact_info, contract_terms)
VALUES ('Vendor A', 'Marketing', 'contact@vendorA.com', '1 year contract'),
       ('Vendor B', 'Advertising', 'contact@vendorB.com', '2 years contract');

-- ������� ������ � ������� Clients
INSERT INTO Clients (name, contact_info, industry, contract_date, contract_status)
VALUES ('Client X', 'contact@clientX.com', 'Retail', '2023-01-01', 'Active'),
       ('Client Y', 'contact@clientY.com', 'Technology', '2023-02-01', 'Pending');

-- ������� ������ � ������� Campaigns
INSERT INTO Campaigns (name, start_date, end_date, budget, status, client_id)
VALUES ('Campaign 1', '2023-03-01', '2023-03-31', 10000.00, 'Active', 1),
       ('Campaign 2', '2023-04-01', '2023-04-30', 15000.00, 'Pending', 2);

-- ������� ������ � ������� Channels
INSERT INTO Channels (name, type)
VALUES ('Channel A', 'Social Media'), 
       ('Channel B', 'Email');

-- ������� ������ � ������� Employees
INSERT INTO Employees (full_name, position, contact_info, salary)
VALUES ('John Doe', 'Manager', 'john.doe@company.com', 5000.00),
       ('Jane Smith', 'Analyst', 'jane.smith@company.com', 4000.00);

-- ������� ������ � ������� Payments
INSERT INTO Payments (amount, payment_data, payment_type, campaign_id)
VALUES (5000.00, '2023-03-15', 'Credit Card', 1),
       (7500.00, '2023-04-15', 'Bank Transfer', 2);

-- ������� ������ � ������� Campaign_Results
INSERT INTO Campaign_Results (impressions, clicks, conversions, lead_count, roi, cost, end_month, campaign_id)
VALUES (100000, 1000, 100, 50, 5.00, 5000.00, '2023-03-31', 1),
       (150000, 1500, 150, 75, 6.00, 7500.00, '2023-04-30', 2);

-- ������� ������ � ������������� ������� Campaign_Channels
INSERT INTO Campaign_Channels (campaign_id, channel_id)
VALUES (1, 1),
       (2, 2);

-- ������� ������ � ������������� ������� Vendor_Campaigns
INSERT INTO Vendor_Campaigns (vendor_id, campaign_id)
VALUES (1, 1), -- Vendor A ������ � Campaign 1
       (2, 2); -- Vendor B ������ � Campaign 2

-- ������� ������ � ������������� ������� Employee_Campaigns
INSERT INTO Employee_Campaigns (employee_id, campaign_id)
VALUES (1, 1), -- John Doe ������ � Campaign 1
       (2, 2); -- Jane Smith ������ � Campaign 2