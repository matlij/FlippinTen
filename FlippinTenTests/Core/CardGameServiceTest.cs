using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Services;
using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FlippinTenTests.Core
{
    [TestClass]
    public class CardGameServiceTest
    {
        private Mock<IGenericRepository> _repository;
        private CardGameUtilities _cardGameUtilities;
        private ICardGameService _sut;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IGenericRepository>();
            _cardGameUtilities = new CardGameUtilities(new CardUtilities());
            _sut = new OnlineCardGameService(_repository.Object, _cardGameUtilities);
        }
    }
}
