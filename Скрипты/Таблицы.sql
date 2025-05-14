-- �������� ������� Vendors
CREATE TABLE Vendors (
    vendor_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    service_type NVARCHAR(100) NOT NULL,
    contact_info NVARCHAR(255) NOT NULL,
    contract_terms NVARCHAR(MAX)
);

-- �������� ������� Clients
CREATE TABLE Clients (
    client_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    contact_info NVARCHAR(255) NOT NULL,
    industry NVARCHAR(100) NOT NULL,
    contract_date DATE NOT NULL,
    contract_status NVARCHAR(50) NOT NULL
);

-- �������� ������� Campaigns
CREATE TABLE Campaigns (
    campaign_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    budget DECIMAL(18, 2) NOT NULL,
    status NVARCHAR(50) NOT NULL,

    client_id INT NOT NULL FOREIGN KEY REFERENCES Clients(client_id)
);

-- �������� ������� Channels
CREATE TABLE Channels (
    channel_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    type NVARCHAR(50)
);

-- �������� ������� Employees
CREATE TABLE Employees (
    employee_id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(100) NOT NULL,
    position NVARCHAR(100) NOT NULL,
    contact_info NVARCHAR(200) NOT NULL,
    salary DECIMAL(18, 2) NOT NULL
);

-- �������� ������� Payments
CREATE TABLE Payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    amount DECIMAL(18, 2) NOT NULL,
    payment_data DATE NOT NULL,
    payment_type NVARCHAR(50) NOT NULL,

    campaign_id INT NOT NULL FOREIGN KEY REFERENCES Campaigns(campaign_id)
);

-- �������� ������� Campaign_Results
CREATE TABLE Campaign_Results (
    result_id INT PRIMARY KEY IDENTITY(1,1),
    impressions INT NOT NULL,
    clicks INT NOT NULL,
    conversions INT NOT NULL,
    lead_count INT NOT NULL,
    roi DECIMAL(8, 4) NOT NULL,
    cost DECIMAL(18, 2)	NOT NULL,
    end_month DATE NOT NULL,
    campaign_id INT NOT NULL FOREIGN KEY REFERENCES Campaigns(campaign_id)
);

-- �������� ������������� ������� ��� ����� Campaigns � Channels
CREATE TABLE Campaign_Channels (
    campaign_id INT FOREIGN KEY REFERENCES Campaigns(campaign_id),
    channel_id INT FOREIGN KEY REFERENCES Channels(channel_id),
    PRIMARY KEY (campaign_id, channel_id)
);

-- �������� ������������� ������� ��� ����� ����� Vendors � Campaigns
CREATE TABLE Vendor_Campaigns (
    vendor_id INT FOREIGN KEY REFERENCES Vendors(vendor_id),
    campaign_id INT FOREIGN KEY REFERENCES Campaigns(campaign_id),
    PRIMARY KEY (vendor_id, campaign_id)
);

-- �������� ������������� ������� ��� ����� ����� Employees � Campaigns
CREATE TABLE Employee_Campaigns (
    employee_id INT FOREIGN KEY REFERENCES Employees(employee_id),
    campaign_id INT FOREIGN KEY REFERENCES Campaigns(campaign_id),
    PRIMARY KEY (employee_id, campaign_id)
);