using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using WebAPI.Exceptions;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests;

[TestClass]
public class SeatsControllerTests
{
    Mock<SeatsService> mockService;
    Mock<SeatsController> mockController;
    public SeatsControllerTests()
    {
        mockService = new Mock<SeatsService>();
        mockController = new Mock<SeatsController>(mockService.Object) { CallBase = true };

        mockController.Setup(c => c.UserId).Returns("11111");
        
    }

    [TestMethod]
    public void ReserveSeat()
    {
        Seat seat = new Seat();
        seat.Id = 1;
        seat.Number = 1;

        mockService.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Returns(seat);

        var actionresult = mockController.Object.ReserveSeat(seat.Number);

        var result = actionresult.Result as OkObjectResult;
        Assert.IsNotNull(result);
    }
    [TestMethod]
    public void ReserveSeat_SeatAlreadyTaken()
    {
       

        mockService.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatAlreadyTakenException());

        var actionresult = mockController.Object.ReserveSeat(1);
        var result = actionresult.Result as UnauthorizedResult;
        
        Assert.IsNotNull(result);
    }
    [TestMethod]
    public void ReserveSeat_SeatOutOfBounds()
    {
        mockService.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatOutOfBoundsException());

        var seatNumber = 1;
        var actionresult = mockController.Object.ReserveSeat(seatNumber);
        
        var result = actionresult.Result as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("Could not find " + seatNumber, result.Value);
    }
    [TestMethod]
    public void ReserveSeat_UserAlreadySeated()
    {

        mockService.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new UserAlreadySeatedException());

        var actionresult = mockController.Object.ReserveSeat(1);
        var result = actionresult.Result as BadRequestResult;

        Assert.IsNotNull(result);
    }
}
