proyecto_banco/
│
├── 📄 config.py              # Configuración de conexión a la base de datos
├── 📄 init_db.py             # Script para crear tablas e insertar datos
├── 📄 consultas.py           # Todas las consultas SQL que necesitas ejecutar
├── 📄 main.py                # Script principal que llama a las funciones
└── 📄 requirements.txt       # Dependencias del proyecto

#config.py
import pymysql

def get_connection():
    return pymysql.connect(
        host='localhost',
        user='tu_usuario',
        password='tu_contraseña',
        database='nombre_de_tu_base',
        cursorclass=pymysql.cursors.Cursor
    )

#init_db.py
from config import get_connection

def crear_tablas_y_datos():
    conn = get_connection()
    cursor = conn.cursor()

    # Crear tablas
    cursor.execute("""CREATE TABLE IF NOT EXISTS clientes (
        id_cliente INT PRIMARY KEY,
        nombre VARCHAR(100),
        región VARCHAR(100)
    );""")

    cursor.execute("""CREATE TABLE IF NOT EXISTS productos (
        id_producto INT PRIMARY KEY,
        nombre_producto VARCHAR(100),
        categoría VARCHAR(50),
        precio DECIMAL(10,2)
    );""")

    cursor.execute("""CREATE TABLE IF NOT EXISTS ventas (
        id_venta INT PRIMARY KEY,
        fecha DATE,
        id_cliente INT,
        id_producto INT,
        cantidad INT,
        total DECIMAL(10,2),
        FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),
        FOREIGN KEY (id_producto) REFERENCES productos(id_producto)
    );""")

    # Insertar datos
    clientes = [
        (1, 'Juan Pérez', 'La Paz'),
        (2, 'María García', 'Santa Cruz'),
        (3, 'Carlos Mendoza', 'Cochabamba'),
        (4, 'Ana López', 'La Paz'),
        (5, 'Luis Rodríguez', 'Tarija')
    ]

    productos = [
        (1, 'Crédito Personal', 'Crédito', 5000.00),
        (2, 'Tarjeta de Crédito Oro', 'Tarjeta', 150.00),
        (3, 'Seguro de Vida', 'Seguro', 250.00),
        (4, 'Cuenta de Ahorros Plus', 'Cuenta', 0.00),
        (5, 'Crédito Hipotecario', 'Crédito', 20000.00),
        (6, 'Tarjeta de Débito', 'Tarjeta', 0.00)
    ]

    ventas = [
        (1, '2024-01-15', 1, 1, 1, 5000.00),
        (2, '2024-01-20', 2, 2, 1, 150.00),
        (3, '2024-02-10', 3, 3, 1, 250.00),
        (4, '2024-02-15', 1, 4, 1, 0.00),
        (5, '2024-03-01', 2, 5, 1, 20000.00),
        (6, '2024-03-10', 4, 1, 1, 5000.00),
        (7, '2024-04-01', 5, 2, 1, 150.00),
        (8, '2024-04-15', 3, 6, 1, 0.00),
        (9, '2024-05-01', 4, 3, 1, 250.00),
        (10, '2024-05-10', 5, 5, 1, 20000.00)
    ]

    cursor.executemany("INSERT IGNORE INTO clientes VALUES (%s, %s, %s)", clientes)
    cursor.executemany("INSERT IGNORE INTO productos VALUES (%s, %s, %s, %s)", productos)
    cursor.executemany("INSERT IGNORE INTO ventas VALUES (%s, %s, %s, %s, %s, %s)", ventas)

    conn.commit()
    conn.close()
    print("Base de datos inicializada correctamente.")

#consultas.py
from config import get_connection

def consulta_monto_total_por_cliente():
    conn = get_connection()
    cursor = conn.cursor()

    query = """
    SELECT c.nombre, SUM(v.total) AS monto_total
    FROM ventas v
    JOIN clientes c ON v.id_cliente = c.id_cliente
    GROUP BY c.nombre
    ORDER BY monto_total DESC;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()

    return resultados

#✅ 1. Monto total vendido por producto
def consulta_monto_total_por_producto():
    conn = get_connection()
    cursor = conn.cursor()

    query = """
    SELECT p.nombre_producto, SUM(v.total) AS total_vendido
    FROM ventas v
    JOIN productos p ON v.id_producto = p.id_producto
    GROUP BY p.nombre_producto
    ORDER BY total_vendido DESC;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()
    return resultados

#✅ 2. Total de ventas por región
def consulta_total_ventas_por_region():
    conn = get_connection()
    cursor = conn.cursor()

    query = """
    SELECT c.región, SUM(v.total) AS total_region
    FROM ventas v
    JOIN clientes c ON v.id_cliente = c.id_cliente
    GROUP BY c.región
    ORDER BY total_region DESC;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()
    return resultados

#✅ 3. Clientes que contrataron más de un producto
def clientes_con_multiples_productos():
    conn = get_connection()
    cursor = conn.cursor()

    query = """
    SELECT c.nombre, COUNT(DISTINCT v.id_producto) AS productos_contratados
    FROM ventas v
    JOIN clientes c ON v.id_cliente = c.id_cliente
    GROUP BY c.id_cliente
    HAVING productos_contratados > 1
    ORDER BY productos_contratados DESC;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()
    return resultados

#✅ 4. Productos que no fueron contratados
def productos_no_contratados():
    conn = get_connection()
    cursor = conn.cursor()

    query = """
    SELECT p.nombre_producto
    FROM productos p
    LEFT JOIN ventas v ON p.id_producto = v.id_producto
    WHERE v.id_producto IS NULL;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()
    return resultados

#✅ 5. Ranking de clientes por monto total contratado
def ranking_clientes():
    conn = get_connection()
    cursor = conn.cursor()

    query = """
    SELECT c.nombre, SUM(v.total) AS total_contratado
    FROM ventas v
    JOIN clientes c ON v.id_cliente = c.id_cliente
    GROUP BY c.nombre
    ORDER BY total_contratado DESC;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()
    return resultados

#✅ 6. Ventas por mes (año dinámico)
def ventas_por_mes(anio=2024):
    conn = get_connection()
    cursor = conn.cursor()

    query = f"""
    SELECT MONTH(fecha) AS mes, SUM(total) AS total_mes
    FROM ventas
    WHERE YEAR(fecha) = {anio}
    GROUP BY MONTH(fecha)
    ORDER BY mes;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    conn.close()
    return resultados

#¿Cómo usarlos en main.py?
from consultas import (
    consulta_monto_total_por_producto,
    consulta_total_ventas_por_region,
    clientes_con_multiples_productos,
    productos_no_contratados,
    ranking_clientes,
    ventas_por_mes
)

# Ejecutar cada consulta e imprimir resultados
print("Ranking de productos vendidos:")
for nombre, total in consulta_monto_total_por_producto():
    print(f"{nombre}: Bs {total:,.2f}")

print("\nVentas por región:")
for region, total in consulta_total_ventas_por_region():
    print(f"{region}: Bs {total:,.2f}")

print("\nClientes con múltiples productos:")
for nombre, count in clientes_con_multiples_productos():
    print(f"{nombre} contrató {count} productos")

print("\nProductos no contratados:")
for (producto,) in productos_no_contratados():
    print(f"- {producto}")

print("\nRanking de clientes por monto contratado:")
for nombre, total in ranking_clientes():
    print(f"{nombre}: Bs {total:,.2f}")

print("\nVentas por mes en 2024:")
for mes, total in ventas_por_mes():
    print(f"Mes {mes}: Bs {total:,.2f}")


#requirements.txt
pymysql
