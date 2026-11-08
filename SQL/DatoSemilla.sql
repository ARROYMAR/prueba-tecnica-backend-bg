USE CarritoDB
GO

-- Se insertan datos del usuario
INSERT INTO dbo.Usuario (Usuario, Email, Nombre, Password, Rol) VALUES
(N'admin',   N'administrator',   N'Administrador del Sistema',
 N'AQAAAAIAAYagAAAAEHJMzFbGoLv7x7H3yJr+W5StZe5ciG6Zg7Wn0bRkWmlA+XAwraOg0g3ohnYnwfLTsA==', 'Admin'),
(N'cliente', N'cliente@carrito.test', N'Cliente',
 N'AQAAAAIAAYagAAAAEMr+TXubOBzR8n92mjAz+RLxjfET3PXs4utZlfghvgYDIgmjDHtVrXZ/dt1RXCj9HA==', 'Cliente');

-- Se insertan datos de categoria
INSERT INTO dbo.Categoria (Descripcion) VALUES
('Tecnología'), ('Hogar'), ('Deportes'), ('Libros'), ('Oficina');
GO

-- Se insertan datos del producto
INSERT INTO dbo.Producto (Codigo, Producto, Descripcion, CategoriaId, Precio, Stock) VALUES
(N'TEC-001', N'Mouse inalámbrico',        N'Mouse óptico 2.4 GHz con receptor USB',            1,  15.99, 50),
(N'TEC-002', N'Teclado mecánico',         N'Teclado mecánico retroiluminado, switches rojos',  1,  59.90, 25),
(N'TEC-003', N'Audífonos Bluetooth',      N'Audífonos con cancelación de ruido y micrófono',   1,  79.50, 15),
(N'TEC-004', N'Monitor 24 pulgadas',      N'Monitor Full HD IPS 75 Hz',                        1, 139.00,  8),
(N'TEC-005', N'Memoria USB 64 GB',        N'Memoria USB 3.0 de 64 GB',                         1,   8.75,100),
(N'HOG-001', N'Lámpara de escritorio LED',N'Lámpara regulable con puerto de carga USB',        2,  22.40, 30),
(N'HOG-002', N'Licuadora 600 W',          N'Licuadora con jarra de vidrio de 1.5 L',           2,  45.00, 12),
(N'HOG-003', N'Juego de sábanas queen',   N'Juego de sábanas de algodón, 4 piezas',            2,  34.99, 20),
(N'DEP-001', N'Balón de fútbol N.º 5',    N'Balón profesional de cuero sintético',             3,  24.50, 40),
(N'DEP-002', N'Mancuernas 5 kg (par)',    N'Par de mancuernas recubiertas de vinilo',          3,  29.90, 18),
(N'DEP-003', N'Colchoneta de yoga',       N'Colchoneta antideslizante de 6 mm',                3,  18.00, 35),
(N'LIB-001', N'Clean Code',               N'Manual de estilo para desarrollo ágil de software',4,  38.00, 10),
(N'LIB-002', N'El Principito',            N'Edición ilustrada',                                4,   9.50, 60),
(N'OFI-001', N'Cuaderno universitario',   N'Cuaderno de 100 hojas, cuadriculado',              5,   2.25,200),
(N'OFI-002', N'Silla ergonómica',         N'Silla de oficina con soporte lumbar',              5, 120.00,  5),
(N'OFI-003', N'Resma de papel A4',        N'500 hojas, 75 g/m²',                               5,   4.80, 80),
(N'TEC-006', N'Cámara web HD (agotada)',  N'Producto sin stock para pruebas',                  1,  29.00,  0);
GO