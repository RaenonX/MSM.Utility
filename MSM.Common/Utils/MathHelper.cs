namespace MSM.Common.Utils; 

public static class MathHelper {
    public static dynamic DifferencePct(dynamic num1, dynamic num2) {
        return Math.Abs(num1 - num2) / Math.Max(num1, num2) * 100;
    }
}