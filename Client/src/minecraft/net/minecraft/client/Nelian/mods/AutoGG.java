package net.minecraft.client.Nelian.mods;

import net.minecraft.client.Minecraft;
import net.minecraft.client.Nelian.Nelianoptions;
import java.util.regex.Pattern;

public class AutoGG {
 //This class is pretty easy, you do not need any comments but anyways
    
    private static final Pattern WIN_PATTERN = Pattern.compile(
        "^(1st killer[\\s:-]+|1st place[\\s:-]+|winner[\\s:-]+).+|.+ won the game$" //these are the possible chat messages when the game ends
    );

    public void onChat(String message) {
        Minecraft mc = Minecraft.getMinecraft();
        if (!Nelianoptions.autoGGEnabled || mc == null || mc.thePlayer == null) { //if autogg is off or the player returns null, return
            return;
        }

        String text = message
                .replaceAll("§[0-9a-fk-or]", "")
                .toLowerCase()
                .trim(); //get the clear message

        if (WIN_PATTERN.matcher(text).matches()) {
            mc.thePlayer.sendChatMessage("GG");
        }
    }

    public boolean isEnabled() {
        return Nelianoptions.autoGGEnabled; //get toggle info from the options
    }

    public void setEnabled(boolean enabled) {
        Nelianoptions.autoGGEnabled = enabled; //set it enabled
    }
}
