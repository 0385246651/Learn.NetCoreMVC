using App.Models;

namespace App.Services
{
  public class PlanetService : List<PlanetModel>
  {
    public PlanetService()
    {
      Add(new PlanetModel { Id = 1, Name = "Mercury", VnName = "Sao Thủy", Content = "Sao Thủy là hành tinh nhỏ nhất trong Hệ Mặt Trời và gần Mặt Trời nhất. Sao Thủy có bề mặt giống với Mặt Trăng của Trái Đất, với nhiều hố va chạm và vùng đất bằng phẳng." });

      Add(new PlanetModel { Id = 2, Name = "Venus", VnName = "Sao Kim", Content = "Sao Kim là hành tinh thứ hai từ Mặt Trời và là hành tinh sáng nhất trên bầu trời đêm. Bề mặt của Sao Kim được bao phủ bởi các đám mây dày đặc chứa axit sulfuric, làm cho nó trở thành một trong những nơi nóng nhất trong Hệ Mặt Trời." });

      Add(new PlanetModel { Id = 3, Name = "Earth", VnName = "Trái Đất", Content = "Trái Đất là hành tinh thứ ba từ Mặt Trời và là nơi duy nhất được biết đến có sự sống. Nó có một bầu khí quyển giàu oxy và nước ở dạng lỏng trên bề mặt, tạo điều kiện thuận lợi cho sự phát triển của các sinh vật." });

      Add(new PlanetModel { Id = 4, Name = "Mars", VnName = "Sao Hỏa", Content = "Sao Hỏa là hành tinh thứ tư từ Mặt Trời và được biết đến như 'Hành tinh Đỏ' do màu sắc đỏ đặc trưng của nó. Bề mặt của Sao Hỏa có các thung lũng sâu, núi lửa lớn và bằng chứng về nước trong quá khứ." });

      Add(new PlanetModel { Id = 5, Name = "Jupiter", VnName = "Sao Mộc", Content = "Sao Mộc là hành tinh lớn nhất trong Hệ Mặt Trời và là một hành tinh khí khổng lồ. Nó có một hệ thống vòng lớn và nhiều mặt trăng, bao gồm Ganymede, mặt trăng lớn nhất trong Hệ Mặt Trời." });
    }
  }
}