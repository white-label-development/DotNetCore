# Mocking with moq and xunit

## 2

Test Double:

+ fake - working implementation
+ dummies - passed around but not used (satisfy parms that can't be null)
+ stubs - can provide answers to calls
+ mocks - expects and verify that calls were used

moq does all except fakes.

## 3

```c#
Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

//mockValidator.Setup(x => x.IsValid("x")).Returns(true);
//mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
//mockValidator.Setup(
//                  x => x.IsValid(It.Is<string>(number => number.StartsWith("y"))))
//             .Returns(true);
//mockValidator.Setup(
//              x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive)))
//             .Returns(true);
//mockValidator.Setup(
//              x => x.IsValid(It.IsIn("z", "y", "x")))
//             .Returns(true);
mockValidator.Setup(
                x => x.IsValid(It.IsRegex("[a-z]")))
                .Returns(true);
```

verify

`mockValidator.Verify(x => x.IsValid(It.IsAny<string>()));`

`mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);`

properties

```c#
var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
//mockValidator.SetupProperty(x => x.ValidationMode);
mockValidator.SetupAllProperties();
mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
```

can set even without a setter

```c#
mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
 mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
```

exception

`mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws(new Exception("Custom message"));`
