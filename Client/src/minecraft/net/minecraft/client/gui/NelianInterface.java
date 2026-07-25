package net.minecraft.client.gui;

import java.awt.Color;
import java.awt.Graphics2D;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

import net.minecraft.client.Minecraft;
import net.minecraft.client.renderer.GlStateManager;
import net.minecraft.client.renderer.Tessellator;
import net.minecraft.client.renderer.WorldRenderer;
import net.minecraft.client.renderer.texture.DynamicTexture;
import net.minecraft.client.renderer.vertex.DefaultVertexFormats;
import net.minecraft.util.ResourceLocation;

import org.lwjgl.opengl.GL11;

public class NelianInterface {

    public static void drawRoundedRect(float x, float y, float x2, float y2, float radius, int color) {
        if (((color >> 24) & 0xFF) == 0) return;
        
        float alpha = ((color >> 24) & 0xFF) / 255.0F;
        float red = ((color >> 16) & 0xFF) / 255.0F;
        float green = ((color >> 8) & 0xFF) / 255.0F;
        float blue = (color & 0xFF) / 255.0F;

        GlStateManager.pushMatrix();
        GlStateManager.enableBlend();
        GlStateManager.disableTexture2D();
        GlStateManager.tryBlendFuncSeparate(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA, GL11.GL_ONE, GL11.GL_ZERO);
        GlStateManager.color(red, green, blue, alpha);

        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer worldRenderer = tessellator.getWorldRenderer();
        worldRenderer.begin(GL11.GL_POLYGON, DefaultVertexFormats.POSITION);

        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(180 + (90.0 * i / 18.0));
            worldRenderer.pos(x + radius + Math.cos(angle) * radius, y + radius + Math.sin(angle) * radius, 0).endVertex();
        }
        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(270 + (90.0 * i / 18.0));
            worldRenderer.pos(x2 - radius + Math.cos(angle) * radius, y + radius + Math.sin(angle) * radius, 0).endVertex();
        }
        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(0 + (90.0 * i / 18.0));
            worldRenderer.pos(x2 - radius + Math.cos(angle) * radius, y2 - radius + Math.sin(angle) * radius, 0).endVertex();
        }
        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(90 + (90.0 * i / 18.0));
            worldRenderer.pos(x + radius + Math.cos(angle) * radius, y2 - radius + Math.sin(angle) * radius, 0).endVertex();
        }

        tessellator.draw();
        
        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
        GlStateManager.color(1.0F, 1.0F, 1.0F, 1.0F);
        GlStateManager.popMatrix();
    }

    public static void drawRoundedRectBorder(int x0, int y0, int x1, int y1, int radius, int color, float lineWidth) {
        drawRoundedRectBorder((float)x0, (float)y0, (float)x1, (float)y1, (float)radius, lineWidth, color);
    }

    public static void drawRoundedRectBorder(float x, float y, float x2, float y2, float radius, float lineWidth, int color) {
        if (((color >> 24) & 0xFF) == 0) return;
        
        float alpha = ((color >> 24) & 0xFF) / 255.0F;
        float red = ((color >> 16) & 0xFF) / 255.0F;
        float green = ((color >> 8) & 0xFF) / 255.0F;
        float blue = (color & 0xFF) / 255.0F;

        GlStateManager.pushMatrix();
        GlStateManager.enableBlend();
        GlStateManager.disableTexture2D();
        GlStateManager.tryBlendFuncSeparate(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA, GL11.GL_ONE, GL11.GL_ZERO);
        GlStateManager.color(red, green, blue, alpha);
        
        GL11.glEnable(GL11.GL_LINE_SMOOTH);
        GL11.glLineWidth(lineWidth);

        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer worldRenderer = tessellator.getWorldRenderer();
        worldRenderer.begin(GL11.GL_LINE_LOOP, DefaultVertexFormats.POSITION);

        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(180 + (90.0 * i / 18.0));
            worldRenderer.pos(x + radius + Math.cos(angle) * radius, y + radius + Math.sin(angle) * radius, 0).endVertex();
        }
        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(270 + (90.0 * i / 18.0));
            worldRenderer.pos(x2 - radius + Math.cos(angle) * radius, y + radius + Math.sin(angle) * radius, 0).endVertex();
        }
        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(0 + (90.0 * i / 18.0));
            worldRenderer.pos(x2 - radius + Math.cos(angle) * radius, y2 - radius + Math.sin(angle) * radius, 0).endVertex();
        }
        for (int i = 0; i <= 18; i++) {
            double angle = Math.toRadians(90 + (90.0 * i / 18.0));
            worldRenderer.pos(x + radius + Math.cos(angle) * radius, y2 - radius + Math.sin(angle) * radius, 0).endVertex();
        }

        tessellator.draw();

        GL11.glDisable(GL11.GL_LINE_SMOOTH);
        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
        GlStateManager.color(1.0F, 1.0F, 1.0F, 1.0F);
        GlStateManager.popMatrix();
    }

    public static void drawFilledCircle(int cx, int cy, int rad, int color) {
        drawFilledCircle((float)cx, (float)cy, (float)rad, color);
    }

    public static void drawFilledCircle(float cx, float cy, float radius, int color) {
        if (radius <= 0 || ((color >> 24) & 0xFF) == 0) return;
        
        float alpha = ((color >> 24) & 0xFF) / 255.0F;
        float red = ((color >> 16) & 0xFF) / 255.0F;
        float green = ((color >> 8) & 0xFF) / 255.0F;
        float blue = (color & 0xFF) / 255.0F;

        GlStateManager.pushMatrix();
        GlStateManager.enableBlend();
        GlStateManager.disableTexture2D();
        GlStateManager.tryBlendFuncSeparate(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA, GL11.GL_ONE, GL11.GL_ZERO);
        GlStateManager.color(red, green, blue, alpha);
        
        GL11.glEnable(GL11.GL_POLYGON_SMOOTH);

        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer worldRenderer = tessellator.getWorldRenderer();
        worldRenderer.begin(GL11.GL_POLYGON, DefaultVertexFormats.POSITION);

        int segments = (int) Math.max(30, radius * 3);
        for (int i = 0; i <= segments; i++) {
            double angle = Math.toRadians(360.0 * i / segments);
            worldRenderer.pos(cx + Math.cos(angle) * radius, cy + Math.sin(angle) * radius, 0).endVertex();
        }
        
        tessellator.draw();
        
        GL11.glDisable(GL11.GL_POLYGON_SMOOTH);
        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
        GlStateManager.color(1.0F, 1.0F, 1.0F, 1.0F);
        GlStateManager.popMatrix();
    }

    public static class ModernButton extends GuiButton {
        private boolean hovered = false;
        protected float hoverProgress = 0F;
        protected float pressProgress = 0F;
        private boolean primary = false;

        public ModernButton(int buttonId, int x, int y, int widthIn, int heightIn, String buttonText) {
            super(buttonId, x, y, widthIn, heightIn, buttonText);
        }

        public ModernButton(int buttonId, int x, int y, String buttonText) {
            super(buttonId, x, y, buttonText);
        }

        public void setPrimaryMode(boolean primary) {
            this.primary = primary;
        }

        public void setHovered(boolean hovered) {
            this.hovered = hovered;
        }

        @Override
        public void drawButton(Minecraft mc, int mouseX, int mouseY) {
            if (!this.visible) return;

            boolean isHoveredNow = mouseX >= this.xPosition && mouseX <= this.xPosition + this.width
                    && mouseY >= this.yPosition && mouseY <= this.yPosition + this.height && this.enabled;
            
            float hoverTarget = isHoveredNow ? 1.0F : 0.0F;
            hoverProgress += (hoverTarget - hoverProgress) * 0.25F;

            int baseA = (int) (170 + 50 * hoverProgress);
            int bg = new Color(32, 33, 39, baseA).getRGB();
            int border = new Color(90, 92, 100, (int) (150 + 60 * hoverProgress)).getRGB();

            drawRoundedRect(this.xPosition, this.yPosition, this.xPosition + this.width, this.yPosition + this.height, 6, bg);
            drawRoundedRectBorder(this.xPosition, this.yPosition, this.xPosition + this.width, this.yPosition + this.height, 6, border, 1.0F);

            int textWidth = mc.fontRendererObj.getStringWidth(this.displayString);
            int textX = this.xPosition + (this.width - textWidth) / 2;
            int textY = this.yPosition + (this.height - 8) / 2;

            mc.fontRendererObj.drawString(this.displayString, textX, textY + 1, new Color(0, 0, 0, 90).getRGB());
            mc.fontRendererObj.drawString(this.displayString, textX, textY, 0xFFE7E9EE);
        }
    }

    private static final int[][][] NELIAN_LETTERS = {
            {{1,0,0,0,1},{1,1,0,0,1},{1,0,1,0,1},{1,0,0,1,1},{1,0,0,0,1}},
            {{1,1,1,1,1},{1,0,0,0,0},{1,1,1,1,0},{1,0,0,0,0},{1,1,1,1,1}},
            {{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,1,1,1,1}},
            {{1,1,1,1,1},{0,0,1,0,0},{0,0,1,0,0},{0,0,1,0,0},{1,1,1,1,1}},
            {{0,1,1,1,0},{1,0,0,0,1},{1,1,1,1,1},{1,0,0,0,1},{1,0,0,0,1}},
            {{1,0,0,0,1},{1,1,0,0,1},{1,0,1,0,1},{1,0,0,1,1},{1,0,0,0,1}}
    };

    public static ResourceLocation createNelianTexture(Minecraft mc, int pixelSize, int letterSpacing) {
        int totalWidth = NELIAN_LETTERS.length * (5 * pixelSize + letterSpacing) - letterSpacing;
        int totalHeight = 5 * pixelSize;

        BufferedImage image = new BufferedImage(totalWidth, totalHeight, BufferedImage.TYPE_INT_ARGB);
        Graphics2D g2d = image.createGraphics();
        g2d.setColor(new Color(0, 0, 0, 0));
        g2d.fillRect(0, 0, totalWidth, totalHeight);
        g2d.setColor(Color.WHITE);

        int currentX = 0;
        for (int[][] letter : NELIAN_LETTERS) {
            for (int row = 0; row < 5; row++) {
                for (int col = 0; col < 5; col++) {
                    if (letter[row][col] == 1) {
                        int px = currentX + col * pixelSize;
                        int py = row * pixelSize;
                        g2d.fillRect(px, py, pixelSize, pixelSize);
                    }
                }
            }
            currentX += 5 * pixelSize + letterSpacing;
        }
        g2d.dispose();

        DynamicTexture dynamicTexture = new DynamicTexture(image);
        return mc.getTextureManager().getDynamicTextureLocation("nelian_title", dynamicTexture);
    }

    public static void drawNelianTitle(Minecraft mc, ResourceLocation texture, int x, int y, int width, int height, float alpha) {
        if (texture == null) return;
        GlStateManager.pushMatrix();
        GlStateManager.enableBlend();
        GlStateManager.tryBlendFuncSeparate(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA, 1, 0);
        GlStateManager.enableTexture2D();
        GlStateManager.color(1.0F, 1.0F, 1.0F, alpha);
        mc.getTextureManager().bindTexture(texture);

        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer wr = tessellator.getWorldRenderer();
        wr.begin(GL11.GL_QUADS, DefaultVertexFormats.POSITION_TEX);
        wr.pos(x, y + height, 0).tex(0, 1).endVertex();
        wr.pos(x + width, y + height, 0).tex(1, 1).endVertex();
        wr.pos(x + width, y, 0).tex(1, 0).endVertex();
        wr.pos(x, y, 0).tex(0, 0).endVertex();
        tessellator.draw();

        GlStateManager.color(1.0F, 1.0F, 1.0F, 1.0F);
        GlStateManager.disableBlend();
        GlStateManager.popMatrix();
    }

    public static class Snowflake {
        public float x, y;
        public float speed;
        public int size;

        public Snowflake(float x, float y, float speed, int size) {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.size = size;
        }

        public void update(int screenWidth, int screenHeight) {
            y += speed;
            if (y > screenHeight) {
                y = 0;
                x = (float) (Math.random() * screenWidth);
            }
        }
    }

    public static class MouseTrailPoint {
        public int x, y;
        public long life;

        public MouseTrailPoint(int x, int y, long life) {
            this.x = x;
            this.y = y;
            this.life = life;
        }
    }

    public static void drawSnowflakes(List<Snowflake> snowflakes, int screenHeight) {
        if (snowflakes == null || snowflakes.isEmpty()) return;
        GlStateManager.enableBlend();
        GlStateManager.disableTexture2D();
        GlStateManager.tryBlendFuncSeparate(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA, 1, 0);

        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer wr = tessellator.getWorldRenderer();

        for (Snowflake s : snowflakes) {
            float alpha = 0.08f + (s.y / screenHeight) * 0.18f;
            int a = (int) (alpha * 255);
            wr.begin(GL11.GL_QUADS, DefaultVertexFormats.POSITION_COLOR);
            wr.pos(s.x, s.y + s.size, 0).color(255, 255, 255, a).endVertex();
            wr.pos(s.x + s.size, s.y + s.size, 0).color(255, 255, 255, a).endVertex();
            wr.pos(s.x + s.size, s.y, 0).color(255, 255, 255, a).endVertex();
            wr.pos(s.x, s.y, 0).color(255, 255, 255, a).endVertex();
            tessellator.draw();
        }
        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
    }

    public static void drawMouseTrail(List<MouseTrailPoint> trail) {
        if (trail == null || trail.isEmpty()) return;
        GlStateManager.enableBlend();
        GlStateManager.disableTexture2D();
        GlStateManager.tryBlendFuncSeparate(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA, 1, 0);

        long now = System.currentTimeMillis();
        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer wr = tessellator.getWorldRenderer();

        for (MouseTrailPoint p : trail) {
            float age = (now - p.life) / 400f;
            if (age > 1.0f) continue;
            int alpha = (int) ((1 - age) * 110);
            int size = (int) (4 * (1 - age));
            wr.begin(GL11.GL_QUADS, DefaultVertexFormats.POSITION_COLOR);
            wr.pos(p.x - size / 2.0f, p.y - size / 2.0f, 0).color(110, 160, 255, alpha).endVertex();
            wr.pos(p.x + size / 2.0f, p.y - size / 2.0f, 0).color(110, 160, 255, alpha).endVertex();
            wr.pos(p.x + size / 2.0f, p.y + size / 2.0f, 0).color(110, 160, 255, alpha).endVertex();
            wr.pos(p.x - size / 2.0f, p.y + size / 2.0f, 0).color(110, 160, 255, alpha).endVertex();
            tessellator.draw();
        }
        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
    }
}
