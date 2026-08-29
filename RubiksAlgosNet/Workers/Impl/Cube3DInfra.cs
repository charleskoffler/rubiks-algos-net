using Clprolf.ArchUnitNet.Attributes;
using Raylib_cs;
using System.Numerics;
using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;
using static RubiksAlgosNet.Agents.ICubelet;
using RubiksAlgos.Agents.Impl;

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

        // 1. Où se trouve la caméra au démarrage (ex: en haut à droite en vue 3/4)
        Vector3 positionInitiale = new Vector3(6.0f, 6.0f, 6.0f);
     
        Vector3 dirInitiale = Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f));
        float distance = 1f;
        float distanceMin = 0.3f; // Permet de zoomer de très près
        float distanceMax = 5.0f; // Limite pour ne pas trop s'éloigner

        // 2. Quelle direction pointe vers le haut pour la caméra au démarrage (l'axe Y vers le haut)
        Vector3 upInitial = new Vector3(0.0f, 1.0f, 0.0f);

        Camera3D camera = new Camera3D
        {
            Position = positionInitiale,
            Target = new Vector3(0.0f, 0.0f, 0.0f),
            Up = upInitial,
            FovY = 45.0f,
            Projection = CameraProjection.Perspective
        };

        float cubeSize = 0.96f;
        float spacing = 1.0f;
        float stickerSize = 0.82f;
        float offset = cubeSize / 2.0f + 0.005f;
        float deltaSourisX = 0.0f;
        float deltaSourisY = 0.0f;

        while (!Raylib.WindowShouldClose())
        {
            bool isShift = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);
            bool isCtrl = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);

            // ZOOM (déplace la position sur son axe de visée)
            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                distance -= wheel * 0.1f; // Sensibilité du zoom
                distance = Math.Clamp(distance, distanceMin, distanceMax); // Empêche de trop s'approcher ou de trop s'éloigner
            }

            // ROTATION LIBRE AU CLIC DROIT
            // --- GESTION DU CLIC DROIT (Rotation manuelle de l'utilisateur) ---
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 mouseDelta = Raylib.GetMouseDelta();

                // Gauche / Droite
                deltaSourisX -= mouseDelta.X * 0.005f;

                // Haut / Bas
                deltaSourisY += mouseDelta.Y * 0.005f;

                // Bloquer le pitch pour éviter le retournement de la vue
                //deltaSourisY = Math.Clamp(deltaSourisY, -1.4f, 1.4f);
            }

            // ROTATIONS GLOBALES X, Y, Z
            // Inputs(le domaine calcule la nouvelle orientation)
            if (Raylib.IsKeyPressed(KeyboardKey.X))
                cube.Executer(isShift ? Mouvement.xPrime : Mouvement.x);

            if (Raylib.IsKeyPressed(KeyboardKey.Y))
                cube.Executer(isShift ? Mouvement.yPrime : Mouvement.y);

            bool isZPressed = Raylib.IsKeyPressed(KeyboardKey.W);

            if (isZPressed)
                cube.Executer(isShift ? Mouvement.zPrime : Mouvement.z);

            // OBTENTION DE LA ROTATION DU CUBE (OrientationRoot) ---
            Quaternion rotCube = OrientationReducerHelper.ObtenirRotation(cube.OrientationCourante);

            Vector3 basePos = Vector3.Transform(positionInitiale, rotCube);
            Vector3 baseUp = Vector3.Transform(upInitial, rotCube);

            // Déplacement de la souris (on garde la même accumulation)
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 mouseDelta = Raylib.GetMouseDelta();

                deltaSourisX -= mouseDelta.X * 0.005f;
                deltaSourisY += mouseDelta.Y * 0.005f;

                deltaSourisY = Math.Clamp(deltaSourisY, -1.4f, 1.4f);
            }

            // Repère local standard
            Vector3 rightLocal = Vector3.Normalize(Vector3.Cross(baseUp, -basePos));

            Quaternion qYaw = Quaternion.CreateFromAxisAngle(baseUp, deltaSourisX);

            Quaternion qPitch = Quaternion.CreateFromAxisAngle(rightLocal, -deltaSourisY);

            Quaternion qMouse = qYaw * qPitch;
            Vector3 dirFinale = Vector3.Transform(basePos, qMouse);

            // La caméra orbitale finale
            camera.Position = dirFinale * distance;
            camera.Up = Vector3.Transform(baseUp, qMouse);

            // RENDU
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

            //  TRANCHES
            
            if (Raylib.IsKeyPressed(KeyboardKey.R) && !isCtrl) cube.Executer(isShift ? Mouvement.RPrime : Mouvement.R);
            if (Raylib.IsKeyPressed(KeyboardKey.L)) cube.Executer(isShift ? Mouvement.LPrime : Mouvement.L);
            if (Raylib.IsKeyPressed(KeyboardKey.F)) cube.Executer(isShift ? Mouvement.FPrime : Mouvement.F);
            if (Raylib.IsKeyPressed(KeyboardKey.B)) cube.Executer(isShift ? Mouvement.BPrime : Mouvement.B);
            if (Raylib.IsKeyPressed(KeyboardKey.U)) cube.Executer(isShift ? Mouvement.UPrime : Mouvement.U);
            if (Raylib.IsKeyPressed(KeyboardKey.D)) cube.Executer(isShift ? Mouvement.DPrime : Mouvement.D);

            if (Raylib.IsKeyPressed(KeyboardKey.R) && isCtrl) cube.Executer(isShift ? Mouvement.rPrime : Mouvement.r);

            bool isMPressed = Raylib.IsKeyPressed(KeyboardKey.Semicolon)
               || Raylib.IsKeyPressed(KeyboardKey.Comma);

            if (isMPressed) cube.Executer(isShift ? Mouvement.MPrime : Mouvement.M);

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