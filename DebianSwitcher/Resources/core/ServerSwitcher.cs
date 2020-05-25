using System.Linq;
using DebianSwitcher.Extensions;
using DebianSwitcher.Helpers;
using System.Threading.Tasks;

namespace DebianSwitcher
{
    class ServerSwitcher
    {
        private readonly string serverip;

        public ServerSwitcher(string ipaddress)
        {
            this.serverip = ipaddress;
        }

        public void SwitchDebian()
        {
            var lines = HostsFile.ReadAllLines();
            var result = lines.Where(x => !x.Contains("ppy.sh")).ToList();
            result.AddRange
            (
                serverip + " osu.ppy.sh",
                serverip + " c.ppy.sh",
                serverip + " c1.ppy.sh",
                serverip + " c2.ppy.sh",
                serverip + " c3.ppy.sh",
                serverip + " c4.ppy.sh",
                serverip + " c5.ppy.sh",
                serverip + " c6.ppy.sh",
                serverip + " ce.ppy.sh",
                serverip + " a.ppy.sh",
                serverip + " i.ppy.sh"
            );
            HostsFile.WriteAllLines(result);
        }

        public void SwitchBancho()
        {
            HostsFile.WriteAllLines(HostsFile.ReadAllLines().Where(x => !x.Contains("ppy.sh")));
        }

        public Task<Server> GetCurrentServerAsync()
        {
            return Task.Run<Server>(() => GetCurrentServer());
        }

        public Server GetCurrentServer()
        {
            bool isDebian = HostsFile.ReadAllLines().Any(x => x.Contains("osu.ppy.sh") && !x.Contains("#"));
            return isDebian ? Server.Debian : Server.Bancho;
        }
    }

    public enum Server
    {
        Bancho, Debian
    }
}
