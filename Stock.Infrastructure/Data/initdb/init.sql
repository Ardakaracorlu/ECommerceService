CREATE TABLE stock_info
(
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "DeletedAt" timestamp without time zone,
    CONSTRAINT stockinfo_pkey PRIMARY KEY ("Id")
);

insert into stock_info(
	"Id", "ProductId", "Quantity", "CreatedAt")
	VALUES ('fce013f1-1e71-4478-886e-bd3cf0cd1c69', '3fa85f64-5717-4562-b3fc-2c963f66afa6', 200, '2024-08-10');