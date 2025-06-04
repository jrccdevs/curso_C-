kotlin_task_manager/
├── src/
│   └── main/
│       └── kotlin/
│           ├── Item.kt
│           └── Main.kt
└── build.gradle.kts (si usas Gradle)


// src/main/kotlin/Item.kt

package com.example.taskmanager // Puedes usar el nombre de paquete que quieras

data class Item(val id: Int, var nombre: String, var completado: Boolean = false) {
    // La data class automáticamente genera equals(), hashCode(), toString(), copy(), etc.
    // No necesitamos un constructor explícito si usamos valores por defecto para las propiedades.
}

2. Main.kt(Contiene la lógica principal, similar a un "controlador" que interactúa con la "vista" implícita de la consola)
Este archivo contiene la función mainy todas las funciones que implementan la lógica del programa, interactuando con la lista de Itemsy el usuario.
// src/main/kotlin/Main.kt

package com.example.taskmanager // Mismo nombre de paquete

import java.util.Scanner

// Variables globales para la lista de ítems y el generador de IDs
val scanner = Scanner(System.`in`)
val items = mutableListOf<Item>() // MutableList es el equivalente a List<T> en C#
var nextId = 1

fun main() {
    var running = true
    while (running) {
        mostrarMenu()
        when (val opcion = leerOpcion()) { // 'when' es el equivalente de 'switch' en Kotlin
            1 -> agregarItem()
            2 -> listarItems()
            3 -> marcarComoCompletado()
            4 -> eliminarItem()
            5 -> running = false
            else -> { // Bloque para opciones no válidas
                println("Opción no válida. Por favor, intente de nuevo.")
                pausar()
            }
        }
    }
    println("\nSaliendo de la aplicación. ¡Hasta luego!")
    scanner.close() // Es buena práctica cerrar el scanner cuando ya no se necesita
}

/**
 * Muestra el menú principal de la aplicación.
 */
fun mostrarMenu() {
    print("\u001b[H\u001b[2J") // Limpiar la consola (código ANSI, puede no funcionar en todas las terminales)
    System.out.flush() // Asegurarse de que el buffer se vacíe

    println("--- GESTIÓN DE ITEMS ---")
    println("1. Agregar Item")
    println("2. Listar Items")
    println("3. Marcar Item como Completado")
    println("4. Eliminar Item")
    println("5. Salir")
    print("Seleccione una opción: ")
}

/**
 * Lee la opción del menú ingresada por el usuario, validando que sea un número entero.
 * @return La opción seleccionada o -1 si es una entrada inválida.
 */
fun leerOpcion(): Int {
    return try {
        scanner.nextLine().toInt()
    } catch (e: NumberFormatException) {
        -1 // Retorna un valor que indicará una opción inválida
    }
}

/**
 * Agrega un nuevo item a la lista.
 */
fun agregarItem() {
    print("Ingrese el nombre del item: ")
    val nombre = scanner.nextLine().trim() // .trim() elimina espacios en blanco al inicio y final

    if (nombre.isNotBlank()) { // isNotBlank() es una función útil para cadenas no vacías y con contenido
        val newItem = Item(nextId++, nombre)
        items.add(newItem)
        println("Item '$nombre' agregado con éxito. ID: ${newItem.id}")
    } else {
        println("El nombre del item no puede estar vacío.")
    }
    pausar()
}

/**
 * Lista todos los items en la colección.
 */
fun listarItems() {
    print("\u001b[H\u001b[2J") // Limpiar la consola
    System.out.flush()

    if (items.isEmpty()) { // isEmpty() es una función de extensión para colecciones
        println("No hay ítems para mostrar.")
        pausar()
        return
    }

    println("--- LISTA DE ITEMS ---")
    println("ID\tEstado\tNombre")
    println("----------------------------------")
    items.forEach { item -> // forEach es una función de orden superior para iterar
        val estado = if (item.completado) "Completado" else "Pendiente "
        println("${item.id}\t$estado\t${item.nombre}")
    }
    println("----------------------------------")
    pausar()
}

/**
 * Solicita un ID y marca el item correspondiente como completado.
 */
fun marcarComoCompletado() {
    print("Ingrese el ID del item a marcar como completado: ")
    val id = leerNumero()

    val itemToComplete = items.find { it.id == id } // find es una función de extensión para buscar
    if (itemToComplete != null) { // Null safety en acción: itemToComplete puede ser null
        itemToComplete.completado = true
        println("Item con ID $id marcado como completado: '${itemToComplete.nombre}'.")
    } else {
        println("No se encontró un item con ID $id.")
    }
    pausar()
}

/**
 * Solicita un ID y elimina el item correspondiente.
 */
fun eliminarItem() {
    print("Ingrese el ID del item a eliminar: ")
    val id = leerNumero()

    val removed = items.removeIf { it.id == id } // removeIf es una función para eliminar condicionalmente
    if (removed) {
        println("Item con ID $id eliminado con éxito.")
    } else {
        println("No se encontró un item con ID $id.")
    }
    pausar()
}

/**
 * Función auxiliar para leer un número entero desde la entrada del usuario.
 * @return El número entero o -1 si la entrada es inválida.
 */
fun leerNumero(): Int {
    return try {
        scanner.nextLine().toInt()
    } catch (e: NumberFormatException) {
        println("Entrada inválida. Por favor, ingrese un número entero.")
        -1
    }
}

/**
 * Pausa la ejecución hasta que el usuario presione Enter.
 */
fun pausar() {
    println("\nPresione Enter para continuar...")
    scanner.nextLine() // Consumir la línea pendiente
}

Cómo ejecutarlo:
En IntelliJ IDEA:

Abra IntelliJ IDEA.
Haga clic en "Nuevo Proyecto".
Selecciona "Kotlin" en el panel izquierdo.
Elige "JVM | Aplicación de consola" como plantilla.
Haga clic en "Siguiente".
Dale un nombre a tu proyecto (ej., "KotlinTaskManager").
Haga clic en "Finalizar".
Dentro de la carpeta src/main/kotlin, verás un archivo Main.kt. Crea un nuevo archivo Item.ktal mismo nivel.
Copia el código de Item.kten su respectivo archivo.
Copia el código de Main.kten su respectivo archivo.
Asegúrese de que el package com.example.taskmanager(o el que elijas) sea consistente en ambos archivos.
Haga clic derecho en el archivo Main.kty seleccione "Run 'MainKt'".
En Visual Studio Code (con las extensiones y compilador de Kotlin instalados):

Crea una nueva carpeta para tu proyecto.
Dentro de esa carpeta, crea los archivos Item.kty Main.kt.
Pegue el código en sus respectivos archivos.
Asegúrese de que el packagedeclarado al inicio de ambos archivos sea el mismo (ej., package com.example.taskmanager).
Abre la carpeta en VS Code.
Abre el archivo Main.kt.
Haz clic derecho en el editor y selecciona "Run Code" (si tienes la extensión Code Runner) o abre la terminal (Ctrl+Shift+`) y ejecuta el compilador y JVM manualmente:
kotlinc Item.kt Main.kt -include-runtime -d taskmanager.jar(esto compila los archivos Kotlin y los empaqueta en un JAR ejecutable)
java -jar taskmanager.jar(esto ejecuta el JAR)
Respuestas a las Preguntas Teóricas Cortas de Kotlin:
¿Cuál es la principal ventaja de Kotlin sobre Java para el desarrollo de Android (o desarrollo en general)? Mencione al menos dos puntos.

Concisión (Less Boilerplate): Kotlin es mucho más conciso que Java, lo que significa escribir menos código para lograr lo mismo. Por ejemplo, las data classeliminan la necesidad de escribir manualmente equals(), hashCode(), toString(), etc.
Null Safety: Kotlin tiene un sistema de tipos nulos integrado que ayuda a eliminar la causa de la mayoría de las NullPointerExceptionen tiempo de compilación, lo que lleva un código más robusto y menos errores en tiempo de ejecución.
Interoperabilidad con Java: Kotlin es 100% interoperable con Java, lo que significa que puedes usar código Java en proyectos Kotlin y viceversa. Esto facilita la migración gradual y el uso de librerías Java existentes.
Funciones de extensión: Permiten agregar nuevas funcionalidades a clases existentes sin modificarlas o usar herencia, lo que mejora la legibilidad y modularidad del código.
Explica qué es la seguridad nula en Kotlin y cómo te ayuda a evitarlo NullPointerExceptions.

El null safety en Kotlin es un sistema de tipos que distingue entre tipos que pueden contener un valor null(tipos nullable , marcados con ?como String?) y tipos que no pueden ( String).
Esto obliga al desarrollador a manejar explícitamente los casos donde un valor podría ser nulo, generalmente usando operadores como ?.(llamada segura), ?:(operador Elvis) o bloques if (x != null).
Al forzar este manejo en tiempo de compilación, Kotlin reduce excesivamente las NullPointerExceptionen tiempo de ejecución, ya que el compilador no permite invocar métodos o propiedades en un objeto que potencialmente podría ser nulo sin una comprobación adecuada.
¿Qué es una data classen Kotlin y por qué es útil?

Una data classes una clase especial en Kotlin diseñada específicamente para contener datos. El compilador de Kotlin genera automáticamente varias funciones útiles para estas clases, como:
equals(): Para comparar la igualdad de dos objetos basándose en sus propiedades.
hashCode(): Para usar objetos en colecciones basadas en hash (como HashSeto HashMap).
toString(): Una representación de cadena legible del objeto.
copy(): Para crear copias de un objeto, posiblemente modificando algunas propiedades.
componentN():Para la declaración de desestructuración .
Es útil porque reducir la cantidad de código repetitivo que se necesitaría escribir manualmente en Java o en una clase regular de Kotlin para lograr la misma funcionalidad en objetos de datos.
¿Cuál es la diferencia entre valy varal declarar variables en Kotlin?

val(valor): Se utiliza para declarar variables de solo lectura (inmutables) . Una vez que se le asigna un valor, no se puede reasignar. Es similar a la palabra clave finalen Java o readonlyen C#. Fomenta el uso de programación funcional y la inmutabilidad.
var(variable): Se utiliza para declarar variables mutables . Su valor puede ser reasignado en cualquier momento después de su declaración. Es similar a la declaración de una variable normal en Java o C#.
¿Qué son las funciones de extensión en Kotlin? Da un ejemplo simple de cuándo podrías usar una.

Las funciones de extensión (funciones de extensión) permiten "extender" una clase con nuevas funcionalidades sin tener que heredar de la clase o usar patrones de diseño como el Decorator . Simplemente añades una función a una clase existente, como si fuera parte de ella, aunque el código de la función esté definido fuera de esa clase.
Ejemplo: Podrías crear una función de extensión para la clase Stringque ponga en mayúscula la primera letra de cada palabra:

fun String.capitalizeWords(): String {
    return split(" ").joinToString(" ") { it.capitalize() }
}

// Uso:
val frase = "hola mundo kotlin"
println(frase.capitalizeWords()) // Salida: "Hola Mundo Kotlin"