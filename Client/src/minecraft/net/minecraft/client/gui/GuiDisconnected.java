package net.minecraft.client.gui;

import java.io.IOException;
import java.util.List;
import java.awt.Color;

import net.minecraft.client.Minecraft;
import net.minecraft.client.multiplayer.GuiConnecting;
import net.minecraft.client.multiplayer.ServerData;
import net.minecraft.client.renderer.GlStateManager;
import net.minecraft.client.renderer.Tessellator;
import net.minecraft.client.renderer.WorldRenderer;
import net.minecraft.client.renderer.vertex.DefaultVertexFormats;
import net.minecraft.client.resources.I18n;
import net.minecraft.util.IChatComponent;
import net.minecraft.util.ResourceLocation;

public class GuiDisconnected extends GuiScreen
{
    private String reason;
    private IChatComponent message;
    private List<String> multilineMessage;
    private final GuiScreen parentScreen;
    private int field_175353_i;
    private float fadeProgress = 0;
    private float dotProgress = 0;

    public GuiDisconnected(GuiScreen screen, String reasonLocalizationKey, IChatComponent chatComp)
    {
        this.parentScreen = screen;
        this.reason = I18n.format(reasonLocalizationKey, new Object[0]);
        this.message = chatComp;
    }

    protected void keyTyped(char typedChar, int keyCode) throws IOException
    {
    }

    public void initGui()
    {
        this.fadeProgress = 0;
        this.buttonList.clear();
        this.multilineMessage = this.fontRendererObj.listFormattedStringToWidth(this.message.getFormattedText(), this.width - 50);
        this.field_175353_i = this.multilineMessage.size() * this.fontRendererObj.FONT_HEIGHT;
        
        ModernDisconnectButton menuButton = new ModernDisconnectButton(0, 
            this.width / 2 - 100,
            this.height / 2 + this.field_175353_i / 2 + this.fontRendererObj.FONT_HEIGHT,
            200, 20,
            I18n.format("gui.toMenu", new Object[0]));
        menuButton.setPrimaryMode(false);
        this.buttonList.add(menuButton);
        
        ModernDisconnectButton retryButton = new ModernDisconnectButton(1,
            this.width / 2 - 100,
            this.height / 2 + this.field_175353_i / 2 + this.fontRendererObj.FONT_HEIGHT + 28,
            200, 20,
            I18n.format("Try Again", new Object[0]));
        retryButton.setPrimaryMode(true);
        this.buttonList.add(retryButton);
    }
    
    public void updateScreen()
    {
        super.updateScreen();
        if (fadeProgress < 1.0f) {
            fadeProgress += 0.05f;
            if (fadeProgress > 1.0f) fadeProgress = 1.0f;
        }
        
        dotProgress += 0.05f;
        if (dotProgress > 1.0f) dotProgress = 0.0f;
    }

    protected void actionPerformed(GuiButton button) throws IOException
    {
        if (button.id == 0)
        {
            this.mc.displayGuiScreen(this.parentScreen);
            return;
        }

        if (button.id == 1)
        {
            ServerData server = this.mc.getCurrentServerData();

            if (server != null)
            {
                this.mc.displayGuiScreen(
                    new GuiConnecting(
                        this.parentScreen,
                        this.mc,
                        server
                    )
                );
            }
        }
    }

    public void drawScreen(int mouseX, int mouseY, float partialTicks)
    {
    	if (this.mc.theWorld != null)
    	{
    	    this.drawGradientRect(
    	            0,
    	            0,
    	            this.width,
    	            this.height,
    	            -1072689136,
    	            -804253680
    	    );
    	}
    	else
    	{
    	    if (Nelianoptions.IsCustomLoadingArtEnabled)
    	    {
    	        this.mc.getTextureManager().bindTexture(
    	                new ResourceLocation("minecraft", "textures/gui/Loading_background.png")
    	        );

    	        GlStateManager.color(1.0F, 1.0F, 1.0F, 1.0F);

    	        Tessellator tessellator = Tessellator.getInstance();
    	        WorldRenderer worldrenderer = tessellator.getWorldRenderer();

    	        worldrenderer.begin(7, DefaultVertexFormats.POSITION_TEX);

    	        worldrenderer.pos(0.0D, this.height, 0.0D)
    	                .tex(0.0D, 1.0D)
    	                .endVertex();

    	        worldrenderer.pos(this.width, this.height, 0.0D)
    	                .tex(1.0D, 1.0D)
    	                .endVertex();

    	        worldrenderer.pos(this.width, 0.0D, 0.0D)
    	                .tex(1.0D, 0.0D)
    	                .endVertex();

    	        worldrenderer.pos(0.0D, 0.0D, 0.0D)
    	                .tex(0.0D, 0.0D)
    	                .endVertex();

    	        tessellator.draw();

    	        GlStateManager.disableTexture2D();
    	        GlStateManager.enableBlend();

    	        worldrenderer.begin(7, DefaultVertexFormats.POSITION_COLOR);

    	        worldrenderer.pos(0.0D, this.height, 0.0D)
    	                .color(0, 0, 0, 80)
    	                .endVertex();

    	        worldrenderer.pos(this.width, this.height, 0.0D)
    	                .color(0, 0, 0, 80)
    	                .endVertex();

    	        worldrenderer.pos(this.width, 0.0D, 0.0D)
    	                .color(0, 0, 0, 80)
    	                .endVertex();

    	        worldrenderer.pos(0.0D, 0.0D, 0.0D)
    	                .color(0, 0, 0, 80)
    	                .endVertex();

    	        tessellator.draw();

    	        GlStateManager.enableTexture2D();
    	        GlStateManager.disableBlend();
    	    }
    	    else
    	    {
    	        GlobalMenuBackground.get().render(this, this.width, this.height);
    	    }
    	}
        
        drawRect(0, 0, this.width, this.height, new Color(0, 0, 0, 120).getRGB());
        
        drawErrorIcon();
        
        drawModernTitle();
        
        drawDecorativeLine();
        
        for (Object obj : this.buttonList) {
            if (obj instanceof ModernDisconnectButton) {
                ModernDisconnectButton btn = (ModernDisconnectButton) obj;
                boolean isHovered = mouseX >= btn.xPosition && mouseY >= btn.yPosition && 
                                   mouseX < btn.xPosition + btn.getButtonWidth() && 
                                   mouseY < btn.yPosition + btn.getButtonHeight();
                btn.setHovered(isHovered);
            }
        }
        
        drawMessage();
        
        drawCornerDecorations();
        
        super.drawScreen(mouseX, mouseY, partialTicks);
    }
    private void drawCircle(int x, int y, int width, int height, Color color) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int dx = i - width/2;
                int dy = j - height/2;
                if (dx*dx + dy*dy <= (width/2)*(width/2)) {
                    drawRect(x + i, y + j, x + i + 1, y + j + 1, color.getRGB());
                }
            }
        }
    }
    private void drawErrorIcon() {
        int centerX = this.width / 2;
        int iconY = 40;
        int iconSize = 45;
        
        drawCircle(centerX - iconSize/2, iconY, iconSize, iconSize, new Color(200, 60, 50, 200));
        
        drawRect(centerX - 3, iconY + 12, centerX + 3, iconY + 26, new Color(255, 255, 255, 220).getRGB());
        
        drawRect(centerX - 3, iconY + 32, centerX + 3, iconY + 36, new Color(255, 255, 255, 220).getRGB());
    }

    private void drawModernTitle() {
        String title = this.reason;
        int titleWidth = this.fontRendererObj.getStringWidth(title);
        int titleX = this.width / 2 - titleWidth / 2;
        int titleY = 100;
        
        this.fontRendererObj.drawString(title, titleX + 2, titleY + 2, new Color(0, 0, 0, 100).getRGB());
        
        for (int i = 0; i < title.length(); i++) {
            String letter = String.valueOf(title.charAt(i));
            int xPos = titleX + this.fontRendererObj.getStringWidth(title.substring(0, i));
            float progress = (float)i / title.length();
            int r = (int)(255 * (0.7 + 0.3 * Math.sin(progress * Math.PI)));
            int g = (int)(100 * (0.6 + 0.4 * Math.cos(progress * Math.PI)));
            int b = (int)(80 * (0.5 + 0.5 * Math.sin(progress * Math.PI)));
            int color = new Color(r, g, b).getRGB();
            this.fontRendererObj.drawString(letter, xPos, titleY, color);
        }
    }

    private void drawDecorativeLine() {
        int lineWidth = 200;
        int lineY = 125;
        int lineStartX = this.width / 2 - lineWidth / 2;
        
        for (int i = 0; i < lineWidth; i++) {
            float progress = (float)i / lineWidth;
            int alpha = (int)(100 * Math.sin(progress * Math.PI));
            int color = new Color(200, 80, 70, alpha).getRGB();
            drawRect(lineStartX + i, lineY, lineStartX + i + 1, lineY + 2, color);
        }
    }

    private void drawMessage() {
        int i = this.height / 2 - this.field_175353_i / 2 - 30;
        
        if (this.multilineMessage != null)
        {
            for (String s : this.multilineMessage)
            {
                this.fontRendererObj.drawString(s, this.width / 2 - this.fontRendererObj.getStringWidth(s) / 2 + 1, i + 1, new Color(0, 0, 0, 100).getRGB());
                this.fontRendererObj.drawString(s, this.width / 2 - this.fontRendererObj.getStringWidth(s) / 2, i, new Color(220, 220, 230).getRGB());
                i += this.fontRendererObj.FONT_HEIGHT;
            }
        }
        
        int statusY = i + 15;
        int dotCount = (int)(dotProgress * 6) % 4;
        String dots = "";
        for (int d = 0; d < dotCount; d++) {
            dots += ".";
        }
        
        String statusText = "Connection Failed" + dots;
        this.fontRendererObj.drawString(statusText, 
            this.width / 2 - this.fontRendererObj.getStringWidth(statusText) / 2, 
            statusY, 
            new Color(150, 150, 170).getRGB());
    }
    
    private void drawCornerDecorations() {
        int cornerSize = 40;
        int cornerThickness = 2;
        Color cornerColor = new Color(200, 80, 70, 60);
        
        drawRect(0, 0, cornerSize, cornerThickness, cornerColor.getRGB());
        drawRect(0, 0, cornerThickness, cornerSize, cornerColor.getRGB());
        
        drawRect(this.width - cornerSize, 0, this.width, cornerThickness, cornerColor.getRGB());
        drawRect(this.width - cornerThickness, 0, this.width, cornerSize, cornerColor.getRGB());
        
        drawRect(0, this.height - cornerThickness, cornerSize, this.height, cornerColor.getRGB());
        drawRect(0, this.height - cornerSize, cornerThickness, this.height, cornerColor.getRGB());
        
        drawRect(this.width - cornerSize, this.height - cornerThickness, this.width, this.height, cornerColor.getRGB());
        drawRect(this.width - cornerThickness, this.height - cornerSize, this.width, this.height, cornerColor.getRGB());
    }

    class ModernDisconnectButton extends GuiButton {
        private boolean isHovered = false;
        private float hoverProgress = 0;
        private boolean isPrimary = false;
        
        public ModernDisconnectButton(int buttonId, int x, int y, int widthIn, int heightIn, String buttonText) {
            super(buttonId, x, y, widthIn, heightIn, buttonText);
        }
        
        public int getButtonWidth() {
            return this.width;
        }
        
        public int getButtonHeight() {
            return this.height;
        }
        
        public void setPrimaryMode(boolean primary) {
            this.isPrimary = primary;
        }
        
        public void setHovered(boolean hovered) {
            if (hovered != this.isHovered) {
                this.isHovered = hovered;
            }
            
            if (hovered && hoverProgress < 1.0f) {
                hoverProgress += 0.1f;
                if (hoverProgress > 1.0f) hoverProgress = 1.0f;
            } else if (!hovered && hoverProgress > 0.0f) {
                hoverProgress -= 0.1f;
                if (hoverProgress < 0.0f) hoverProgress = 0.0f;
            }
        }
        
        @Override
        public void drawButton(Minecraft mc, int mouseX, int mouseY) {
            if (this.visible) {
                Color bgColor, borderColor, textColor;
                
                if (!this.enabled) {
                    bgColor = new Color(60, 60, 70, 150);
                    borderColor = new Color(80, 80, 90, 200);
                    textColor = new Color(120, 120, 130);
                } else if (isPrimary) {
                    bgColor = new Color(50, 180, 80, (int)(200 + 55 * hoverProgress));
                    borderColor = new Color(80, 220, 110);
                    textColor = Color.WHITE;
                } else {
                    bgColor = new Color(30, 30, 40, (int)(180 + 75 * hoverProgress));
                    borderColor = new Color(100, 100, 120);
                    textColor = new Color(220, 220, 230);
                }
                
                drawRect(this.xPosition, this.yPosition, this.xPosition + this.width, this.yPosition + this.height, bgColor.getRGB());
                drawHorizontalLine(this.xPosition, this.xPosition + this.width, this.yPosition, borderColor.getRGB());
                drawHorizontalLine(this.xPosition, this.xPosition + this.width, this.yPosition + this.height - 1, borderColor.getRGB());
                drawVerticalLine(this.xPosition, this.yPosition, this.yPosition + this.height, borderColor.getRGB());
                drawVerticalLine(this.xPosition + this.width - 1, this.yPosition, this.yPosition + this.height, borderColor.getRGB());
                
                if (isHovered && enabled && isPrimary) {
                    int shimmerX = this.xPosition + (int)((System.currentTimeMillis() / 5) % (this.width + 50)) - 50;
                    for (int i = 0; i < 20; i++) {
                        int alpha = (int)(30 * (1.0f - Math.abs(i - 10) / 10.0f));
                        drawRect(shimmerX + i, this.yPosition, shimmerX + i + 1, this.yPosition + this.height, 
                            new Color(255, 255, 255, alpha).getRGB());
                    }
                }
                
                int textWidth = mc.fontRendererObj.getStringWidth(this.displayString);
                int textX = this.xPosition + (this.width - textWidth) / 2;
                int textY = this.yPosition + (this.height - 8) / 2;
                
                mc.fontRendererObj.drawString(this.displayString, textX + 1, textY + 1, new Color(0, 0, 0, 100).getRGB());
                mc.fontRendererObj.drawString(this.displayString, textX, textY, textColor.getRGB());
            }
        }
    }
}
