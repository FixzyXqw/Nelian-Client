package net.minecraft.client.Nelian;

public class Nelianinfo {
    public static boolean yes = true;
    public static boolean no = false;
    
    public static boolean isBeta = yes;
    public static boolean isPre = no;
    public static boolean isRelease = no;
    // This is a dev-ish tool i use to set the versions on everwhere instead of settin it manually on any class
   // could be better but it works so
  // also writing true or false is boring so i set them as yes & no, i know LOL :D
    public static final String crtRENDER = "Renderer VENOM v1";
    private static final String BLD = "1";
    private static final String VER = "1";
    public static final String VERSION = "Nelian v" + VER;
    public static final String BUILD = "Nelian Build " + BLD;
    public static final String ALL = "Nelian v" + VER + ", Build" + BLD;

    public static final String VERSION_NUMBER = VER;
    public static final String BUILD_NUMBER = BLD;

    public static void CheckBeta() {
        if (isBeta == true && isPre == true) {
            isBeta = false;
        } else if (isBeta == true) {
            isPre = false;
        } else if (isPre == true) {
            isBeta = false;
        }
        

        isRelease = (!isBeta && !isPre);
    }
    public static String getVersion() {
        return VERSION;
    }
    
    public static String getBuild() {
        return BUILD;
    }
    
    public static String getAll() {
        return ALL;
    }

    public static String getVersionNumber() {
        return VERSION_NUMBER;
    }
    
    public static String getBuildNumber() {
        return BUILD_NUMBER;
    }
    
    public String version() {
        return VERSION;
    }
    
    public String build() {
        return BUILD;
    }
    
    public String all() {
        return ALL;
    }
    
    @Override
    public String toString() {
        return ALL;
    }
}
