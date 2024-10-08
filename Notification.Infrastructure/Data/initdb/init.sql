CREATE TABLE notification_info
(
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Recipient" character varying  NOT NULL,
    "Message" character varying NOT NULL,
    "NotificationType" integer NOT NULL,
    "NotificationStatus" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "DeletedAt" timestamp without time zone,
    CONSTRAINT notification_info_pkey PRIMARY KEY ("Id")
)

