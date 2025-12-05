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
        public DbSet<UcastDetailView> UcastiDetail { get; set; } = null!;
        public DbSet<DokumentZavodnika> DokumentyZavodniku { get; set; } = null!;
        public DbSet<Uzivatel> Uzivatele { get; set; } = null!;
        public DbSet<ZavodDetailView> ZavodyDetail { get; set; } = null!;

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

            // ----- ZAVODNIK -----
            modelBuilder.Entity<Zavodnik>(entity =>
            {
                entity.Property(z => z.IdZavodnik).HasColumnName("ID_ZAVODNIK");
                entity.Property(z => z.Jmeno).HasColumnName("JMENO");
                entity.Property(z => z.Prijmeni).HasColumnName("PRIJMENI");
                entity.Property(z => z.Vek).HasColumnName("VEK");
                entity.Property(z => z.Pohlavi).HasColumnName("POHLAVI");
                entity.Property(z => z.UrovenZkusenosti).HasColumnName("UROVEN_ZKUSENOSTI");
            });

            // ----- KOLOBEZKA -----
            modelBuilder.Entity<Kolobezka>(entity =>
            {
                entity.Property(k => k.IdKolobezka).HasColumnName("ID_KOLOBEZKA");
                entity.Property(k => k.Model).HasColumnName("MODEL");
                entity.Property(k => k.Znacka).HasColumnName("ZNACKA");
                entity.Property(k => k.IdTypKolobezky).HasColumnName("ID_TYP_KOLOBEZKY");
            });

            // ----- JEZDI_NA -----
            modelBuilder.Entity<JezdiNa>(entity =>
            {
                entity.Property(j => j.IdZavodnik).HasColumnName("ZAVODNIK_ID_ZAVODNIK");
                entity.Property(j => j.IdKolobezka).HasColumnName("KOLOBEZKA_ID_KOLOBEZKA");
            });

            // ----- PK COMPOSITE -----
            modelBuilder.Entity<Ucast>().HasKey(u => new { u.IdZavodnik, u.IdZavod });
            modelBuilder.Entity<JezdiNa>().HasKey(j => new { j.IdZavodnik, j.IdKolobezka });
            modelBuilder.Entity<Organizace>().HasKey(o => new { o.IdZamestnanec, o.IdZavod });

            // ----- PK -----
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

            // ----- MAPOVÁNÍ SLOUPCŮ -----
            modelBuilder.Entity<TypZavodu>()
                .Property(t => t.IdTypZavodu).HasColumnName("ID_TYP_ZAVODU");
            modelBuilder.Entity<TypZavodu>()
                .Property(t => t.Nazev).HasColumnName("NAZEV");

            modelBuilder.Entity<Zavod>()
                .Property(z => z.IdTypZavodu).HasColumnName("ID_TYP_ZAVODU");
            modelBuilder.Entity<Zavod>()
                .Property(z => z.IdMisto).HasColumnName("ID_MISTO");
            modelBuilder.Entity<Zavod>()
                .Property(z => z.IdHodnoceni).HasColumnName("ID_HODNOCENI");

            modelBuilder.Entity<Misto>()
                .Property(m => m.IdMisto).HasColumnName("ID_MISTO");
            modelBuilder.Entity<Misto>()
                .Property(m => m.Nazev).HasColumnName("NAZEV");
            modelBuilder.Entity<Misto>()
                .Property(m => m.IdAdresa).HasColumnName("ID_ADRESA");

            modelBuilder.Entity<Hodnoceni>()
                .Property(h => h.IdHodnoceni).HasColumnName("ID_HODNOCENI");
            modelBuilder.Entity<Hodnoceni>()
                .Property(h => h.Metoda).HasColumnName("METODA");

            // ----- ZAVOD -----
            modelBuilder.Entity<Zavod>(entity =>
            {
                entity.Property(z => z.IdZavod).HasColumnName("ID_ZAVOD");
                entity.Property(z => z.Nazev).HasColumnName("NAZEV");
                entity.Property(z => z.Datum).HasColumnName("DATUM");
                entity.Property(z => z.Startovne).HasColumnName("STARTOVNE");
                entity.Property(z => z.IdTypZavodu).HasColumnName("ID_TYP_ZAVODU");
                entity.Property(z => z.IdMisto).HasColumnName("ID_MISTO");
                entity.Property(z => z.IdHodnoceni).HasColumnName("ID_HODNOCENI");
            });

            // ----- FK -----
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

            // ===========================================
            //          VIEW V_ZAVODY_DETAIL
            // ===========================================
            modelBuilder.Entity<ZavodDetailView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("V_ZAVODY_DETAIL");

                entity.Property(e => e.IdZavod).HasColumnName("ID_ZAVOD");
                entity.Property(e => e.NazevZavodu).HasColumnName("NAZEV_ZAVODU");
                entity.Property(e => e.Datum).HasColumnName("DATUM");
                entity.Property(e => e.Startovne).HasColumnName("STARTOVNE");
                entity.Property(e => e.TypZavodu).HasColumnName("TYP_ZAVODU");
                entity.Property(e => e.Misto).HasColumnName("MISTO");
                entity.Property(e => e.Ulice).HasColumnName("ULICE");
                entity.Property(e => e.Cp).HasColumnName("CP");
                entity.Property(e => e.Psc).HasColumnName("PSC");
                entity.Property(e => e.Mesto).HasColumnName("MESTO");
                entity.Property(e => e.MetodaHodnoceni).HasColumnName("METODA_HODNOCENI");
            });

            // ===========================================
            //          VIEW V_UCASTI_DETAIL
            // ===========================================
            modelBuilder.Entity<UcastDetailView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("V_UCASTI_DETAIL");

                entity.Property(e => e.IdZavodnik).HasColumnName("ID_ZAVODNIK");
                entity.Property(e => e.IdZavod).HasColumnName("ID_ZAVOD");
                entity.Property(e => e.Jmeno).HasColumnName("JMENO");
                entity.Property(e => e.Prijmeni).HasColumnName("PRIJMENI");
                entity.Property(e => e.NazevZavodu).HasColumnName("NAZEV_ZAVODU");
                entity.Property(e => e.DatumZavodu).HasColumnName("DATUM_ZAVODU");
                entity.Property(e => e.Castka).HasColumnName("CASTKA");
                entity.Property(e => e.TypPlatby).HasColumnName("TYP_PLATBY");
            });

            // ===========================================
            //      DOKUMENTY_ZAVODNIKU
            // ===========================================
            modelBuilder.Entity<DokumentZavodnika>(entity =>
            {
                entity.ToTable("DOKUMENTY_ZAVODNIKU");

                entity.HasKey(d => d.IdDokument);

                entity.Property(d => d.IdDokument).HasColumnName("ID_DOKUMENT");
                entity.Property(d => d.IdZavodnik).HasColumnName("ID_ZAVODNIK");
                entity.Property(d => d.NazevSouboru).HasColumnName("NAZEV_SOUBORU");
                entity.Property(d => d.TypSouboru).HasColumnName("TYP_SOUBORU");
                entity.Property(d => d.PriponaSouboru).HasColumnName("PRIPONA_SOUBORU");
                entity.Property(d => d.Obsah).HasColumnName("OBSAH");
                entity.Property(d => d.DatumNahrani).HasColumnName("DATUM_NAHRANI");
                entity.Property(d => d.DatumModifikace).HasColumnName("DATUM_MODIFIKACE");
                entity.Property(d => d.UzivatelVytvoril).HasColumnName("UZIVATEL_VYTVORIL");
                entity.Property(d => d.UzivatelZmenil).HasColumnName("UZIVATEL_ZMENIL");

                entity.HasOne(d => d.Zavodnik)
                      .WithMany()
                      .HasForeignKey(d => d.IdZavodnik);
            });

            // ===========================================
            //               UZIVATELE
            // ===========================================
            modelBuilder.Entity<Uzivatel>().ToTable("UZIVATELE");

            modelBuilder.Entity<Uzivatel>().HasKey(u => u.IdUzivatel);

            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.IdUzivatel).HasColumnName("ID_UZIVATEL");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.Jmeno).HasColumnName("JMENO");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.Email).HasColumnName("EMAIL");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.Heslo).HasColumnName("HESLO");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.Role).HasColumnName("ROLE");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.DatumVytvoreni).HasColumnName("DATUM_VYTVORENI");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.DatumPoslednihoPrihlaseni).HasColumnName("DATUM_POSLEDNIHO_PRIHLASENI");
            modelBuilder.Entity<Uzivatel>()
                .Property(u => u.IdZavodnik).HasColumnName("ID_ZAVODNIK");

            modelBuilder.Entity<Uzivatel>()
                .HasOne(u => u.Zavodnik)
                .WithMany()
                .HasForeignKey(u => u.IdZavodnik)
                .HasConstraintName("UZIVATEL_ZAVODNIK_FK");
        }
    }
}
