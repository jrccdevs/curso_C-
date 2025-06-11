/*base de datos sql*/
CREATE TABLE clientes (
    id_cliente INT PRIMARY KEY,
    nombre VARCHAR(100),
    región VARCHAR(50)
);

CREATE TABLE productos (
    id_producto INT PRIMARY KEY,
    nombre_producto VARCHAR(100),
    categoría VARCHAR(50),
    precio DECIMAL(10,2)
);

CREATE TABLE ventas (
    id_venta INT PRIMARY KEY,
    fecha DATE,
    id_cliente INT,
    id_producto INT,
    cantidad INT,
    total DECIMAL(10,2),
    FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),
    FOREIGN KEY (id_producto) REFERENCES productos(id_producto)
);




/*Listar todas las ventas con nombre del cliente y nombre del producto*/
SELECT v.id_venta, v.fecha, c.nombre AS cliente, p.nombre_producto AS producto, v.cantidad, v.total
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
JOIN productos p ON v.id_producto = p.id_producto;

/*📈 Consultas de análisis
4. Total de ventas por cliente*/
SELECT c.nombre, SUM(v.total) AS total_gastado
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
GROUP BY c.nombre
ORDER BY total_gastado DESC;

/*5. Total de ventas por producto*/
SELECT p.nombre_producto, SUM(v.cantidad) AS unidades_vendidas, SUM(v.total) AS ingresos
FROM ventas v
JOIN productos p ON v.id_producto = p.id_producto
GROUP BY p.nombre_producto
ORDER BY ingresos DESC;

/*6. Ventas por región*/
SELECT c.región, SUM(v.total) AS total_ventas
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
GROUP BY c.región;


/*📅 Consultas temporales
7. Ventas por mes**/
SELECT DATE_FORMAT(fecha, '%Y-%m') AS mes, SUM(total) AS total_mes
FROM ventas
GROUP BY mes
ORDER BY mes;

/*8. Producto más vendido en el último mes*/
SELECT p.nombre_producto, SUM(v.cantidad) AS total_vendido
FROM ventas v
JOIN productos p ON v.id_producto = p.id_producto
WHERE fecha >= DATE_SUB(CURDATE(), INTERVAL 1 MONTH)
GROUP BY p.nombre_producto
ORDER BY total_vendido DESC
LIMIT 1;

/*🧠 Consultas avanzadas
9. Clientes que no han comprado nada*/
SELECT c.*
FROM clientes c
LEFT JOIN ventas v ON c.id_cliente = v.id_cliente
WHERE v.id_cliente IS NULL;

/*10. Producto más rentable (precio x unidades vendidas)*/
SELECT p.nombre_producto, SUM(v.total) AS ingresos_totales
FROM ventas v
JOIN productos p ON v.id_producto = p.id_producto
GROUP BY p.nombre_producto
ORDER BY ingresos_totales DESC
LIMIT 1;

/*Consultas SQL Avanzadas para Prueba Técnica en un Banco
Asumimos que las tablas clientes, productos (servicios financieros como tarjetas, créditos, seguros), y ventas (transacciones o contrataciones) tienen este contexto.

📌 1. Total contratado por cliente (inversiones, créditos, etc.)*/
SELECT c.nombre, SUM(v.total) AS monto_total_contratado
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
GROUP BY c.nombre
ORDER BY monto_total_contratado DESC;

/* 2. Clientes que tienen más de un producto contratado*/
SELECT c.nombre, COUNT(DISTINCT v.id_producto) AS productos_contratados
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
GROUP BY c.nombre
HAVING productos_contratados > 1
ORDER BY productos_contratados DESC;

/*📌 3. Ingresos por tipo de producto financiero (por ejemplo: crédito, tarjeta, seguro)*/
SELECT p.categoría AS tipo_producto, SUM(v.total) AS ingresos
FROM ventas v
JOIN productos p ON v.id_producto = p.id_producto
GROUP BY tipo_producto
ORDER BY ingresos DESC;

/*📌 4. Clientes que contrataron productos de más de una categoría
sql
Copiar
Editar
*/
SELECT c.nombre, COUNT(DISTINCT p.categoría) AS categorias_distintas
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
JOIN productos p ON v.id_producto = p.id_producto
GROUP BY c.nombre
HAVING categorias_distintas > 1;

/*📌 5. Productos más populares por región*/
SELECT c.región, p.nombre_producto, COUNT(*) AS veces_contratado
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
JOIN productos p ON v.id_producto = p.id_producto
GROUP BY c.región, p.nombre_producto
ORDER BY c.región, veces_contratado DESC;

/*📌 6. Clientes VIP (con más de $10,000 en productos contratados)*/
SELECT c.id_cliente, c.nombre, SUM(v.total) AS total
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
GROUP BY c.id_cliente, c.nombre
HAVING total >= 10000;

/*📌 7. Tendencia de ventas de productos financieros por trimestre*/
SELECT CONCAT(YEAR(fecha), '-T', QUARTER(fecha)) AS trimestre, 
       p.categoría, 
       SUM(v.total) AS total_trimestre
FROM ventas v
JOIN productos p ON v.id_producto = p.id_producto
GROUP BY trimestre, p.categoría
ORDER BY trimestre;

/*📌 8. Clientes sin actividad en los últimos 6 meses*/
SELECT c.id_cliente, c.nombre
FROM clientes c
LEFT JOIN ventas v ON c.id_cliente = v.id_cliente AND v.fecha >= DATE_SUB(CURDATE(), INTERVAL 6 MONTH)
WHERE v.id_venta IS NULL;

/*📌 9. Tasa de contratación por región*/
SELECT c.región, COUNT(v.id_venta) AS total_contratos, COUNT(DISTINCT c.id_cliente) AS clientes_únicos
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
GROUP BY c.región;

/*📌 10. Detección de comportamiento inusual: compras > $10,000*/
SELECT v.*, c.nombre, p.nombre_producto
FROM ventas v
JOIN clientes c ON v.id_cliente = c.id_cliente
JOIN productos p ON v.id_producto = p.id_producto
WHERE v.total > 10000;
