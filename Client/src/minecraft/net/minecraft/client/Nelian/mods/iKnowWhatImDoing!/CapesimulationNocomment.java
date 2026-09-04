package net.minecraft.client.Nelian.mods;

import java.util.ArrayList;
import java.util.List;

public class Capesimulation {

    private static final float FIXED_DT = 0.05f;      
    public static final float SEGMENT_LENGTH = 1.0f;
    private static final int SOLVER_ITERATIONS = 3;  
    private static final float MAX_BEND_DEG = 30.0f;  
    private static final float REST_EASE = 0.25f;      
    private static final float ROOT_EASE = 0.35f;      
    private static final float VERTICAL_SCALE = 1.0f;  
    private static final float LEAN_GAIN = 3.0f;       

    public final List<Point> points = new ArrayList<Point>();
    public float gravity = 0.9f;

    public Capesimulation(int segmentCount) {
        for (int i = 0; i < segmentCount; i++) {
            Point p = new Point();
            p.y = i * SEGMENT_LENGTH;
            p.locked = (i == 0);
            points.add(p);
        }
    }
    public void applyMovement(float lean, float vertical) {
        int n = points.size();
        for (int i = 1; i < n; i++) {
            Point p = points.get(i);
            float weight = (float) i / (float) (n - 1);
            p.prevX = p.x;
            p.prevY = p.y;
            p.x += (lean * weight * LEAN_GAIN - p.x) * ROOT_EASE;
            p.y += vertical * weight * VERTICAL_SCALE;
        }
    }

    public void tick(boolean playerMoving) {
        if (!playerMoving) {
            settleToRest();
            return;
        }
        for (Point p : points) {
            if (p.locked) continue;
            p.y += gravity * FIXED_DT;
        }
        for (int i = points.size() - 2; i >= 1; i--) {
            Point prev = points.get(i - 1);
            Point mid = points.get(i);
            Point next = points.get(i + 1);

            double angle = angleBetween(mid, prev, next);
            double abs = Math.abs(normalizeDegrees(angle));

            if (abs < 180 - MAX_BEND_DEG) {
                clampAngle(mid, prev, next, angle < 0 ? -(180 - MAX_BEND_DEG + 1) : (180 - MAX_BEND_DEG + 1));
            } else if (abs > 180 + MAX_BEND_DEG) {
                clampAngle(mid, prev, next, angle < 0 ? -(180 + MAX_BEND_DEG - 1) : (180 + MAX_BEND_DEG - 1));
            }
        }

        for (int iter = 0; iter < SOLVER_ITERATIONS; iter++) {
            for (int i = points.size() - 1; i > 0; i--) {
                Point a = points.get(i - 1);
                Point b = points.get(i);

                float midX = (a.x + b.x) * 0.5f;
                float midY = (a.y + b.y) * 0.5f;

                float dx = a.x - b.x, dy = a.y - b.y;
                float len = (float) Math.sqrt(dx * dx + dy * dy);
                if (len < 1.0e-4f) continue;
                dx /= len; dy /= len;

                if (!a.locked) {
                    a.x = midX + dx * (SEGMENT_LENGTH * 0.5f);
                    a.y = midY + dy * (SEGMENT_LENGTH * 0.5f);
                }
                if (!b.locked) {
                    b.x = midX - dx * (SEGMENT_LENGTH * 0.5f);
                    b.y = midY - dy * (SEGMENT_LENGTH * 0.5f);
                }
            }
        }

        for (int i = 1; i < points.size(); i++) {
            Point a = points.get(i - 1);
            Point b = points.get(i);
            float dx = a.x - b.x, dy = a.y - b.y;
            float len = (float) Math.sqrt(dx * dx + dy * dy);
            if (len < 1.0e-4f) continue;
            dx /= len; dy /= len;
            if (!b.locked) {
                b.x = a.x - dx * SEGMENT_LENGTH;
                b.y = a.y - dy * SEGMENT_LENGTH;
            }
        }
    }

    private void settleToRest() {
        for (int i = 0; i < points.size(); i++) {
            Point p = points.get(i);
            if (p.locked) continue;
            float restY = i * SEGMENT_LENGTH;
            p.prevX = p.x;
            p.prevY = p.y;
            p.x += (0.0f - p.x) * REST_EASE;
            p.y += (restY - p.y) * REST_EASE;
        }
        Point root = points.get(0);
        root.prevX = root.x;
        root.prevY = root.y;
        root.x = 0.0f;
        root.y = 0.0f;
    }

    private double angleBetween(Point mid, Point prev, Point next) {
        double a1 = Math.atan2(next.y - mid.y, next.x - mid.x);
        double a2 = Math.atan2(prev.y - mid.y, prev.x - mid.x);
        return Math.toDegrees(a1 - a2);
    }

    private double normalizeDegrees(double deg) {
        while (deg > 360) deg -= 360;
        while (deg < -360) deg += 360;
        return deg;
    }

    private void clampAngle(Point mid, Point prev, Point next, double targetDeg) {
        double theta = Math.toRadians(targetDeg);
        float x = prev.x - mid.x;
        float y = prev.y - mid.y;
        double cs = Math.cos(theta), sn = Math.sin(theta);
        next.x = (float) (x * cs - y * sn + mid.x);
        next.y = (float) (x * sn + y * cs + mid.y);
    }

    public Point getPoint(int index) {
        return points.get(index);
    }

    public int getPointCount() {
        return points.size();
    }

    public static class Point {
        public float x, y;
        public float prevX, prevY;
        public boolean locked;

        public float getRenderX(float partialTicks) {
            return prevX + (x - prevX) * partialTicks;
        }

        public float getRenderY(float partialTicks) {
            return prevY + (y - prevY) * partialTicks;
        }
    }
}
