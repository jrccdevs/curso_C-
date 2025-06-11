mi_proyecto_powerbi_python/
│
├── config/
│   └── db_config.py          # Configuración de la base de datos (host, usuario, password, etc.)
│
├── queries/
│   └── ventas_queries.py     # Archivo con funciones SQL o funciones que devuelven DataFrames
│
├── charts/
│   └── ventas_charts.py      # Gráficos (usando matplotlib, seaborn, etc.)
│
├── main.py                   # Script principal que ejecuta todo
│
├── requirements.txt          # Dependencias del proyecto
└── README.md                 # Instrucciones del proyecto

#config/db_config.py
import mysql.connector

def get_connection():
    return mysql.connector.connect(
        host="localhost",
        user="tu_usuario",
        password="tu_contraseña",
        database="nombre_base"
    )


#queries/ventas_queries.py
import pandas as pd
from config.db_config import get_connection

def ventas_por_producto():
    conn = get_connection()
    query = """
        SELECT p.nombre_producto, SUM(v.total) AS total_vendido
        FROM ventas v
        JOIN productos p ON v.id_producto = p.id_producto
        GROUP BY p.nombre_producto
    """
    df = pd.read_sql(query, conn)
    conn.close()
    return df

#charts/ventas_charts.py
import matplotlib.pyplot as plt
import seaborn as sns

def graficar_ventas(df):
    plt.figure(figsize=(10, 6))
    sns.barplot(x='total_vendido', y='nombre_producto', data=df, palette='mako')
    plt.title('Total Vendido por Producto')
    plt.xlabel('Total Vendido')
    plt.ylabel('Producto')
    plt.tight_layout()
    plt.show()

#main.py
from queries.ventas_queries import ventas_por_producto
from charts.ventas_charts import graficar_ventas

def main():
    df = ventas_por_producto()
    print(df.head())
    graficar_ventas(df)

if __name__ == "__main__":
    main()

#requirements.txt
mysql-connector-python
pandas
matplotlib
seaborn

#Puedes instalar las dependencias con:
pip install -r requirements.txt

pymysql o mysql.connector: para conectarnos a la base de datos MySQL.

pandas: para manejar los datos como un DataFrame.

matplotlib o seaborn: para graficar.