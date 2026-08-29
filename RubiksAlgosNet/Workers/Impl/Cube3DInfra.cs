using Clprolf.ArchUnitNet.Attributes;
using Raylib_cs;
using System.Numerics;
using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;
using static RubiksAlgosNet.Agents.ICubelet;

namespace RubiksAlgosNet.Workers.Impl;

[ClInfrastructure]
internal class Cube3DInfra : ICubeInfra
{
    private readonly RubiksCube cube;
    public Cube3DInfra(RubiksCube cube) { this.cube = cube; }

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
            _ => new Color(20, 20, 20, 255)
        };
    }

    public void AfficherCube()
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

        float cubeSize = 0.96f;
        float spacing = 1.0f;
        float stickerSize = 0.82f;
        float offset = cubeSize / 2.0f + 0.005f;
        float angle90 = (float)(Math.PI / 2.0);

        while (!Raylib.WindowShouldClose())
        {
            bool isShift = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

            // 1. ZOOM (déplace la position sur son axe de visée)
            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                float dist = camera.Position.Length();
                dist = Math.Clamp(dist - wheel * 0.8f, 3.0f, 25.0f);
                camera.Position = Vector3.Normalize(camera.Position) * dist;
            }

            // 2. ROTATION LIBRE AU CLIC DROIT
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 delta = Raylib.GetMouseDelta();

                Quaternion rotY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -delta.X * 0.005f);
                camera.Position = Vector3.Transform(camera.Position, rotY);
                camera.Up = Vector3.Transform(camera.Up, rotY);

                Vector3 right = Vector3.Normalize(Vector3.Cross(camera.Position, camera.Up));
                Quaternion rotRight = Quaternion.CreateFromAxisAngle(right, delta.Y * 0.005f);
                camera.Position = Vector3.Transform(camera.Position, rotRight);
                camera.Up = Vector3.Transform(camera.Up, rotRight);
            }

            // 3. ROTATIONS GLOBALES X, Y, Z (à 90°)
            if (Raylib.IsKeyPressed(KeyboardKey.X))
            {
                cube.Executer(isShift ? Mouvement.xPrime : Mouvement.x);
                float angle = isShift ? -angle90 : angle90;
                Quaternion rotX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, angle);
                camera.Position = Vector3.Transform(camera.Position, rotX);
                camera.Up = Vector3.Transform(camera.Up, rotX);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Y))
            {
                cube.Executer(isShift ? Mouvement.yPrime : Mouvement.y);
                float angle = isShift ? -angle90 : angle90;
                Quaternion rotY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angle);
                camera.Position = Vector3.Transform(camera.Position, rotY);
                camera.Up = Vector3.Transform(camera.Up, rotY);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.W))
            {
                cube.Executer(isShift ? Mouvement.zPrime : Mouvement.z);

                // Dans le repère 3D : une rotation Z inverse l'angle pour correspondre au sens horaire du joueur
                float angle = isShift ? -angle90 : angle90;

                // Quaternion de rotation autour de l'axe Z du monde (0, 0, 1)
                Quaternion rotZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, angle);

                // On applique la rotation aux DEUX composantes
                camera.Position = Vector3.Transform(camera.Position, rotZ);
                camera.Up = Vector3.Transform(camera.Up, rotZ);
            }

            // 4. RENDU
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(30, 30, 30, 255));

            Raylib.BeginMode3D(camera);

            foreach (var piece in cube.Cubelets)
            {
                Vector3 pos = new Vector3(piece.X * spacing, piece.Y * spacing, piece.Z * spacing);

                Raylib.DrawCube(pos, cubeSize, cubeSize, cubeSize, new Color(20, 20, 20, 255));
                Raylib.DrawCubeWires(pos, cubeSize, cubeSize, cubeSize, Color.Black);

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

            // 5. TRANCHES
            if (Raylib.IsKeyPressed(KeyboardKey.R)) cube.Executer(isShift ? Mouvement.RPrime : Mouvement.R);
            if (Raylib.IsKeyPressed(KeyboardKey.L)) cube.Executer(isShift ? Mouvement.LPrime : Mouvement.L);
            if (Raylib.IsKeyPressed(KeyboardKey.F)) cube.Executer(isShift ? Mouvement.FPrime : Mouvement.F);
            if (Raylib.IsKeyPressed(KeyboardKey.B)) cube.Executer(isShift ? Mouvement.BPrime : Mouvement.B);
            if (Raylib.IsKeyPressed(KeyboardKey.U)) cube.Executer(isShift ? Mouvement.UPrime : Mouvement.U);
            if (Raylib.IsKeyPressed(KeyboardKey.D)) cube.Executer(isShift ? Mouvement.DPrime : Mouvement.D);

            Raylib.EndMode3D();

            Raylib.DrawText("CLIC DROIT : Caméra libre | MOLETTE : Zoom | TOUCHES : R, L, U, D, F, B, X, Y, Z (+Shift)", 10, 10, 16, Color.White);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public void DrawSticker(Vector3 center, float width, float height, bool isXAxis, bool isYAxis, Color color)
    {
        if (isXAxis)
            Raylib.DrawCube(center, 0.01f, height, width, color);
        else if (isYAxis)
            Raylib.DrawCube(center, width, 0.01f, height, color);
        else
            Raylib.DrawCube(center, width, height, 0.01f, color);
    }
}