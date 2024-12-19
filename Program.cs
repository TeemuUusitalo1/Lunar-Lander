using System.Numerics;
using ZeroElectric.Vinculum; // Lisätty oikea namespace Raylibille

namespace LunarLander
{
    internal class Lander
    {
        static void Main(string[] args)
        {
            Lander game = new Lander();
            game.Init();
            game.GameLoop();
        }

        /////////////////////////////////////

        // Pelaajan sijainti
        // x on aluksen keskikohta
        // y on aluksen pohja
        float x = 120;
        float y = 16;

        // Onko moottori päällä
        bool engine_on = false;

        // Pelaajan nopeus, polttoaine ja polttonopeus
        float velocity = 0;
        float fuel = 100;
        float fuel_consumption = 10.0f;

        // Laskeutumisalustan katon sijainti y-akselilla. Y kasvaa alaspäin.
        int landing_y = 125;

        // Ruudunpäivitykseen menevä aika (oletus)
        float delta_time = 1.0f / 60.0f;

        // Moottorin voimakkuus
        float acceleration = 20.9f;

        // Painovoiman voimakkuus
        float gravity = 9.89f;

        // Pelialueen ja ikkunan mittasuhteet
        int game_width = 240;
        int game_height = 136;

        int screen_width = 1280;
        int screen_height = 720;

        RenderTexture renderTexture;
        Texture shipTexture;

        void Init()
        {
            // Aloita Raylib ja luo ikkuna.
            Raylib.InitWindow(screen_width, screen_height, "Lunar Lander");
            renderTexture = Raylib.LoadRenderTexture(game_width, game_height);
            shipTexture = Raylib.LoadTexture("Map/ship.png");
            Raylib.SetTargetFPS(60);
        }

        void GameLoop()
        {
            while (!Raylib.WindowShouldClose()) // Pyöritä gamelooppia kunnes ikkuna suljetaan
            {
                Update();
                Draw();
            }
            Raylib.UnloadTexture(shipTexture); // Vapauta tekstuurin muisti
            Raylib.CloseWindow(); // Sulje Raylib-ikkuna
        }

        void Update()
        {
            // Kysy Raylibiltä miten pitkään yksi ruudunpäivitys kesti
            delta_time = Raylib.GetFrameTime();

            // Lisää painovoiman vaikutus
            velocity += gravity * delta_time;

            // Kun pelaaja painaa nappia (esim. SPACE) ja polttoainetta on jäljellä
            if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) && fuel > 0)
            {
                velocity -= acceleration * delta_time;
                fuel -= fuel_consumption * delta_time;
                engine_on = true;
            }
            else
            {
                engine_on = false;
            }

            // Liikuta alusta
            y += velocity * delta_time;

            // Estä alusta menemästä pelialueen ulkopuolelle
            if (y >= game_height - 10) // Oletetaan 10 yksikön marginaali
            {
                y = game_height - 10;
                velocity = 0;
            }
        }

        void Draw()
        {
            // Aloita piirtäminen renderöintipuskuriin
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Raylib.BLACK);

            int plat_x = (int)x - 30;
            int plat_y = landing_y;
            int plat_w = 60;
            int plat_h = 10;

            // Piirrä laskeutumisalusta suorakulmiona
            Raylib.DrawRectangle(plat_x, plat_y, plat_w, plat_h, Raylib.GRAY);

            // Piirrä alus tekstuurina
            Raylib.DrawTexture(shipTexture, (int)x - shipTexture.width / 2, (int)y - shipTexture.height, Raylib.WHITE);

            // Piirrä moottorin liekki
            if (engine_on)
            {
                Raylib.DrawTriangle(
                    new Vector2(x - 5, y),
                    new Vector2(x, y + 32),
                    new Vector2(x + 5, y),
                    Raylib.YELLOW);
            }

            // Piirrä polttoaineen tilanne
            Raylib.DrawRectangle(9, 9, 102, 12, Raylib.BLUE);
            Raylib.DrawRectangle(10, 10, (int)fuel, 10, Raylib.YELLOW);
            Raylib.DrawText("FUEL", 11, 11, 12, Raylib.DARKBLUE);

            // Piirrä debug-tietoja
            Raylib.DrawText($"V:{velocity}", 11, 31, 8, Raylib.WHITE);
            Raylib.DrawText($"dt:{delta_time}", 11, 41, 8, Raylib.WHITE);

            // Lopeta piirtäminen renderöintipuskuriin
            Raylib.EndTextureMode();

            // Skaalaa peli-ikkunan sisältö oikeaan kokoon
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.DrawTexturePro(
                renderTexture.texture,
                new Rectangle(0, 0, game_width, -game_height),
                new Rectangle(0, 0, screen_width, screen_height),
                Vector2.Zero,
                0.0f,
                Raylib.WHITE);
            Raylib.EndDrawing();
        }
    }
}
