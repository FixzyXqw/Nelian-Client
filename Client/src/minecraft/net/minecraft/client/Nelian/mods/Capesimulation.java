package net.minecraft.client.Nelian.mods;

import java.util.ArrayList;
import java.util.List;

/*
 *I KNOW IM STRICT WITH MY CODE SOMETIMES, BUT ISE THIS ONE HOWEVER YOU WANT BECAUSE I KNOW ITS HARD :D
 *
 *
 *
 * I SOMETIMES USE GOOGLE TRANSLATE FOR LONG ASS COMMENTS SO MAY INCLUDE PROBLEMS IN IT ALR?
 * 
 * Days wasted: 4 and a half
 * 
 * 
 * * A simple 2D Verlet-ish (i know :D) rope simulation.
 * I studied the general approach used by WaveyCapes StickSimulation,
 * but this was implemented separately and intentionally kept SIMPLE cuz OMFG its hard.
 *
 * READ BEFORE U NEED:
 * There is NO velocity/momentum transfer system.
 * 
 * I intentionally left momentum transfer out because it can make the
 * physics unstable and cause uncontrolled spinning typa shit behavior.
 * Instead, a fixed amount of gravity is applied every tick because its easier :D
 *
 * This isn't technically full Verlet integration it's closer to a simple
 * semi implicit Euler approach. The goal isn't perfect physics anyways, but
 * what i want is stable and predictable cape movement.
 *
 * The simulation only uses 2 axes:
 *   x = forward/backward bending
 *   y = vertical movement
 *  THERES NO FUCKIN Z OKAY?
 *  
 * Left/right swinging is NOT physically simulated in this class.
 * Instead, the cape plane is rotated around the Y axis during rendering
 * to create the lateral movement. If I've ever add any physics to the Z
 * I'd do it in a diff. class cause this is enough cursed already
 *
 * This gives us the visual effect without adding a third physics axis
 * and making the whole thing unnecessarily complicated.
 *
 *
 * DISCLAIMER:
 * I did NOT copy this from the WaveyCapes source code.
 * I studied their implementation to understand how this kind of
 * simulation works, then wrote my own version.
 *
 * Studying an implementation and using common math/physics concepts
 * does not mean copying the implementation itself.
 *
 * Math is math, buddy AKA tr7zw(please dont copyright my ass im already broke and Nelian is completely free and NEVER will be paid.)
 * Nelian doesn't contain anything that supports me financially, but im sure it effects me bad with mental shit CAUSE I HATE MATH.
 * 
 * 
 * Türk canlarım için:
 * 
 * 2D Verlet tarzı bir ip simülasyonu.
 * WaveyCapes'in StickSimulation'ını inceledim onun genel yaklaşımına
 * yakın ama kendi implementasyonum olacak şekilde kasıtlı olarak SADE
 * tuttum.
 *
 * HABERİNİZ OLSUN:
 * Hız/momentum taşıma sistemi YOK.
 *
 * Momentum taşımayı denediğimde fizik kolayca kararsızlaşıp kontrolsüz
 * şekilde helicopter efekti yapabildiği için bunu bilerek kullanmadım.
 * Bunun yerine her tick'te sabit bir yerçekimi miktarı ekleniyor.
 *
 * Bu teknik olarak tam Verlet değil daha çok basit bir semi implicit
 * Euler yaklaşımı. Amaç mükemmel fizik yapmak değil, stabil ve
 * öngörülebilir bir cape hareketi elde etmek.
 *
 * Simülasyon sadece 2 eksenli:
 *   x = ileri/geri eğilme
 *   y = dikey hareket
 *
 * Sağa/sola sallanma bu sınıfta fiziksel olarak simüle edilmiyor.
 * Bunun yerine render tarafında cape düzlemini Y ekseni etrafında
 * döndürerek bu efekti elde ediyoruz. Sonradan Z için bir system yaparsam 
 * başka bir sınıf içinde yaparım çünkü bu sınıf olduğunca lanetli :D
 *
 * Böylece üçüncü bir fizik boyutu ekleyip sistemi gereksiz yere
 * karmaşıklaştırmadan lateral hareketi görsel olarak elde ediyoruz.
 *
 * DEVAMINI ÇEVİRMEYECEĞIM ÜŞENDİM. AŞAĞIDAKİLERİN HEPSİ INGILIZCE!!
 *
 */

public class Capesimulation {

    private static final float FIXED_DT = 0.05f;       // 1 real game tick = 50ms
    public static final float SEGMENT_LENGTH = 1.0f;   // every sticks target length
    private static final int SOLVER_ITERATIONS = 3;    // stick constraint iter.
    private static final float MAX_BEND_DEG = 30.0f;   // Max extra bend between adjacent sticks – 5 was too stiff; it made them look like a fuckin brick.
    private static final float REST_EASE = 0.25f;      // i think this one is very understandable but anyways; how quickly the animation settles back to its resting position
    private static final float ROOT_EASE = 0.35f;      // how quickly free points smoothly approach the target lean value
    private static final float VERTICAL_SCALE = 1.0f;  // effect of vertical lag (added similarly to gravity)
    private static final float LEAN_GAIN = 3.0f;       // amplify the lean signal at the PHYSICS level, not during rendering so it prevents stretching, literally my hero :D

    public final List<Point> points = new ArrayList<Point>();
    public float gravity = 0.9f;

    public Capesimulation(int segmentCount) {
        for (int i = 0; i < segmentCount; i++) {
            Point p = new Point();
            p.y = i * SEGMENT_LENGTH; // positive = downwards (render space Y down convention)
            p.locked = (i == 0);      // first point is fixed to the shoulder
            points.add(p);
        }
    }

    /**
     * lean: forward/backward bending lag, vertical: vertical lag.
     *
     * IMPORTANT: Applied to ALL free points, NOT the root (point 0).
     *
     * The root should never move (it's physically fixed to the shoulder).
     * We previously tried moving the root and then using a "pull it back"
     * hack during rendering to compensate, but that also killed the visible
     * swinging because the rest of the chain has no momentum and therefore
     * follows the root too closely, leaving almost no difference between them.
     *
     * Now the root stays FIXED and the force is applied directly to the free
     * points. This means points near the root move less, while points near the
     * end swing more, creating a more natural flow / whip-like feeling.
     */
    
    
    public void applyMovement(float lean, float vertical) {
        int n = points.size();
        for (int i = 1; i < n; i++) {
            Point p = points.get(i);
         // weight: 0 at the root, 1 at the very end - this creates a gradually
         // increasing target along the entire chain. Previously, every point was
         // given the SAME "lean" target, which caused bending only around a few
         // points near the root (where the fixed root and the target were fighting)
         // while everything after that reached the same target and simply flattened
         // out ("bending only in the middle" problem).
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

     // 1) only apply gravity, momentum/velocity transfer is intentionally NOT used.
     // NOTE: prevX/prevY are NOT updated here again - applyMovement() has already
     // stored the correct "end of previous tick" values for this tick. Updating
     // them again here (like we used to) would shrink the interpolation range,
     // making the cape look "steppy", as if it were running at low FPS.
        for (Point p : points) {
            if (p.locked) continue;
            p.y += gravity * FIXED_DT;
        }

     // You'll remember this part from the start of the comments. It was originally written according to WaveyCapes
     // own x-sign convention, but since our lean sign was flipped during the
     // forward/backward direction fix, this step was instantly cancelling out
     // the swing produced every tick - this was the reason for the
     // my ass is the same ass as the vanilla ass problem.

     // 3) limit excessive bending doesn't work properly, but prevents folding so who gives a fuck :D
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
