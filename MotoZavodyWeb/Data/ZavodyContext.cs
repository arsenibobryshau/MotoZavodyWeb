using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Models;
using MotoZavodyWeb.Models;

namespace MotoZavodyWeb.Data
{
    public class ZavodyContext : DbContext
    {
        public ZavodyContext(DbContextOptions<ZavodyContext> options)
            : base(options)
        {
        }

        public DbSet<Adresa> Adresy { get; set; } = null!;
        public DbSet<Hodnoceni> Hodnoceni { get; set; } = null!;
        public DbSet<JezdiNa> JezdiNa { get; set; } = null!;
        public DbSet<Kolobezka> Kolobezky { get; set; } = null!;
        public DbSet<Misto> Mista { get; set; } = null!;
        public DbSet<Organizace> Organizace { get; set; } = null!;
        public DbSet<Platba> Platby { get; set; } = null!;
        public DbSet<Posta> Posty { get; set; } = null!;
        public DbSet<Pozice> Pozice { get; set; } = null!;
        public DbSet<TypKolobezky> TypyKolobezek { get; set; } = null!;
        public DbSet<TypZavodu> TypyZavodu { get; set; } = null!;
        public DbSet<Ucast> Ucasti { get; set; } = null!;
        public DbSet<Zamestnanec> Zamestnanci { get; set; } = null!;
        public DbSet<Zavod> Zavody { get; set; } = null!;
        public DbSet<Zavodnik> Zavodnici { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite PKs
            modelBuilder.Entity<Ucast>()
                .HasKey(u => new { u.IdZavodnik, u.IdZavod });

            modelBuilder.Entity<JezdiNa>()
                .HasKey(j => new { j.IdZavodnik, j.IdKolobezka });

            modelBuilder.Entity<Organizace>()
                .HasKey(o => new { o.IdZamestnanec, o.IdZavod });

            // Jednoduché PKs
            modelBuilder.Entity<Adresa>()
                .HasKey(a => a.IdAdresa);

            modelBuilder.Entity<Hodnoceni>()
                .HasKey(h => h.IdHodnoceni);

            modelBuilder.Entity<Kolobezka>()
                .HasKey(k => k.IdKolobezka);

            modelBuilder.Entity<Misto>()
                .HasKey(m => m.IdMisto);

            modelBuilder.Entity<Platba>()
                .HasKey(p => p.IdPlatby);

            modelBuilder.Entity<Posta>()
                .HasKey(p => p.IdPosta);

            modelBuilder.Entity<Pozice>()
                .HasKey(p => p.IdPozice);

            modelBuilder.Entity<TypKolobezky>()
                .HasKey(t => t.IdTypKolobezky);

            modelBuilder.Entity<TypZavodu>()
                .HasKey(t => t.IdTypZavodu);

            modelBuilder.Entity<Zamestnanec>()
                .HasKey(z => z.IdZamestnanec);

            modelBuilder.Entity<Zavod>()
                .HasKey(z => z.IdZavod);

            modelBuilder.Entity<Zavodnik>()
                .HasKey(z => z.IdZavodnik);
        }
    }
}
