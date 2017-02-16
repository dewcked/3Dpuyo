using System;
using Assets.Scripts;
using NUnit.Framework;

[TestFixture]
internal class PuyoHelperTest
{
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ThrowOutOfRangeExceptionWhenPassingPink()
    {
        PuyoHelper.GetPuyoColorFromString("pink");
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ThrowArgumentNullExceptionWhenPassingEmptyColor()
    {
        PuyoHelper.GetPuyoColorFromString(string.Empty);
    }

    [Test]
    public void ReturnBlueWhenPassingBlue()
    {
        Assert.AreEqual(PuyoHelper.GetPuyoColorFromString("Blue"), PuyoColor.Blue);
    }

    [Test]
    public void ReturnRedWhenPassingRed()
    {
        Assert.AreEqual(PuyoHelper.GetPuyoColorFromString("Red"), PuyoColor.Red);
    }

    [Test]
    public void ReturnYellowWhenPassingYellow()
    {
        Assert.AreEqual(PuyoHelper.GetPuyoColorFromString("Yellow"), PuyoColor.Yellow);
    }

    [Test]
    public void ReturnGreenWhenPassingGreen()
    {
        Assert.AreEqual(PuyoHelper.GetPuyoColorFromString("Green"), PuyoColor.Green);
    }
}