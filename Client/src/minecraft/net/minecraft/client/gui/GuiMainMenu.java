package net.minecraft.client.gui;

import com.google.common.base.Strings;
import com.google.common.collect.Lists;

import java.awt.Color;
import java.io.IOException;
import java.net.URI;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.atomic.AtomicInteger;

import net.minecraft.client.Minecraft;
import net.minecraft.client.renderer.GlStateManager;
import net.minecraft.client.renderer.OpenGlHelper;
import net.minecraft.client.renderer.Tessellator;
import net.minecraft.client.renderer.WorldRenderer;
import net.minecraft.client.renderer.texture.DynamicTexture;
import net.minecraft.client.renderer.vertex.DefaultVertexFormats;
import net.minecraft.client.resources.I18n;
import net.minecraft.client.settings.GameSettings;
import net.minecraft.util.EnumChatFormatting;
import net.minecraft.util.ResourceLocation;
import net.optifine.reflect.Reflector;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.lwjgl.opengl.GL11;
import org.lwjgl.opengl.GLContext;

public class GuiMainMenu extends GuiScreen implements GuiYesNoCallback {

    private static final AtomicInteger field_175373_f = new AtomicInteger(0);
    private static final Logger logger = LogManager.getLogger();
    private static final Random RANDOM = new Random();

    private float updateCounter;
    private String splashText;
    private float globeRotation = 0F;
    private int panoramaTimer;
    private DynamicTexture viewportTexture;
    private boolean field_175375_v = true;
    private final Object threadLock = new Object();
    private String openGLWarning1;
    private String openGLWarning2;
    private String openGLWarningLink;
    public static final String field_96138_a = "Please click " + EnumChatFormatting.UNDERLINE + "here" + EnumChatFormatting.RESET + " for more information.";

    private int field_92022_t;
    private int field_92021_u;
    private int field_92020_v;
    private int field_92019_w;
    private int field_92024_r;
    private ResourceLocation backgroundTexture;
    private boolean field_183502_L;
    private GuiScreen field_183503_M;
    private GuiButton modButton;
    private GuiScreen modUpdateNotification;

    private List<NelianInterface.Snowflake> snowflakes = new ArrayList<>();
    private List<NelianInterface.MouseTrailPoint> mouseTrail = new ArrayList<>();
    private int lastMouseX = 0;
    private int lastMouseY = 0;

    private ResourceLocation nelianTexture;
    private int nelianTextureWidth;
    private int nelianTextureHeight;
    private int nelianTextureX;
    private int nelianTextureY;
    private boolean nelianTextureCreated = false;

    private AccountPanel accountPanel;


 
    private static class GuiButtonModern extends NelianInterface.ModernButton {

        public GuiButtonModern(int buttonId, int x, int y, int widthIn, int heightIn, String buttonText) {
            super(buttonId, x, y, widthIn, heightIn, buttonText);
        }

        @Override
        public void drawButton(Minecraft mc, int mouseX, int mouseY) {
            if (!this.visible) return;

            boolean isHovered = mouseX >= this.xPosition && mouseY >= this.yPosition &&
                    mouseX < this.xPosition + this.width && mouseY < this.yPosition + this.height;

            this.setHovered(isHovered);

            super.drawButton(mc, mouseX, mouseY);

            if (!this.enabled) {
                drawRect(
                        this.xPosition,
                        this.yPosition,
                        this.xPosition + this.width,
                        this.yPosition + this.height,
                        0x66000000
                );
            }
        }
    }
    public GuiMainMenu() {
        this.openGLWarning2 = field_96138_a;
        this.field_183502_L = false;
        this.splashText = "";
        this.updateCounter = RANDOM.nextFloat();
        this.openGLWarning1 = "";

        for (int i = 0; i < 120; i++) {
            snowflakes.add(new NelianInterface.Snowflake(
                    RANDOM.nextFloat() * 1920,
                    RANDOM.nextFloat() * 1080,
                    0.4f + RANDOM.nextFloat() * 1.2f,
                    1 + RANDOM.nextInt(2)
            ));
        }

        if (!GLContext.getCapabilities().OpenGL20 && !OpenGlHelper.areShadersSupported()) {
            this.openGLWarning1 = I18n.format("title.oldgl1");
            this.openGLWarning2 = I18n.format("title.oldgl2");
            this.openGLWarningLink = "https://help.mojang.com/customer/portal/articles/325948?ref=game";
        }
    }

    private boolean func_183501_a() {
        return Minecraft.getMinecraft().gameSettings.getOptionOrdinalValue(GameSettings.Options.REALMS_NOTIFICATIONS)
                && this.field_183503_M != null;
    }

    @Override
    public void updateScreen() {
        globeRotation += 0.5F;
        ++this.panoramaTimer;

        for (NelianInterface.Snowflake s : snowflakes) {
            s.update(this.width, this.height);
        }

        long now = System.currentTimeMillis();
        mouseTrail.removeIf(p -> now - p.life > 400);

        if (this.func_183501_a()) this.field_183503_M.updateScreen();
    }

    @Override
    public boolean doesGuiPauseGame() {
        return false;
    }

    @Override
    protected void keyTyped(char typedChar, int keyCode) throws IOException {
    }


    @Override
    public void initGui() {
        if (accountPanel == null) {
            accountPanel = new AccountPanel(8, 8);
        }

        this.viewportTexture = new DynamicTexture(256, 256);
        this.backgroundTexture = this.mc.getTextureManager().getDynamicTextureLocation("background", this.viewportTexture);

        if (!nelianTextureCreated) {
            createNelianTexture();
        }

        this.buttonList.clear();

        int bWidth = 140;
        int bHeight = 26;
        int spacing = 8;
        int startX = this.width / 2 - bWidth - (spacing / 2);
        int startY = this.height / 2 - 20;

        this.buttonList.add(new GuiButtonModern(1, startX, startY, bWidth, bHeight, I18n.format("menu.singleplayer")));
        this.buttonList.add(new GuiButtonModern(3, startX, startY + (bHeight + spacing), bWidth, bHeight, "Alt Manager"));
        GuiButtonModern cosmeticsButton = new GuiButtonModern(5, startX, startY + (bHeight + spacing) * 2, bWidth, bHeight,"Cosmetics");
        cosmeticsButton.enabled = false;
        this.buttonList.add(cosmeticsButton);

        int rightX = this.width / 2 + (spacing / 2);
        this.buttonList.add(new GuiButtonModern(2, rightX, startY, bWidth, bHeight, I18n.format("menu.multiplayer")));
        this.buttonList.add(new GuiButtonModern(4, rightX, startY + (bHeight + spacing), bWidth, bHeight, "Mods"));
        this.buttonList.add(new GuiButtonModern(6, rightX, startY + (bHeight + spacing) * 2, bWidth, bHeight, I18n.format("menu.options")));

        this.buttonList.add(new GuiButtonModern(7, startX, startY + (bHeight + spacing) * 3, (bWidth * 2) + spacing, bHeight, I18n.format("menu.quit")));

        nelianTextureX = (this.width - nelianTextureWidth) / 2;
        nelianTextureY = 45;

        this.mc.setConnectedToRealms(false);
    }


    @Override
    protected void actionPerformed(GuiButton button) throws IOException {
        switch (button.id) {
            case 1:
                this.mc.displayGuiScreen(new GuiSelectWorld(this));
                break;
            case 2:
                this.mc.displayGuiScreen(new GuiMultiplayer(this));
                break;
            case 3:
            	mc.displayGuiScreen(new GuiOfflineManager(this));
                break;
            case 4:
                this.mc.displayGuiScreen(new GuiNelianOverlay());
                break;
            case 5:
                break;
            case 6:
                this.mc.displayGuiScreen(new GuiOptions(this, this.mc.gameSettings));
                break;
            case 7:
                this.mc.shutdown();
                break;
        }
    }

    @Override
    public void confirmClicked(boolean result, int id) {
        if (result && id == 12) {
            this.mc.getSaveLoader().deleteWorldDirectory("Demo_World");
            this.mc.displayGuiScreen(this);
        } else if (id == 13 && result) {
            try {
                Class<?> oclass = Class.forName("java.awt.Desktop");
                Object desktop = oclass.getMethod("getDesktop").invoke(null);
                oclass.getMethod("browse", URI.class).invoke(desktop, new URI(this.openGLWarningLink));
            } catch (Throwable t) {
                logger.error("Couldn't open link", t);
            }
            this.mc.displayGuiScreen(this);
        }
    }


    private void renderSkybox(int mouseX, int mouseY, float partialTicks) {
        drawGradientRect(0, 0, this.width, this.height, 0xFF0B0C10, 0xFF14161D);
        drawGradientRect(0, 0, this.width, this.height, 0x1A000000, 0x66000000);
    }


    public void createNelianTexture() {
        int pixelSize = 7;
        int spacing = 5;
        nelianTexture = NelianInterface.createNelianTexture(this.mc, pixelSize, spacing);
        int totalWidth = 6 * (5 * pixelSize + spacing) - spacing;
        int totalHeight = 5 * pixelSize;
        nelianTextureWidth = totalWidth;
        nelianTextureHeight = totalHeight;
        nelianTextureCreated = true;
    }

    public boolean isNelianTextureCreated() {
        return nelianTextureCreated;
    }

    private void drawNelianTitle() {
        drawNelianTitleWithAlpha(1.0F);
    }

    public void drawNelianTitleWithAlpha(float alpha) {
        NelianInterface.drawNelianTitle(
            this.mc, 
            nelianTexture, 
            nelianTextureX, 
            nelianTextureY, 
            nelianTextureWidth, 
            nelianTextureHeight, 
            alpha
        );
    }


    private void drawSnowflakes() {
        NelianInterface.drawSnowflakes(snowflakes, this.height);
    }

    private void drawMouseTrail() {
        NelianInterface.drawMouseTrail(mouseTrail);
    }


    @Override
    public void drawScreen(int mouseX, int mouseY, float partialTicks) {
        long now = System.currentTimeMillis();
        if (mouseX != lastMouseX || mouseY != lastMouseY) {
            mouseTrail.add(new NelianInterface.MouseTrailPoint(mouseX, mouseY, now));
            lastMouseX = mouseX;
            lastMouseY = mouseY;
        }

        Nelianinfo.CheckBeta();

        GlStateManager.disableAlpha();
        this.renderSkybox(mouseX, mouseY, partialTicks);
        GlStateManager.enableAlpha();

        if (this.mc.theWorld != null) {
            this.drawGradientRect(0, 0, this.width, this.height, -1072689136, -804253680);
        } else {
            GlobalMenuBackground.get().render(this, this.width, this.height);
        }

        this.drawSnowflakes();
        this.drawMouseTrail();

        int bWidth = 140;
        int bHeight = 26;
        int spacing = 8;
        int startX = this.width / 2 - bWidth - (spacing / 2);
        int startY = this.height / 2 - 20;
        int totalHeight = (bHeight + spacing) * 3 + bHeight;
        int panelLeft   = startX - 25;
        int panelRight  = startX + (bWidth * 2) + spacing + 25;
        int panelTop    = startY - 25;
        int panelBottom = startY + totalHeight + 25;
        int panelColor  = new Color(10, 10, 12, 160).getRGB();
        
        GlStateManager.disableCull();
        
        NelianInterface.drawRoundedRect(panelLeft, panelTop, panelRight, panelBottom, 14, panelColor);
        int outlineColor = new Color(63, 63, 70, 90).getRGB();
        NelianInterface.drawRoundedRectBorder(panelLeft, panelTop, panelRight, panelBottom, 14, outlineColor, 1.5f);
        

        if (accountPanel != null) {
            accountPanel.draw(mc, mouseX, mouseY);
        }

        this.drawNelianTitle();

        int infoColor = 0x44FFFFFF;
        int infoPre   = 0x44AA0000;

        int rightX = this.width - 10;
        int bottomY = this.height - 10;
        int lineHeight = this.fontRendererObj.FONT_HEIGHT + 2;

        int grayColor = 0xFFA0AAB2;

        String nelianVer = Nelianinfo.crtRENDER;
        int nelianY = bottomY - lineHeight * 4 + 7;
        this.drawString(this.fontRendererObj, nelianVer,
                rightX - this.fontRendererObj.getStringWidth(nelianVer),
                nelianY, grayColor);

        String mcVer = "Minecraft 1.8.9";
        int mcY = bottomY - lineHeight * 3 + 7;
        this.drawString(this.fontRendererObj, mcVer,
                rightX - this.fontRendererObj.getStringWidth(mcVer),
                mcY, grayColor);

        String javaVer = "Java " + System.getProperty("java.version");
        int javaY = bottomY - lineHeight * 2 + 7;
        this.drawString(this.fontRendererObj, javaVer,
                rightX - this.fontRendererObj.getStringWidth(javaVer),
                javaY, grayColor);

        String copyright = "Has nothing to do with Mojang!";
        int copyrightY = bottomY - lineHeight * 1;
        this.drawString(this.fontRendererObj, copyright,
                rightX - this.fontRendererObj.getStringWidth(copyright),
                this.height - 12, 0xFFFFFFFF);

        int infoPre2 = 0xFFFF5555;
        if (Nelianinfo.isPre) {
            String preLine = "Pre-release " + Nelianinfo.ALL;
            this.drawString(this.fontRendererObj, preLine,
                    8, this.height - 12, infoPre2);
        } else if (Nelianinfo.isBeta) {
            String betaLine = "BETA " + Nelianinfo.ALL;
            this.drawString(this.fontRendererObj, betaLine,
                    8, this.height - 12, infoPre2);
        }

        if (this.openGLWarning1 != null && !this.openGLWarning1.isEmpty()) {
            drawRect(this.field_92022_t - 2, this.field_92021_u - 2,
                    this.field_92020_v + 2, this.field_92019_w - 1, 0x55000000);
            this.drawString(this.fontRendererObj, this.openGLWarning1,
                    this.field_92022_t, this.field_92021_u, -1);
            this.drawString(this.fontRendererObj, this.openGLWarning2,
                    (this.width - this.field_92024_r) / 2,
                    ((GuiButton) this.buttonList.get(0)).yPosition - 12, -1);
        }

        super.drawScreen(mouseX, mouseY, partialTicks);

        if (this.func_183501_a()) this.field_183503_M.drawScreen(mouseX, mouseY, partialTicks);
        if (this.modUpdateNotification != null) this.modUpdateNotification.drawScreen(mouseX, mouseY, partialTicks);
    }

    @Override
    protected void mouseClicked(int mouseX, int mouseY, int mouseButton) throws IOException {
        if (accountPanel != null) {
            accountPanel.mouseClicked(mouseX, mouseY, mouseButton, mc);
        }
        super.mouseClicked(mouseX, mouseY, mouseButton);

        synchronized (this.threadLock) {
            if (this.openGLWarning1.length() > 0 &&
                    mouseX >= this.field_92022_t && mouseX <= this.field_92020_v &&
                    mouseY >= this.field_92021_u && mouseY <= this.field_92019_w) {
                GuiConfirmOpenLink gui = new GuiConfirmOpenLink(this, this.openGLWarningLink, 13, true);
                gui.disableSecurityWarning();
                this.mc.displayGuiScreen(gui);
            }
        }
        if (this.func_183501_a()) this.field_183503_M.mouseClicked(mouseX, mouseY, mouseButton);
    }

    @Override
    public void onGuiClosed() {
        if (this.field_183503_M != null) this.field_183503_M.onGuiClosed();
    }
}
