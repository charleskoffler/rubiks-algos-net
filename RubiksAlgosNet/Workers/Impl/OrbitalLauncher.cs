using Clprolf.ArchUnitNet.Attributes;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace RubiksAlgos.Workers.Impl
{
    [ClMechanism]
    public class OrbitalLauncher : IOrbitalLauncher
    {
        public void launchOrbital()
        {
            Raylib.InitWindow(800, 600, "Rubik's Cube 3D - Raylib-cs");
            Raylib.SetTargetFPS(60);

            Camera3D camera = new Camera3D
            {
                Position = new Vector3(6.0f, 6.0f, 6.0f),
                Target = new Vector3(0.0f, 0.0f, 0.0f),
                Up = new Vector3(0.0f, 1.0f, 0.0f),
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };

            // Variables pour gérer la rotation à la souris
            float angleX = 0.8f; // Angle vertical
            float angleY = 0.8f; // Angle horizontal
            float distance = 10.0f; // Distance par rapport au centre

            float size = 0.96f;
            float spacing = 1.0f;

            while (!Raylib.WindowShouldClose())
            {
                // 1. ZOOM à la molette
                float wheel = Raylib.GetMouseWheelMove();
                distance -= wheel * 0.8f;
                if (distance < 3.0f) distance = 3.0f;   // Limite zoom avant
                if (distance > 25.0f) distance = 25.0f; // Limite zoom arrière

                // 2. ROTATION avec le CLIC DROIT enfoncé
                if (Raylib.IsMouseButtonDown(MouseButton.Right))
                {
                    Vector2 delta = Raylib.GetMouseDelta();
                    angleY -= delta.X * 0.005f; // Sensibilité horizontale
                    angleX += delta.Y * 0.005f; // Sensibilité verticale

                    // Limiter l'angle vertical pour éviter de renverser la caméra
                    float limit = 1.5f; // ~85 degrés
                    if (angleX > limit) angleX = limit;
                    if (angleX < -limit) angleX = -limit;
                }

                // 3. Reconstitution de la position 3D de la caméra (Coordonnées sphériques)
                camera.Position = new Vector3(
                    (float)(distance * Math.Cos(angleX) * Math.Sin(angleY)),
                            (float)(distance * Math.Sin(angleX)),
                            (float)(distance * Math.Cos(angleX) * Math.Cos(angleY))
                        );

                // Rendu
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(30, 30, 30, 255));

                Raylib.BeginMode3D(camera);

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            Vector3 position = new Vector3(x * spacing, y * spacing, z * spacing);
                            Raylib.DrawCube(position, size, size, size, Color.DarkGray);
                            Raylib.DrawCubeWires(position, size, size, size, Color.Black);
                        }
                    }
                }

                Raylib.EndMode3D();

                Raylib.DrawText("CLIC DROIT + glisser : Tourner la caméra", 10, 10, 18, Color.White);
                Raylib.DrawText("MOLETTE : Zoomer / Dézoomer", 10, 32, 18, Color.Gray);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}