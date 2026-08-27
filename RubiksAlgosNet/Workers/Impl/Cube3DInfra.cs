using Clprolf.ArchUnitNet.Attributes;
using Raylib_cs;
using System.Numerics;
using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;
using static RubiksAlgosNet.Agents.ICubelet;

namespace RubiksAlgosNet.Workers.Impl;

[ClInfrastructure]
internal class Cube3DInfra: ICubeInfra
{
    private readonly RubiksCube cube;
    public Cube3DInfra(RubiksCube cube) { this.cube = cube; }

    // Couleurs officielles du Rubik's Cube
    static readonly Color ColorWhite = new Color(255, 255, 255, 255);
    static readonly Color ColorYellow = new Color(255, 213, 0, 255);
    static readonly Color ColorRed = new Color(185, 0, 0, 255);
    static readonly Color ColorOrange = new Color(255, 88, 0, 255);
    static readonly Color ColorBlue = new Color(0, 70, 173, 255);
    static readonly Color ColorGreen = new Color(0, 155, 72, 255);

    public Color ObtenirCouleurRaylib(Couleur c)
    {
        return c switch
        {
            Couleur.W => ColorWhite,
            Couleur.Y => ColorYellow,
            Couleur.R => ColorRed,
            Couleur.O => ColorOrange,
            Couleur.G => ColorGreen,
            Couleur.B => ColorBlue,
            _ => new Color(20, 20, 20, 255) // Plastique noir (Couleur.X)
        };
    }

    public void AfficherCube()
    {
        Raylib.InitWindow(800, 600, "Rubik's Cube 3D Coloré - Raylib-cs");
        Raylib.SetTargetFPS(60);

        Camera3D camera = new Camera3D
        {
            Position = new Vector3(6.0f, 6.0f, 6.0f),
            Target = new Vector3(0.0f, 0.0f, 0.0f),
            Up = new Vector3(0.0f, 1.0f, 0.0f),
            FovY = 45.0f,
            Projection = CameraProjection.Perspective
        };

        float angleX = 0.8f;
        float angleY = 0.8f;
        float distance = 10.0f;

        float cubeSize = 0.96f;
        float spacing = 1.0f;
        float stickerSize = 0.82f; // Taille de l'autocollant coloré
        float offset = cubeSize / 2.0f + 0.005f; // Légèrement au-dessus de la surface plastique

        while (!Raylib.WindowShouldClose())
        {
            // 1. Contrôle du ZOOM à la molette
            float wheel = Raylib.GetMouseWheelMove();
            distance -= wheel * 0.8f;
            if (distance < 3.0f) distance = 3.0f;
            if (distance > 25.0f) distance = 25.0f;

            // 2. Contrôle de la ROTATION avec le CLIC DROIT
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 delta = Raylib.GetMouseDelta();
                angleY -= delta.X * 0.005f;
                angleX += delta.Y * 0.005f;

                float limit = 1.5f;
                if (angleX > limit) angleX = limit;
                if (angleX < -limit) angleX = -limit;
            }

            camera.Position = new Vector3(
                (float)(distance * Math.Cos(angleX) * Math.Sin(angleY)),
                (float)(distance * Math.Sin(angleX)),
                (float)(distance * Math.Cos(angleX) * Math.Cos(angleY))
            );

            // 3. RENDU DE LA SCÈNE
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(30, 30, 30, 255));

            Raylib.BeginMode3D(camera);

            // On parcourt nos 27 cubelets dynamiquement !
            foreach (var piece in cube.Cubelets)
            {
                // Position 3D basée sur les coordonnées réelles du Cubelet
                Vector3 pos = new Vector3(piece.X * spacing, piece.Y * spacing, piece.Z * spacing);

                // Corps en plastique noir
                Raylib.DrawCube(pos, cubeSize, cubeSize, cubeSize, new Color(20, 20, 20, 255));
                Raylib.DrawCubeWires(pos, cubeSize, cubeSize, cubeSize, Color.Black);

                // --- RENDU DES 6 STICKERS DYNAMIQUES ---

                if (piece.Droite != Couleur.X)
                    DrawSticker(pos + new Vector3(offset, 0, 0), stickerSize, stickerSize, true, false, ObtenirCouleurRaylib(piece.Droite));

                if (piece.Gauche != Couleur.X)
                    DrawSticker(pos + new Vector3(-offset, 0, 0), stickerSize, stickerSize, true, false, ObtenirCouleurRaylib(piece.Gauche));

                if (piece.Haut != Couleur.X)
                    DrawSticker(pos + new Vector3(0, offset, 0), stickerSize, stickerSize, false, true, ObtenirCouleurRaylib(piece.Haut));

                if (piece.Bas != Couleur.X)
                    DrawSticker(pos + new Vector3(0, -offset, 0), stickerSize, stickerSize, false, true, ObtenirCouleurRaylib(piece.Bas));

                if (piece.Avant != Couleur.X)
                    DrawSticker(pos + new Vector3(0, 0, offset), stickerSize, stickerSize, false, false, ObtenirCouleurRaylib(piece.Avant));

                if (piece.Arriere != Couleur.X)
                    DrawSticker(pos + new Vector3(0, 0, -offset), stickerSize, stickerSize, false, false, ObtenirCouleurRaylib(piece.Arriere));
            }

            // Exécution des mouvements en appuyant sur les touches du clavier
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                    cube.Executer(Mouvement.RPrime); // Maj + R -> R'
                else
                    cube.Executer(Mouvement.R);      // R -> R
            }

            if (Raylib.IsKeyPressed(KeyboardKey.L))
            {
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                    cube.Executer(Mouvement.LPrime); // Maj + L -> L'
                else
                    cube.Executer(Mouvement.L);      // L -> L
            }

            if (Raylib.IsKeyPressed(KeyboardKey.F))
            {
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                    cube.Executer(Mouvement.FPrime); // Maj + F -> F'
                else
                    cube.Executer(Mouvement.F);      // F -> F
            }

            if (Raylib.IsKeyPressed(KeyboardKey.B))
            {
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                    cube.Executer(Mouvement.BPrime); // Maj + B -> B'
                else
                    cube.Executer(Mouvement.B);      // B -> B
            }

            if (Raylib.IsKeyPressed(KeyboardKey.U))         
            {
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                    cube.Executer(Mouvement.UPrime); // Maj + U -> U'
                else
                    cube.Executer(Mouvement.U);      // U -> U
            }

            if (Raylib.IsKeyPressed(KeyboardKey.D))
            {
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                    cube.Executer(Mouvement.DPrime); // Maj + D -> D'
                else
                    cube.Executer(Mouvement.D);      // D -> D
            }

            Raylib.EndMode3D();

            Raylib.DrawText("CLIC DROIT : Tourner la caméra | MOLETTE : Zoom", 10, 10, 18, Color.White);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    /// <summary>
    /// On dessine un sticker = cube très fin.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="isXAxis"></param>
    /// <param name="isYAxis"></param>
    /// <param name="color"></param>
    public void DrawSticker(Vector3 center, float width, float height, bool isXAxis, bool isYAxis, Color color)
    {
        if (isXAxis)
        {
            // Plane orienté vers X (gauche/droite)
            Raylib.DrawCube(center, 0.01f, height, width, color);
        }
        else if (isYAxis)
        {
            // Plane orienté vers Y (haut/bas)
            Raylib.DrawCube(center, width, 0.01f, height, color);
        }
        else
        {
            // Plane orienté vers Z (avant/arrière)
            Raylib.DrawCube(center, width, height, 0.01f, color);
        }
    }

}