-------------------
-- CREATE TABLES --
-------------------

CREATE TABLE IF NOT EXISTS seller
(
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cpf        TEXT NOT NULL UNIQUE,
    name       TEXT NOT NULL,
    email      TEXT NOT NULL UNIQUE,
    telephone  TEXT NOT NULL UNIQUE,
    created_at TIMESTAMP        DEFAULT NOW(),
    updated_at TIMESTAMP,
    deleted_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS product
(
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name       TEXT    NOT NULL UNIQUE,
    amount     SMALLINT         DEFAULT 0,
    price      DECIMAL NOT NULL,
    created_at TIMESTAMP        DEFAULT NOW(),
    updated_at TIMESTAMP,
    deleted_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS purchase_status
(
    id         SMALLINT PRIMARY KEY,
    status     TEXT UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP,
    deleted_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS purchase
(
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    seller_id          UUID REFERENCES seller (id) NOT NULL,
    purchase_status_id SMALLINT REFERENCES purchase_status (id),
    created_at         TIMESTAMP        DEFAULT NOW(),
    updated_at         TIMESTAMP,
    deleted_at         TIMESTAMP
);

CREATE TABLE IF NOT EXISTS purchase_product
(
    purchase_id    UUID REFERENCES purchase (id) NOT NULL,
    product_id     UUID REFERENCES product (id)  NOT NULL,
    product_amount SMALLINT  DEFAULT 1,
    created_at     TIMESTAMP DEFAULT NOW(),
    updated_at     TIMESTAMP,
    deleted_at     TIMESTAMP,
    PRIMARY KEY (purchase_id, product_id)
);

---------------------------
-- INSERT DEFAULT VALUES --
---------------------------

INSERT INTO seller (cpf, name, email, telephone)
VALUES ('123.456.789-01', 'First seller', 'firstseller@gmail.com', '+55(31)91234-5678'),
       ('234.567.890-12', 'Second seller', 'secondseller@gmail.com', '+55(31)92345-6789'),
       ('345.678.901-23', 'Third seller', 'thirdseller@gmail.com', '+55(31)93456-7890');

INSERT INTO product (name, amount, price)
VALUES ('Playstation 5', 100, 4549.99),
       ('Playstation 4', 500, 2499.99),
       ('Playstation 3', 25, 1249.99),
       ('Xbox Series X', 100, 3999.99),
       ('Xbox One', 25, 1999.99),
       ('Xbox 360', 25, 999.99);

INSERT INTO purchase_status
VALUES (100, 'Aguardando pagamento'),
       (200, 'Pagamento aprovado'),
       (201, 'Enviado para transportadora'),
       (202, 'Entregue'),
       (300, 'Rejeitado'),
       (400, 'Cancelado');

----------------------------
-- FUNCTIONS AND TRIGGERS --
----------------------------

CREATE OR REPLACE FUNCTION trigger_set_timestamp() RETURNS TRIGGER AS
$$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$
    LANGUAGE 'plpgsql';

--

CREATE TRIGGER seller_updated_at
    BEFORE UPDATE
    ON seller
    FOR EACH ROW
EXECUTE PROCEDURE trigger_set_timestamp();

CREATE TRIGGER product_updated_at
    BEFORE UPDATE
    ON product
    FOR EACH ROW
EXECUTE PROCEDURE trigger_set_timestamp();

CREATE TRIGGER purchase_status_updated_at
    BEFORE UPDATE
    ON purchase_status
    FOR EACH ROW
EXECUTE PROCEDURE trigger_set_timestamp();

CREATE TRIGGER purchase_updated_at
    BEFORE UPDATE
    ON purchase
    FOR EACH ROW
EXECUTE PROCEDURE trigger_set_timestamp();

CREATE TRIGGER purchase_product_updated_at
    BEFORE UPDATE
    ON purchase_product
    FOR EACH ROW
EXECUTE PROCEDURE trigger_set_timestamp();