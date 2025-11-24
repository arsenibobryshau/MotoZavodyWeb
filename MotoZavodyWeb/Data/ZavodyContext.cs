using Microsoft.EntityFrameworkCore;
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

            // ----- MAPOVÁNÍ TABULEK -----
            modelBuilder.Entity<Adresa>().ToTable("ADRESY");
            modelBuilder.Entity<Hodnoceni>().ToTable("HODNOCENI");
            modelBuilder.Entity<JezdiNa>().ToTable("JEZDI_NA");
            modelBuilder.Entity<Kolobezka>().ToTable("KOLOBEZKY");
            modelBuilder.Entity<Misto>().ToTable("MISTA");
            modelBuilder.Entity<Organizace>().ToTable("ORGANIZACE");
            modelBuilder.Entity<Platba>().ToTable("PLATBY");
            modelBuilder.Entity<Posta>().ToTable("POSTY");
            modelBuilder.Entity<Pozice>().ToTable("POZICE");
            modelBuilder.Entity<TypKolobezky>().ToTable("TYP_KOLOBEZEK");
            modelBuilder.Entity<TypZavodu>().ToTable("TYP_ZAVODU");
            modelBuilder.Entity<Ucast>().ToTable("UCASTI");
            modelBuilder.Entity<Zamestnanec>().ToTable("ZAMESTNANCI");
            modelBuilder.Entity<Zavodnik>().ToTable("ZAVODNICI");
            modelBuilder.Entity<Zavod>().ToTable("ZAVODY");

            // ----- MAPOVÁNÍ SLOUPCŮ -----
            modelBuilder.Entity<TypZavodu>()
                .Property(t => t.IdTypZavodu)
                .HasColumnName("ID_TYP_ZAVODU");

            modelBuilder.Entity<Zavod>()
                .Property(z => z.IdTypZavodu)
                .HasColumnName("ID_TYP_ZAVODU");

            modelBuilder.Entity<Zavod>()
                .Property(z => z.IdMisto)
                .HasColumnName("ID_MISTO");

            modelBuilder.Entity<Zavod>()
                .Property(z => z.IdHodnoceni)
                .HasColumnName("ID_HODNOCENI");

            modelBuilder.Entity<Zavod>()
                .Property(z => z.Datum)
                .HasColumnName("DATUM");

            modelBuilder.Entity<Misto>()
                .Property(m => m.IdMisto)
                .HasColumnName("ID_MISTO");

            modelBuilder.Entity<Misto>()
                .Property(m => m.Nazev)
                .HasColumnName("NAZEV");

            modelBuilder.Entity<Misto>()
                .Property(m => m.IdAdresa)
                .HasColumnName("ID_ADRESA");


            // ----- FK ----
            
            modelBuilder.Entity<Zavod>()
                .HasOne(z => z.TypZavodu)
                .WithMany()
                .HasForeignKey(z => z.IdTypZavodu)
                .HasConstraintName("FK_ZAVODY_TYP_ZAVODU");

            modelBuilder.Entity<Zavod>()
                .HasOne(z => z.Misto)
                .WithMany(m => m.Zavody)
                .HasForeignKey(z => z.IdMisto)
                .HasConstraintName("FK_ZAVODY_MISTO");

            modelBuilder.Entity<Zavod>()
                .HasOne(z => z.Hodnoceni)
                .WithMany()
                .HasForeignKey(z => z.IdHodnoceni)
                .HasConstraintName("FK_ZAVODY_HODNOCENI");

            modelBuilder.Entity<Misto>()
                .HasOne(m => m.Adresa)
                .WithMany()
                .HasForeignKey(m => m.IdAdresa)
                .HasConstraintName("MISTO_ADRESA_FK");

            // ----- PK -----
            modelBuilder.Entity<Ucast>().HasKey(u => new { u.IdZavodnik, u.IdZavod });
            modelBuilder.Entity<JezdiNa>().HasKey(j => new { j.IdZavodnik, j.IdKolobezka });
            modelBuilder.Entity<Organizace>().HasKey(o => new { o.IdZamestnanec, o.IdZavod });

            modelBuilder.Entity<Adresa>().HasKey(a => a.IdAdresa);
            modelBuilder.Entity<Hodnoceni>().HasKey(h => h.IdHodnoceni);
            modelBuilder.Entity<Kolobezka>().HasKey(k => k.IdKolobezka);
            modelBuilder.Entity<Misto>().HasKey(m => m.IdMisto);
            modelBuilder.Entity<Platba>().HasKey(p => p.IdPlatby);
            modelBuilder.Entity<Posta>().HasKey(p => p.IdPosta);
            modelBuilder.Entity<Pozice>().HasKey(p => p.IdPozice);
            modelBuilder.Entity<TypKolobezky>().HasKey(t => t.IdTypKolobezky);
            modelBuilder.Entity<TypZavodu>().HasKey(t => t.IdTypZavodu);
            modelBuilder.Entity<Zamestnanec>().HasKey(z => z.IdZamestnanec);
            modelBuilder.Entity<Zavod>().HasKey(z => z.IdZavod);
            modelBuilder.Entity<Zavodnik>().HasKey(z => z.IdZavodnik);
        }
    }
}
