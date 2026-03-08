# Audio

Los sonidos del juego se generan por código en `AudioManager.cs` (salto, moneda, daño, muerte, etc.).

Si quiero cambiar algún sonido por un archivo real, solo tengo que:
1. Meter el `.wav` o `.ogg` en esta carpeta
2. En la escena, buscar el `AudioManager` y asignar el clip en el Inspector
