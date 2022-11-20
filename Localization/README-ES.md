# EXILED - EXtended In-runtime Library for External Development

![EXILED CI](https://github.com/galaxy119/EXILED/workflows/EXILED%20CI/badge.svg?branch=2.0.0)
<a href="https://github.com/Exiled-Team/EXILED/releases">
  <img src="https://img.shields.io/github/release/Exiled-Team/EXILED/all.svg?style=flat" alt="GitHub Releases">
</a>
![Github All Downloads](https://img.shields.io/github/downloads/galaxy119/EXILED/total.svg?style=flat)
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev)
<a href="https://discord.gg/PyUkWTg">
  <img src="https://img.shields.io/discord/656673194693885975?logo=discord" alt="Chat on Discord">
</a>


EXILED es una plataforma de desarrollo de plugins para servidores de SCP: Secret Laboratory. Ofrece un sistema de eventos para desarrolladores y poder modificar o cambiar código del juego, o implementar sus propias funciones.
Todos los eventos de EXILED están hechos con Harmony, lo que significa que no es necesario editar el código del juego/servidor para funcionar, esto ofrece dos beneficios únicos.

 - Primeramente, toda la plataforma de desarrollo se puede publicar y distribuir, lo cual permite que los desarrolladores entiendan mejor *como funciona*, al igual que poder sugerir ideas o cambiar algo.
 - Segundo, ya que todo el código relacionado con EXILED no está integrado en el código del servidor, cosas como actualizaciones pequeñas tendrán un pequeño (si acaso) efecto. Esto permite que siga siendo compatible con actualizaciones futuras, también agiliza el proceso de actualización cuando de verdad *es* necesario.

# READMEs Traducidas
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [English](https://github.com/Exiled-Team/EXILED/blob/master/README.md)

# Instalación
La instalación de EXILED puede parecer muy complicada al contrario de otras plataformas, pero en realidad es bastante simple. Como se menciona previamente, la mayoría de EXILED no está dentro del archivo "Assembly-CSharp.dll" del servidor, sin embargo, si hay una única y pequeña modificación a este archivo, la cual se requiere para poder *iniciar* EXILED junto al servidor, una versión del archivo con esta modificación se incluye junto al instalador.

Si eliges el instalador automático (sí se ejecuta correctamente) se encargará de instalar `Exiled.Loader`, `Exiled.Updater`, `Exiled.Permissions`, `Exiled.API` y `Exiled.Events`, al igual que se asegurará de que tu servidor tenga el archivo Assembly-CSharp.dll instalado.

# Windows
### Instalación automática ([más información](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))
**Nota:** Asegurate de que estás en el usuario que ejecuta el servidor o tiene privilegios de administrador antes de iniciar el Instalador.

  - Descarga **`Exiled.Installer-Win.exe` [de aquí](https://github.com/galaxy119/EXILED/releases)** (selecciona Assets -> descarga el Instalador).
  - Colócalo en tu carpeta del servidor (descarga el servidor dedicado si no lo has hecho todavía).
  - Haz doble-clic en **`Exiled.Installer.exe`** o descarga **[este archivo .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** y colócalo en la carpeta del servidor para descargar la última versión de prueba.
  - Para conseguir e instalar plugins, mira el apartado [Instalando plugins](#installing-plugins) más abajo.
**Nota:** Si estás instalando EXILED en un servidor remoto, asegúrate de iniciar el .exe con el mismo usuario que tiene el servidor de SCP:SL (o uno con permisos de Administrador)

### Instalación manual
  - Descarga **`Exiled.tar.gz` [de aquí](https://github.com/galaxy119/EXILED/releases)**
  - Extrae el contenido con [7Zip](https://www.7-zip.org/) o [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Mueve el archivo **``Assembly-CSharp.dll``** a: **`(Carpeta de tu servidor)\SCPSL_Data\Managed`** y reemplaza el archivo.
  - Mueve la carpeta **``EXILED``** a **`%appdata%`** *Nota: ¡Esta carpeta tiene que ir en ``C:\Users\(Usuario)\AppData\Roaming``, y  ***NO*** en ``C:\Users\(Usuario)\AppData\Roaming\SCP Secret Laboratory`` y **DEBE ESTAR** en (...)\AppData\Roaming, no en (...)\AppData\!*
    - Windows 10/11:
      Escribe `%appdata%` en Cortana / la barra de buscar, o en la barra del Explorador de Archivos
    - Cualquier otra versión:
      Presiona Win + R y escribe `%appdata%`

### Instalando plugins
¡Ya estaría! EXILED debería estar instalado y activo la próxima vez que inicies el servidor. Recuerda que EXILED por si solo no hace absolutamente nada, así que asegúrate de que instalas plugins desde **[nuestro servidor de Discord](https://discord.gg/PyUkWTg)**
- Para instalar un plugin simplemente:
  - Descarga un plugin de [*su* página de versiones](https://i.imgur.com/u34wgPD.jpg) (**¡DEBE ser un `.dll`!**)
  - Colócalo en: ``C:\Users\(Usuario)\AppData\Roaming\EXILED\Plugins`` (muévete a esta carpeta presionando Win + R, después escribiendo `%appdata%`)

# Linux
### Instalación automática ([más información](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))

**Nota:** Si estás instalando EXILED en un servidor remoto, asegúrate de que inicias el Instalador con el mismo usuario que ejecuta el servidor de SCP: SL (o con root)

  - Descarga **`Exiled.Installer-Linux` [de aquí](https://github.com/galaxy119/EXILED/releases)** (selecciona Assets -> descarga el Instalador).
  - Se instala escribiendo **`./Exiled.Installer-Linux --path /carpeta/del/servidor`** o ponlo en la carpeta del servidor directamente, muévete a la carpeta desde la terminal (`cd`) y escribe: **`./Exiled.Installer-Linux`**.
  - Si quieres la versión de prueba más reciente, añade **`--pre-releases`** al final del comando. Ejemplos: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**.
  - Otro ejemplo si pusiste `Exiled.Installer-Linux` en la carpeta del servidor: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**.
  - Para instalar y conseguir plugins, mira el apartado [Instalando plugins](#installing-plugins-1) más abajo.

### Instalación manual
  - **Asegurate** de que estás conectado con el usuario que tiene los servidores de SCP:SL.
  - Descarga **`Exiled.tar.gz` [de aquí](https://github.com/galaxy119/EXILED/releases)** (SSH: clic derecho para conseguir el link `Exiled.tar.gz`, después escribe: **`wget (enlace_de_descarga)`**)
  - Para extraerlo, escribe **``tar -xzvf EXILED.tar.gz``** en la terminal.
  - Mueve el archivo **``Assembly-CSharp.dll``** incluido a la carpeta **``SCPSL_Data/Managed``** de tu instalación del servidor (SSH: **`mv Assembly-CSharp.dll (instalación_del_servidor)/SCPSL_Data/Managed`**).
  - Mueve la carpeta **`EXILED`** a **``~/.config``**. *Nota: Esta carpeta tiene que ir en ``~/.config``, y ***NO*** en ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)

### Instalando plugins
¡Ya estaría! EXILED debería estar instalado y activo la próxima vez que inicies el servidor. Recuerda que EXILED por si solo no hace absolutamente nada, así que asegúrate de que instalas plugins desde **[nuestro servidor de Discord](https://discord.gg/PyUkWTg)**
- Para instalar un plugin simplemente:
  - Descarga un plugin de [*su* página de versiones](https://i.imgur.com/u34wgPD.jpg) (**¡DEBE ser un `.dll`!**))
  - Muévalo a: ``~/.config/EXILED/Plugins`` (si usas tu SSH como root, busca la carpeta `.config` correcta, la cual estará en `/home/(Usuario Servidor SCP)`)

# Configuración
EXILED por sí solo tiene algunas configuraciones.
Estas son generadas automáticamente cuando se inicia el servidor, las encontrarás en el archivo ``~/.config/EXILED/Configs/(PuertoServidor)-config.yml`` (``%AppData%\EXILED\Configs\(PuertoServidor)-config.yml`` en Windows).

Los ajustes de plugins ***NO*** están en el archivo ``config_gameplay.txt``, en vez de eso están en el archivo ``~/.config/EXILED/Configs/(PuertoServidor)-config.yml`` (``%AppData%\EXILED\(PuertoServidor)-config.yml`` en Windows).
Sin embargo, algunos plugins colocan sus configuraciones en otras carpetas, este archivo es simplemente el sitio por defecto. 
Mira la documentación o pregúntale al creador del plugin en particular si tienes algún problema.

# Para desarrolladores

Si deseas hacer un plugin para EXILED, es bastante fácil. Si prefieres ver algún tipo de tutorial, visita nuestra página de [Primeros Pasos](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md).

Para una guía más comprensible y tutoriales regularmente actualizados, échale un vistazo a [la página de EXILED](https://exiled-team.github.io/EXILED/articles/install.html).

Asegúrate de seguir estas normas antes de publicar un plugin:

 - Tu plugin debe contener una clase que herede de Exiled.API.Features.Plugin<>, si no, EXILED no podrá cargar el plugin cuando se inicie el servidor.
 - Cuando un plugin se inicia, el código del método ``OnEnabled()`` se ejecutará inmediatamente, no espera a que otros plugins se carguen. No espera a que el servidor se termine de iniciar. ***No espera por absolutamente nada.*** Cuando escribas tu método OnEnable(), asegúrate de que no intentas acceder a objetos/propiedades que puede que aún no estén disponibles en ese momento, como ServerConsole.Port, o PlayerManager.localPlayer.
 - Si tienes que acceder a objetos al principio, se recomienda que uses el evento WaitingForPlayers para hacerlo, si, por alguna razón, tienes que hacerlo incluso antes, encapsula el código en un bucle ```while(!x)``` la cual compruebe que la función/variable que intentas acceder no sea *null* antes de que continúe.
 - EXILED puede recargar plugins dinámicamente en medio de ejecución. Esto significa que, si tienes que actualizar un plugin en algún momento, puedes hacerlo sin tener que reiniciar el servidor. Sin embargo, si haces esto, el plugin debe estar correctamente configurado o tendrás un *ligero percance*. Échale un vistazo al apartado de ``Actualizaciones Dinámicas`` para más información y normas que seguir.
 - ***NO*** existe evento OnUpdate, OnFixedUpdate o OnLateUpdate en EXILED. Si tienes que (por algún motivo) ejecutar código tantas veces, puedes usar una corutina MEC que espere por un fotograma (0.01f) o usar una rutina de Timing como Timing.FixedUpdate.

### Deshabilitar parches de eventos de EXILED
***Esta función está en desuso y no está implementada.***

### Corrutinas Mec
Si no conoces MEC, esta es una guía muy simple para que empieces.
Las corrutinas MEC son métodos cronometrados, las cuales tienen la habilidad de esperar un tiempo predefinido antes de continuar la ejecución, sin interrumpir/dormir el hilo del juego principal.
las corrutinas MEC son seguras para usar con Unity, al contrario de los hilos tradicionales. ***NO intentes hacer hilos nuevos para interactuar con Unity, Estos CAUSARÁN errores con el servidor.***

Para usar MEC, tendrás que añadir ``Assembly-CSharp-firstpass.dll`` a las referencias del servidor, e incluir ``using MEC;``.
Aquí tienes un ejemplo de como llamar una corutina simple, que se repetirá en bucle después de un tiempo de espera:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MiCorutina());
}

public IEnumerator<float> MiCorutina()
{
    for (;;) //repetir esto indefinidamente
    {
        Log.Info("Hey, ¡Soy un bucle infinito!"); //Llamar Log.Info para imprimir una línea en la consola del servidor.
        yield return Timing.WaitForSeconds(5f); //Le dice a la corrutina que espere 5 segundos antes de continuar, al estar al final del bucle, efectivamente evita que el bucle se repita por 5 segundos.
    }
}
```

Recomendamos ***encarecidamente*** que hagas un poquito de investigación, o preguntes en el Discord si no conoces mucho de MEC y te gustaría aprender más, obtener consejos o pedir ayuda. Preguntas, no importa lo 'tontas que sean', siempre se responderán de la forma que más ayuda ofrezca y más clara posible para que los creadores de plugins mejoren sus habilidades. Un buen código es bueno para todos.

### Actualizaciones Dinámicas
EXILED como plataforma puede recargar plugins dinámicamente sin reiniciar el servidor.
Por ejemplo, si inicias el servidor con el plugin `Exiled.Events` solamente, y deseas añadir otro, no tienes que reiniciar el servidor para hacer esto. Puedes simplemente iniciar el RemoteAdmin/ServerConsole y utilizar el comando `reload plugins` para recargar todos los plugins, incluyendo otros que no estaban cargados previamente.

Eso también significa que se pueden *actualizar* plugins sin reiniciar el servidor completamente. Sin embargo, hay algunas reglas que seguir para que esto funcione correctamente:

***Para Hosts***
 - Si actualizas un plugin, asegúrate que el nombre de asamblea no corresponda con la versión instalada (si existe alguno). El creador del plugin hecho con las Actualizaciones Dinámicas en mente tendrá que hacer esto, cambiar el nombre no funciona.
 - Si el plugin soporta Actualizaciones Dinámicas, asegúrate de eliminar la versión posterior del plugin antes de recargar todos los plugins de EXILED, si no te aseguras de esto, podría haber efectos secundarios indeseados.
 - Cualquier problema que sea causa de las Actualizaciónes Dinámicas es responsabilidad exclusivamente del creador del plugin y de ti mismo. Mientras que EXILED soporta completamente (y recomienda) el uso de las Actualizaciones Dinámicas, la única forma de la que podría fallar es si el admin del servidor o el desarrollador del plugin hiciera algo mal. Comprueba tres veces de que todo está correcto por los dos lados antes de reportar un error a los desarrolladores de EXILED acerca de las Actualizaciones Dinámicas.

 ***Para Desarrolladores***

 - Los plugins que quieran implementar las Actualizaciones Dinámicas tienen que asegurarse de que se desubscriben de todo los eventos cuando es Deshabilitado (Disabled) o Recargado (Reloaded).
 - Los plugins que tengan parches propios de Harmony deben de usar algún tipo de variable que cambia adentro de la Instancia de Harmony, y se debe ejecutar UnPatchAll() cuando el plugin es deshabilitado o recargado.
 - Cualquier corutina que se inicie en el método OnEnabled también se tiene que deneter cuando se deshabilite o recargue el plugin.

Todo eso te puede hacer haciendo uso de los métodos OnReloaded() y OnDisabled() en la clase Plugin. Cuando EXILED recarga plugins, llama a OnDisabled() primero, después a OnReloaded(), después carga todas las asambleas nuevas, y finalmente OnEnabled() de nuevo.

Fíjate que son ensamblajes *nuevos*. Si reemplazas un ensamblaje con otro con el mismo nombre, ***NO*** se actualizará. Esto es debido al GAC (Global Assembly Cache o Caché de Ensamblaje Global), si intentas 'cargar' un ensamblaje que ya está en caché, cargará la del caché en vez de la nueva.
Por esta razón, si tu plugin soporta las Actualizaciones Dinámicas, debes crear cada versión con un nombre de ensamblaje distinto en las opciones de compilación (renombrar el archivo no funcionará). Además, ya que el ensamblaje antiguo se "destruye" cuando ya no se necesita, si no te desubscribes de los eventos correctos, eliminar tu instancia de Harmony, detener corrutinas u otros... Ese código seguirá ejecutándose al igual que el código de la nueva versión. 
Esto no es bueno...

Es por eso que los plugins que deseen hacer uso de las Actualizaciones Dinámicas ***DEBEN*** seguir estas guías o no se podrán subir al servidor de Discord (para evitar riesgos innecesarios para los administradores de servidores).

Pero no es obligatorio que todos los plugins soporten las Actualizaciones Dinámicas. Si no quieres, está bien, simplemente no cambies el nombre de asamblea de tu plugin cuando hagas una versión nueva y no tendrás que preocuparte por nada de esto, simplemente asegúrate de que el administrador del servidor sepa que tiene que reiniciar completamente el servidor para actualizar el plugin.

Transcripción por MarcosVLl2 con la ayuda de xRoier y GabiRP
