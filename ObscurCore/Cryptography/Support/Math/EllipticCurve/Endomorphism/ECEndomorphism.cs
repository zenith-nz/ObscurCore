﻿namespace ObscurCore.Cryptography.Support.Math.EllipticCurve.Endomorphism
{
    public interface ECEndomorphism
    {
        ECPointMap PointMap { get; }

        bool HasEfficientPointMap { get; }
    }
}
