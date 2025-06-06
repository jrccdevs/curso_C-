/*3. Frontend: Flutter (Dart)
Vamos a construir una interfaz de usuario simple para listar y ver/editar empleados.

3.1. Dependencias de Flutter
Abre el archivo pubspec.yaml en tu proyecto Flutter y añade las siguientes dependencias:*/

dependencies:
  flutter:
    sdk: flutter
  http: ^1.2.1 # Para hacer peticiones HTTP
  # Para deserializar JSON (opcional, pero recomendado para grandes proyectos)
  # json_annotation: ^4.8.1

dev_dependencies:
  flutter_test:
    sdk: flutter
  flutter_lints: ^3.0.0
  # build_runner: ^2.4.8 # Si usas json_serializable
  # json_serializable: ^6.7.1 # Si usas json_serializable

  /*Después de añadir las dependencias, guarda el archivo y ejecuta flutter pub get en tu terminal.

3.2. El Modelo (Dart)
Define la estructura de tus datos de empleado que coincida con el backend.*/
#lib/models/empleado.dart
// Si usas json_serializable, esto sería un poco diferente
class Empleado {
  final int? id; // Nullable para cuando se crea uno nuevo
  final String nombre;
  final String apellido;
  final String cargo;
  final double salario; // Usamos double en Dart para decimales

  Empleado({
    this.id,
    required this.nombre,
    required this.apellido,
    required this.cargo,
    required this.salario,
  });

  // Constructor de fábrica para crear un Empleado desde un JSON
  factory Empleado.fromJson(Map<String, dynamic> json) {
    return Empleado(
      id: json['id'] as int?,
      nombre: json['nombre'] as String,
      apellido: json['apellido'] as String,
      cargo: json['cargo'] as String,
      salario: (json['salario'] as num).toDouble(), // JSON puede devolver int o double
    );
  }

  // Método para convertir un Empleado a JSON para enviarlo al backend
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'nombre': nombre,
      'apellido': apellido,
      'cargo': cargo,
      'salario': salario,
    };
  }
}

/*3.3. El Servicio (Flutter)
Este es el cliente HTTP que interactuará con tu API backend.

lib/services/empleado_service.dart*/
import 'dart:convert'; // Para json.decode y json.encode
import 'package:http/http.dart' as http; // Importa el paquete http
import 'package:empleados_frontend/models/empleado.dart'; // Importa tu modelo Empleado

class EmpleadoService {
  // Asegúrate de que esta URL coincida con la de tu backend ASP.NET Core
  // Si ejecutas en un emulador de Android, '10.0.2.2' apunta a 'localhost' de tu máquina de desarrollo.
  // Si usas un dispositivo físico o iOS, usa la IP real de tu máquina (ej. 'http://192.168.1.XX:70XX').
  // ¡Importante!: Usa http:// si tu backend no está configurado para HTTPS en el puerto 70XX.
  // Si usas https (recomendado en desarrollo), asegúrate de que el emulador confíe en el certificado auto-firmado,
  // o cambia a http para desarrollo si tienes problemas.
  final String _baseUrl = 'http://10.0.2.2:5000/api/empleados'; // Ejemplo con http en puerto 5000

  // Si tu backend usa HTTPS en Visual Studio por defecto, probablemente será como:
  // final String _baseUrl = 'https://10.0.2.2:70XX/api/empleados';
  // En ese caso, para desarrollo con emuladores, podrías necesitar una configuración especial
  // para aceptar certificados no válidos o, más simple, cambiar el backend para escuchar HTTP en desarrollo.

  Future<List<Empleado>> getEmpleados() async {
    final response = await http.get(Uri.parse(_baseUrl));

    if (response.statusCode == 200) {
      List jsonResponse = json.decode(response.body);
      return jsonResponse.map((empleado) => Empleado.fromJson(empleado)).toList();
    } else {
      throw Exception('Failed to load empleados: ${response.statusCode}');
    }
  }

  Future<Empleado> getEmpleadoById(int id) async {
    final response = await http.get(Uri.parse('$_baseUrl/$id'));

    if (response.statusCode == 200) {
      return Empleado.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to load empleado: ${response.statusCode}');
    }
  }

  Future<Empleado> addEmpleado(Empleado empleado) async {
    final response = await http.post(
      Uri.parse(_baseUrl),
      headers: <String, String>{
        'Content-Type': 'application/json; charset=UTF-8',
      },
      body: jsonEncode(empleado.toJson()),
    );

    if (response.statusCode == 201) { // 201 Created
      return Empleado.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to add empleado: ${response.statusCode} - ${response.body}');
    }
  }

  Future<bool> updateEmpleado(Empleado empleado) async {
    final response = await http.put(
      Uri.parse('$_baseUrl/${empleado.id}'),
      headers: <String, String>{
        'Content-Type': 'application/json; charset=UTF-8',
      },
      body: jsonEncode(empleado.toJson()),
    );

    if (response.statusCode == 204) { // 204 No Content
      return true;
    } else {
      throw Exception('Failed to update empleado: ${response.statusCode} - ${response.body}');
    }
  }

  Future<bool> deleteEmpleado(int id) async {
    final response = await http.delete(Uri.parse('$_baseUrl/$id'));

    if (response.statusCode == 204) { // 204 No Content
      return true;
    } else {
      throw Exception('Failed to delete empleado: ${response.statusCode}');
    }
  }
}

/*3.4. La Vista (Flutter UI)
Aquí construiremos la interfaz de usuario para mostrar la lista de empleados y permitir agregar/editar.

lib/main.dart*/

import 'package:flutter/material.dart';
import 'package:empleados_frontend/models/empleado.dart';
import 'package:empleados_frontend/services/empleado_service.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Gestión de Empleados',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: EmpleadoListPage(),
    );
  }
}

class EmpleadoListPage extends StatefulWidget {
  @override
  _EmpleadoListPageState createState() => _EmpleadoListPageState();
}

class _EmpleadoListPageState extends State<EmpleadoListPage> {
  final EmpleadoService _empleadoService = EmpleadoService();
  late Future<List<Empleado>> _empleadosFuture;

  @override
  void initState() {
    super.initState();
    _empleadosFuture = _empleadoService.getEmpleados();
  }

  void _refreshEmpleados() {
    setState(() {
      _empleadosFuture = _empleadoService.getEmpleados();
    });
  }

  void _navigateToEmpleadoDetail({Empleado? empleado}) async {
    await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => EmpleadoDetailPage(empleado: empleado),
      ),
    );
    _refreshEmpleados(); // Refrescar la lista al volver de la pantalla de detalle
  }

  void _deleteEmpleado(int id) async {
    bool? confirmDelete = await showDialog<bool>(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Confirmar Eliminación'),
          content: const Text('¿Estás seguro de que quieres eliminar este empleado?'),
          actions: <Widget>[
            TextButton(
              onPressed: () => Navigator.of(context).pop(false),
              child: const Text('Cancelar'),
            ),
            TextButton(
              onPressed: () => Navigator.of(context).pop(true),
              child: const Text('Eliminar'),
            ),
          ],
        );
      },
    );

    if (confirmDelete == true) {
      try {
        await _empleadoService.deleteEmpleado(id);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Empleado eliminado con éxito')),
        );
        _refreshEmpleados();
      } catch (e) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error al eliminar empleado: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Lista de Empleados'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _refreshEmpleados,
          ),
        ],
      ),
      body: FutureBuilder<List<Empleado>>(
        future: _empleadosFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text('No hay empleados registrados.'));
          } else {
            return ListView.builder(
              itemCount: snapshot.data!.length,
              itemBuilder: (context, index) {
                final empleado = snapshot.data![index];
                return Card(
                  margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  child: ListTile(
                    title: Text('${empleado.nombre} ${empleado.apellido}'),
                    subtitle: Text('${empleado.cargo} - \$${empleado.salario.toStringAsFixed(2)}'),
                    onTap: () => _navigateToEmpleadoDetail(empleado: empleado),
                    trailing: IconButton(
                      icon: const Icon(Icons.delete, color: Colors.red),
                      onPressed: () => _deleteEmpleado(empleado.id!),
                    ),
                  ),
                );
              },
            );
          }
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _navigateToEmpleadoDetail(), // Navegar para añadir uno nuevo
        child: const Icon(Icons.add),
      ),
    );
  }
}

// Pantalla para añadir o editar un empleado
class EmpleadoDetailPage extends StatefulWidget {
  final Empleado? empleado; // Nullable para añadir nuevo

  const EmpleadoDetailPage({super.key, this.empleado});

  @override
  _EmpleadoDetailPageState createState() => _EmpleadoDetailPageState();
}

class _EmpleadoDetailPageState extends State<EmpleadoDetailPage> {
  final EmpleadoService _empleadoService = EmpleadoService();
  final _formKey = GlobalKey<FormState>(); // Clave para el formulario
  late TextEditingController _nombreController;
  late TextEditingController _apellidoController;
  late TextEditingController _cargoController;
  late TextEditingController _salarioController;

  @override
  void initState() {
    super.initState();
    _nombreController = TextEditingController(text: widget.empleado?.nombre ?? '');
    _apellidoController = TextEditingController(text: widget.empleado?.apellido ?? '');
    _cargoController = TextEditingController(text: widget.empleado?.cargo ?? '');
    _salarioController = TextEditingController(text: widget.empleado?.salario.toStringAsFixed(2) ?? '');
  }

  @override
  void dispose() {
    _nombreController.dispose();
    _apellidoController.dispose();
    _cargoController.dispose();
    _salarioController.dispose();
    super.dispose();
  }

  void _saveEmpleado() async {
    if (_formKey.currentState!.validate()) {
      final empleado = Empleado(
        id: widget.empleado?.id, // Mantiene el ID si es una actualización
        nombre: _nombreController.text,
        apellido: _apellidoController.text,
        cargo: _cargoController.text,
        salario: double.parse(_salarioController.text),
      );

      try {
        if (empleado.id == null) {
          await _empleadoService.addEmpleado(empleado);
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Empleado añadido con éxito')),
          );
        } else {
          await _empleadoService.updateEmpleado(empleado);
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Empleado actualizado con éxito')),
          );
        }
        Navigator.pop(context); // Volver a la lista
      } catch (e) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error al guardar empleado: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.empleado == null ? 'Añadir Empleado' : 'Editar Empleado'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              TextFormField(
                controller: _nombreController,
                decoration: const InputDecoration(labelText: 'Nombre'),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor ingresa el nombre';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _apellidoController,
                decoration: const InputDecoration(labelText: 'Apellido'),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor ingresa el apellido';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _cargoController,
                decoration: const InputDecoration(labelText: 'Cargo'),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor ingresa el cargo';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _salarioController,
                decoration: const InputDecoration(labelText: 'Salario'),
                keyboardType: TextInputType.number,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor ingresa el salario';
                  }
                  if (double.tryParse(value) == null) {
                    return 'Salario inválido';
                  }
                  if (double.parse(value) < 0) {
                    return 'El salario no puede ser negativo';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _saveEmpleado,
                child: Text(widget.empleado == null ? 'Añadir Empleado' : 'Guardar Cambios'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/*4. Ejecución de la Aplicación Completa
Iniciar el Backend (C#):

En Visual Studio 2022, abre tu proyecto EmpleadosApiBackend.

Presiona F5 para iniciar el servidor. Anota la URL (ej. https://localhost:70xx o http://localhost:50xx). Es crucial que el puerto que uses en tu _baseUrl de Flutter coincida con el puerto en el que se ejecuta tu backend.

Nota para HTTPS/HTTP: Si tu backend se inicia con HTTPS (https://localhost:70xx), y tienes problemas con el emulador de Android/iOS o dispositivo real, una solución rápida para el desarrollo es cambiar tu backend para que escuche también HTTP. Puedes modificar launchSettings.json en la carpeta Properties de tu proyecto backend para usar http en un puerto específico (ej. 5000) o directamente en Program.cs para deshabilitar la redirección HTTPS temporalmente si es solo para pruebas. Para el emulador de Android, 10.0.2.2 mapea a localhost. Para iOS o dispositivos físicos, necesitarás la IP de tu máquina de desarrollo.

Iniciar el Frontend (Flutter):

Abre la terminal en la carpeta raíz de tu proyecto empleados_frontend.
Asegúrate de que el _baseUrl en lib/services/empleado_service.dart coincida con la URL y el puerto donde se ejecuta tu backend ASP.NET Core (usando 10.0.2.2 si usas un emulador de Android).
Ejecuta: flutter run
Selecciona un dispositivo (emulador de Android, simulador de iOS o navegador web) para ejecutar la aplicación.
Ahora tendrás una aplicación Flutter en el frontend que se comunica con una API RESTful construida con ASP.NET Core MVC en el backend. Podrás ver la lista de empleados, añadir nuevos, editar los existentes y eliminarlos, y todos los cambios se reflejarán a través de la comunicación entre Flutter y tu API de C#.*/