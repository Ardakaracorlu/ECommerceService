CREATE TABLE order_info (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "CustomerName" character varying(255) NOT NULL,
    "CustomerSurname" character varying(255) NOT NULL,
    "Adress" character varying(255) NOT NULL,
    "Phone" character varying(11) NOT NULL,
    "Email" character varying(50) NOT NULL,
    "Status" integer NOT NULL,
    "Description" character varying(500),
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "DeletedAt" timestamp without time zone,
    CONSTRAINT "OrderInfo_pkey" PRIMARY KEY ("Id")

);
