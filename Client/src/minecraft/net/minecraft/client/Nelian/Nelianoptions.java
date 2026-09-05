package net.minecraft.client.Nelian;

import java.io.*;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import java.nio.charset.StandardCharsets;


import net.minecraft.client.renderer.entity.RenderManager;
import net.minecraft.entity.Entity;

public class Nelianoptions {

    // ========== DOSYA YOLU ==========
    private static final String DIR_PATH  =
            System.getenv("APPDATA") + File.separator
            + ".minecraft" + File.separator ; // directory

    private static final String FILE_PATH = DIR_PATH + File.separator + "optionsnl.txt"; //options file

    // ========== MOD TOGGLE'LARI ==========
    public static boolean keystrokesEnabled = false; 
    public static boolean CapeFizik = true; 
    public static boolean hitboxesEnabled   = false;
    public static boolean crosshairEnabled  = false;
    public static boolean nametagsEnabled   = true;
    public static boolean fpsEnabled        = true;
    public static boolean cpsEnabled        = false;
    public static boolean freelookEnabled   = false;
    public static boolean cameraModsEnabled = false;
    public static boolean IsCustomLoadingArtEnabled = false;
    public static int keystrokesX = 5;
    public static int keystrokesY = 60;
    public static boolean freelookForcedOffByHypixel = false;
    public static boolean betterZoomEnabled = true;
    public static boolean autoGGEnabled     = false;
    // ========== KEYSTROKES AYARLARI ==========
    public static int keystrokesKeySize = 30;                    // Tuş boyutu (piksel)
    public static int keystrokesGap = 3;                         // Tuşlar arası boşluk (piksel)
                 // Y pozisyonu
    public static int keystrokesColorNormal = 0xAA1A1A1A;       // Normal renk (ARGB)
    public static int keystrokesColorPressed = 0xAA8EFFFF;      // Basılı renk (ARGB)
    public static int keystrokesColorText = 0xFFFFFFFF;         // Yazı rengi (ARGB)
    public static float keystrokesAnimationSpeed = 0.20F;       // Animasyon hızı
    public static boolean keystrokesShowLMB = true;             // LMB göster
    public static boolean keystrokesShowRMB = true;             // RMB göster
    public static boolean keystrokesShowSpace = true;           // SPACE göster
    public static boolean keystrokesShowWASD = true;            // WASD göster
    public static boolean keystrokesShowCPS = true;             // CPS değerlerini göster
    public static boolean keystrokesBackground = true;          // Arka plan göster
    public static boolean keystrokesRoundedCorners = false;     // Yuvarlak köşe
    public static int keystrokesCornerRadius = 3;               // Köşe yarıçapı
    public static boolean nelianFontEnabled = true;
    // ========== CROSSHAIR AYARLARI ==========
    public static int     crosshairColor     = 0xFFFFFFFF;      // ARGB
    public static int     crosshairGap       = 0;               // piksel
    public static int     crosshairLength    = 4;               // piksel
    public static int     crosshairThickness = 1;               // piksel
    public static int     crosshairAlpha     = 255;             // 0–255
    public static boolean crosshairDot       = false;           // orta nokta
    public static boolean crosshairRainbow   = false;           // rainbow modu
    
    // ========== CPS AYARLARI ==========
    public static int cpsMinCPS = 6;                            // Düşük -> Orta eşik
    public static int cpsMidCPS = 10;                           // Orta -> Yüksek eşik
    public static int cpsHighCPS = 15;                          // Yüksek -> Çok yüksek eşik
    public static int cpsVeryHighCPS = 20;                      // Çok yüksek eşik
    public static boolean cpsShowLeft = true;
    public static boolean cpsShowRight = true;
    public static boolean cpsShowTotal = true;
    public static boolean cpsShowBackground = true;
    public static int cpsColorLow = 0xFFAAAAAA;                 // Düşük CPS rengi
    public static int cpsColorMedium = 0xFF55FF55;              // Orta CPS rengi
    public static int cpsColorHigh = 0xFFFFAA55;                // Yüksek CPS rengi
    public static int cpsColorVeryHigh = 0xFFFF5555;            // Çok yüksek CPS rengi
    
    // ========== SHAKE AYARLARI ==========
    public static boolean shakeDisableHurtCam = false; 
    public static boolean shakeMinimalizeBobbing = false;
    public static boolean minimalizeShakeEnabled = false;
    
    // ========== NAMETAG AYARLARI ==========
    public static boolean ShowNametagEnabled = false;
    
    // ========== TNT TIMER AYARLARI ==========
    public static boolean tntTimerEnabled = true;
    public static boolean tntTimerShowProgress = true; 
    public static boolean tntTimerShowSeconds = true;
    public static int tntTimerPosition = 1;
    public static int fpsX = 5;
    public static int fpsY = 5;
    public static float fpsScale = 1.0F;
    public static float cpsScale = 1.0F;
    public static int cpsX = 5;
    public static int cpsY = 30;


    // ========== HITBOX AYARLARI ==========
    public static float hitboxRed = 1.0f;
    public static float hitboxGreen = 1.0f;
    public static float hitboxBlue = 1.0f;
    public static float hitboxAlpha = 0.5f;
    public static int hitboxLineThickness = 2;
    public static boolean hitboxUseCustomColor = false;
    public static boolean hitboxShowEyeHeight = true;
    public static boolean hitboxShowLookVector = true;

    // ========== YÜKLEYİCİ ==========
    public static void load() {
        File file = new File(FILE_PATH);
        if (!file.exists()) {
            save(); 
            return;
        }
   
        try (BufferedReader br = new BufferedReader(
                new InputStreamReader(new FileInputStream(file), StandardCharsets.UTF_8))) {
            String line;
            while ((line = br.readLine()) != null) {
                line = line.trim();
                if (line.isEmpty() || line.startsWith("#")) continue;
                int sep = line.indexOf(':');
                if (sep < 0) continue;
                String key = line.substring(0, sep).trim();
                String val = line.substring(sep + 1).trim();
                applyOption(key, val);
            }
        } catch (IOException e) {
            // Okuma hatası – defaults kullan
        }
    }

    // ========== KAYDEDICI ==========
    public static void save() {
        try {
            File dir = new File(DIR_PATH);
            if (!dir.exists()) dir.mkdirs();

            File file = new File(FILE_PATH);
            try (PrintWriter pw = new PrintWriter(
                    new OutputStreamWriter(new FileOutputStream(file), StandardCharsets.UTF_8))) {

                pw.println("# Nelian Client Options");
                pw.println("# You can edit your in-game options from here!");
                pw.println("# Its probably better if you don't touch these from here though. :D");
                pw.println();

                pw.println("# === MOD TOGGLES ===");
                pw.println("cape:"   + b(CapeFizik));
                pw.println("hitboxes:"   + b(hitboxesEnabled));
                pw.println("crosshair:"  + b(crosshairEnabled));
                pw.println("nametags:"   + b(nametagsEnabled));
                pw.println("fps:"        + b(fpsEnabled));
                pw.println("cps:"        + b(cpsEnabled));
                pw.println("freelook:"   + b(freelookEnabled));
                pw.println("minimalizeShake:"   + b(minimalizeShakeEnabled));
                pw.println("keystrokes:" + b(keystrokesEnabled));
                pw.println("Custom_Wallpaper:" + b(IsCustomLoadingArtEnabled));
                pw.println("betterZoom:" + b(betterZoomEnabled));
                pw.println("autoGG:" + b(autoGGEnabled));
                pw.println();


                pw.println("# === KEYSTROKES ===");
                pw.println("keystrokesKeySize:" + keystrokesKeySize);
                pw.println("keystrokesGap:" + keystrokesGap);
                pw.println("keystrokesX:" + keystrokesX);
                pw.println("keystrokesY:" + keystrokesY);
                pw.println("keystrokesColorNormal:" + (keystrokesColorNormal & 0xFFFFFFFFL));
                pw.println("keystrokesColorPressed:" + (keystrokesColorPressed & 0xFFFFFFFFL));
                pw.println("keystrokesColorText:" + (keystrokesColorText & 0xFFFFFFFFL));
                pw.println("keystrokesAnimationSpeed:" + keystrokesAnimationSpeed);
                pw.println("keystrokesShowLMB:" + b(keystrokesShowLMB));
                pw.println("keystrokesShowRMB:" + b(keystrokesShowRMB));
                pw.println("keystrokesShowSpace:" + b(keystrokesShowSpace));
                pw.println("keystrokesShowWASD:" + b(keystrokesShowWASD));
                pw.println("keystrokesShowCPS:" + b(keystrokesShowCPS));
                pw.println("keystrokesBackground:" + b(keystrokesBackground));
                pw.println("keystrokesRoundedCorners:" + b(keystrokesRoundedCorners));
                pw.println("keystrokesCornerRadius:" + keystrokesCornerRadius);
                pw.println();


                pw.println("# === CROSSHAIR ===");
                pw.println("crosshairColor:"     + (crosshairColor & 0xFFFFFFFFL));
                pw.println("crosshairGap:"       + crosshairGap);
                pw.println("crosshairLength:"    + crosshairLength);
                pw.println("crosshairThickness:" + crosshairThickness);
                pw.println("crosshairAlpha:"     + crosshairAlpha);
                pw.println("crosshairDot:"       + b(crosshairDot));
                pw.println("crosshairRainbow:"   + b(crosshairRainbow));
                pw.println();


                pw.println("# === HITBOXES ===");
                pw.println("hitboxRed:" + hitboxRed);
                pw.println("hitboxGreen:" + hitboxGreen);
                pw.println("hitboxBlue:" + hitboxBlue);
                pw.println("hitboxAlpha:" + hitboxAlpha);
                pw.println("hitboxLineThickness:" + hitboxLineThickness);
                pw.println("hitboxUseCustomColor:" + b(hitboxUseCustomColor));
                pw.println("hitboxShowEyeHeight:" + b(hitboxShowEyeHeight));
                pw.println("hitboxShowLookVector:" + b(hitboxShowLookVector));
                pw.println();


                pw.println("# === CPS ===");
                pw.println("cpsMinCPS:" + cpsMinCPS);
                pw.println("cpsMidCPS:" + cpsMidCPS);
                pw.println("cpsHighCPS:" + cpsHighCPS);
                pw.println("cpsVeryHighCPS:" + cpsVeryHighCPS);
                pw.println("cpsColorLow:" + (cpsColorLow & 0xFFFFFFFFL));
                pw.println("cpsColorMedium:" + (cpsColorMedium & 0xFFFFFFFFL));
                pw.println("cpsColorHigh:" + (cpsColorHigh & 0xFFFFFFFFL));
                pw.println("cpsColorVeryHigh:" + (cpsColorVeryHigh & 0xFFFFFFFFL));
                pw.println("cpsShowLeft:" + b(cpsShowLeft));
                pw.println("cpsShowRight:" + b(cpsShowRight));
                pw.println("cpsShowTotal:" + b(cpsShowTotal));
                pw.println("cpsShowBackground:" + b(cpsShowBackground));
                pw.println();


                pw.println("# === CAMERA ===");
                pw.println("cameraMods:" + b(cameraModsEnabled));
                pw.println("shakeDisableHurtCam:" + b(shakeDisableHurtCam));
                pw.println("shakeMinimalizeBobbing:" + b(shakeMinimalizeBobbing));
                pw.println();


                pw.println("# === NAMETAG ===");
                pw.println("ShowNametagEnabled:" + b(ShowNametagEnabled));
                pw.println();


                pw.println("# === HUD ===");
                pw.println("fpsX:" + fpsX);
                pw.println("fpsY:" + fpsY);
                pw.println("cpsX:" + cpsX);
                pw.println("cpsY:" + cpsY);
                pw.println("keystrokesX:" + keystrokesX);
                pw.println("keystrokesY:" + keystrokesY);
                pw.println("fpsScale:" + fpsScale);
                pw.println("cpsScale:" + cpsScale);
                pw.println("nelianFont:" + b(nelianFontEnabled));
                pw.println();
            }
        } catch (IOException e) {
            LOGGER.error("Save Failed!", e);
        }
    }
    private static final Logger LOGGER = LogManager.getLogger();
    private static void applyOption(String key, String val) {
        try {
            switch (key) {
                // Mod toggles
            	  case "cape":            CapeFizik  = parseBool(val); break;
                case "hitboxes":            hitboxesEnabled   = parseBool(val); break;
                case "crosshair":           crosshairEnabled  = parseBool(val); break;
                case "nametags":            nametagsEnabled   = parseBool(val); break;
                case "fps":                 fpsEnabled        = parseBool(val); break;
                case "cps":                 cpsEnabled        = parseBool(val); break;
                case "freelook":            freelookEnabled   = parseBool(val); break;
                case "minimalizeShake":     minimalizeShakeEnabled = parseBool(val); break;
                case "keystrokes":          keystrokesEnabled = parseBool(val); break;
                case "Custom_Wallpaper":	IsCustomLoadingArtEnabled = parseBool(val); break;
                case "betterZoom":          betterZoomEnabled = parseBool(val); break;
                case "autoGG":               autoGGEnabled = parseBool(val); break; 
                // Keystrokes 
                case "keystrokesKeySize":       keystrokesKeySize = clampI(Integer.parseInt(val), 15, 60); break;
                case "keystrokesGap":           keystrokesGap = clampI(Integer.parseInt(val), 0, 15); break;
                case "keystrokesX":             keystrokesX = clampI(Integer.parseInt(val), 0, 1000); break;
                case "keystrokesY":             keystrokesY = clampI(Integer.parseInt(val), 0, 1000); break;
                case "keystrokesColorNormal":   keystrokesColorNormal = (int) Long.parseLong(val); break;
                case "keystrokesColorPressed":  keystrokesColorPressed = (int) Long.parseLong(val); break;
                case "keystrokesColorText":     keystrokesColorText = (int) Long.parseLong(val); break;
                case "keystrokesAnimationSpeed": keystrokesAnimationSpeed = Float.parseFloat(val); break;
                case "keystrokesShowLMB":       keystrokesShowLMB = parseBool(val); break;
                case "keystrokesShowRMB":       keystrokesShowRMB = parseBool(val); break;
                case "keystrokesShowSpace":     keystrokesShowSpace = parseBool(val); break;
                case "keystrokesShowWASD":      keystrokesShowWASD = parseBool(val); break;
                case "keystrokesShowCPS":       keystrokesShowCPS = parseBool(val); break;
                case "keystrokesBackground":    keystrokesBackground = parseBool(val); break;
                case "keystrokesRoundedCorners": keystrokesRoundedCorners = parseBool(val); break;
                case "keystrokesCornerRadius":  keystrokesCornerRadius = clampI(Integer.parseInt(val), 0, 15); break;

                // Crosshair
                case "crosshairColor":      crosshairColor     = (int) Long.parseLong(val); break;
                case "crosshairGap":        crosshairGap       = clampI(Integer.parseInt(val), 0, 20); break;
                case "crosshairLength":     crosshairLength    = clampI(Integer.parseInt(val), 1, 30); break;
                case "crosshairThickness":  crosshairThickness = clampI(Integer.parseInt(val), 1, 10); break;
                case "crosshairAlpha":      crosshairAlpha     = clampI(Integer.parseInt(val), 0, 255); break;
                case "crosshairDot":        crosshairDot       = parseBool(val); break;
                case "crosshairRainbow":    crosshairRainbow   = parseBool(val); break;

                // CPS
                case "cpsMinCPS":           cpsMinCPS = Integer.parseInt(val); break;
                case "cpsMidCPS":           cpsMidCPS = Integer.parseInt(val); break;
                case "cpsHighCPS":          cpsHighCPS = Integer.parseInt(val); break;
                case "cpsVeryHighCPS":      cpsVeryHighCPS = Integer.parseInt(val); break;
                case "cpsColorLow":         cpsColorLow = (int) Long.parseLong(val); break;
                case "cpsColorMedium":      cpsColorMedium = (int) Long.parseLong(val); break;
                case "cpsColorHigh":        cpsColorHigh = (int) Long.parseLong(val); break;
                case "cpsColorVeryHigh":    cpsColorVeryHigh = (int) Long.parseLong(val); break;
                case "cpsShowLeft":         cpsShowLeft = parseBool(val); break;
                case "cpsShowRight":        cpsShowRight = parseBool(val); break;
                case "cpsShowTotal":        cpsShowTotal = parseBool(val); break;
                case "cpsShowBackground":   cpsShowBackground = parseBool(val); break;

                // Camera
                case "cameraMods":             cameraModsEnabled = parseBool(val); break;
                case "shakeDisableHurtCam":    shakeDisableHurtCam = parseBool(val); break;
                case "shakeMinimalizeBobbing": shakeMinimalizeBobbing = parseBool(val); break;

                // Nametag
                case "ShowNametagEnabled":  ShowNametagEnabled = parseBool(val); break;

                // TNT Timer
                case "tntTimerEnabled":         tntTimerEnabled = parseBool(val); break;
                case "tntTimerShowProgress":    tntTimerShowProgress = parseBool(val); break;
                case "tntTimerShowSeconds":     tntTimerShowSeconds = parseBool(val); break;
                case "tntTimerPosition":        tntTimerPosition = Integer.parseInt(val); break;

                // Hitbox
                case "hitboxRed":           hitboxRed = Float.parseFloat(val); break;
                case "hitboxGreen":         hitboxGreen = Float.parseFloat(val); break;
                case "hitboxBlue":          hitboxBlue = Float.parseFloat(val); break;
                case "hitboxAlpha":         hitboxAlpha = Float.parseFloat(val); break;
                case "hitboxLineThickness": hitboxLineThickness = Integer.parseInt(val); break;
                case "hitboxUseCustomColor": hitboxUseCustomColor = parseBool(val); break;
                case "hitboxShowEyeHeight": hitboxShowEyeHeight = parseBool(val); break;
                case "hitboxShowLookVector": hitboxShowLookVector = parseBool(val); break;
                case "fpsX": fpsX = Integer.parseInt(val); break;
                case "fpsY": fpsY = Integer.parseInt(val); break;
                case "cpsX": cpsX = Integer.parseInt(val); break;
                case "cpsY": cpsY = Integer.parseInt(val); break;
                case "fpsScale": fpsScale = clampF(Float.parseFloat(val), 0.5F, 4.0F); break;
                case "cpsScale": cpsScale = clampF(Float.parseFloat(val), 0.5F, 4.0F); break;
                case "nelianFont":          nelianFontEnabled = parseBool(val); break;
            }
        } catch (NumberFormatException ignored) {}
    }
    public static boolean isNelianFontEnabled() {
        return nelianFontEnabled;
    }

    public static void setNelianFontEnabled(boolean enabled) {
        nelianFontEnabled = enabled;
    }

    private static int  b(boolean v)        { return v ? 1 : 0; }
    private static boolean parseBool(String v) { return "1".equals(v) || "true".equalsIgnoreCase(v); }
    private static int  clampI(int v, int mn, int mx) { return Math.max(mn, Math.min(mx, v)); }


    public static int getCrosshairARGB() {
        int rgb = crosshairColor & 0x00FFFFFF;
        return (crosshairAlpha << 24) | rgb;
    }
    private static float clampF(float v, float mn, float mx) { return Math.max(mn, Math.min(mx, v)); }

    public static void setCrosshairRGB(int r, int g, int b) {
        crosshairColor = (crosshairColor & 0xFF000000)
                | ((r & 0xFF) << 16)
                | ((g & 0xFF) << 8)
                | (b & 0xFF);
    }

    public static int getCrosshairR() { return (crosshairColor >> 16) & 0xFF; }
    public static int getCrosshairG() { return (crosshairColor >>  8) & 0xFF; }
    public static int getCrosshairB() { return  crosshairColor        & 0xFF; }
}
