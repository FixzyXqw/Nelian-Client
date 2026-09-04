package net.minecraft.client.Nelian.mods;

import net.minecraft.client.Minecraft;
import net.minecraft.client.Nelian.Nelianoptions;
import java.util.regex.Pattern;

public class AutoGG {

    private static final Pattern WIN_PATTERN = Pattern.compile(
        "^(1st killer[\\s:-]+|1st place[\\s:-]+|winner[\\s:-]+).+|.+ won the game$"
    );

    public void onChat(String message) {
        Minecraft mc = Minecraft.getMinecraft();
        if (!Nelianoptions.autoGGEnabled || mc == null || mc.thePlayer == null) {
            return;
        }

        String text = message
                .replaceAll("§[0-9a-fk-or]", "")
                .toLowerCase()
                .trim();

        if (WIN_PATTERN.matcher(text).matches()) {
            mc.thePlayer.sendChatMessage("GG");
        }
    }

    public boolean isEnabled() {
        return Nelianoptions.autoGGEnabled;
    }

    public void setEnabled(boolean enabled) {
        Nelianoptions.autoGGEnabled = enabled;
    }
}
