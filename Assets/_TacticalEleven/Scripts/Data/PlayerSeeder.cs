using System.Collections.Generic;
using SQLite;
using UnityEngine;

public static class PlayerSeeder
{
    const int GK = 0, CB = 1, LB = 2, RB = 3, LWB = 4, RWB = 5, CDM = 6, CM = 7, CAM = 8, LM = 9, RM = 10, LW = 11, RW = 12, ST = 13, CF = 14;
    const int LEFT = 0, RIGHT = 1, BOTH = 2;
    const int GK_f = 1, CB_f = 2, LB_f = 4, RB_f = 8, LWB_f = 16, RWB_f = 32, CDM_f = 64, CM_f = 128, CAM_f = 256, LM_f = 512, RM_f = 1024, LW_f = 2048, RW_f = 4096, ST_f = 8192, CF_f = 16384;

    public static void Seed(SQLiteConnection db)
    {
        var players = new List<PlayerDataDB>();

        void Add(int teamId, string firstName, string lastName, string commonName, int age, string nationality,
                 int height, int weight, int foot, int mainPos, int secPos,
                 int pace, int acc, int sta, int str, int agi, int bal, int jmp, int nf,
                 int pas, int cro, int dri, int fin, int ft, int hed, int ls, int mar,
                 int pen, int fk, int cor, int tac, int tec, int fla,
                 int agg, int ant, int brv, int cmp, int con, int dec, int det, int lea,
                 int otb, int pos, int tw, int vis, int wr, int cst,
                 int han, int kic, int rfx, int gkp, int ooo, int coa, int ro, int ps,
                 int ovr, int pot, long val, int wage, long releaseClause, int contract, int number)
        {
            players.Add(new PlayerDataDB
            {
                TeamId = teamId,
                FirstName = firstName,
                LastName = lastName,
                CommonName = commonName,
                Age = age,
                Nationality = nationality,
                SecondNationality = "",
                Height = height,
                Weight = weight,
                Foot = foot,
                MainPosition = mainPos,
                SecondaryPositions = secPos,
                Pace = pace,
                Acceleration = acc,
                Stamina = sta,
                Strength = str,
                Agility = agi,
                Balance = bal,
                Jumping = jmp,
                NaturalFitness = nf,
                Passing = pas,
                Crossing = cro,
                Dribbling = dri,
                Finishing = fin,
                FirstTouch = ft,
                Heading = hed,
                LongShots = ls,
                Marking = mar,
                Penalties = pen,
                FreeKicks = fk,
                Corners = cor,
                Tackling = tac,
                Technique = tec,
                Flair = fla,
                Aggression = agg,
                Anticipation = ant,
                Bravery = brv,
                Composure = cmp,
                Concentration = con,
                Decisions = dec,
                Determination = det,
                Leadership = lea,
                OffTheBall = otb,
                Positioning = pos,
                Teamwork = tw,
                Vision = vis,
                WorkRate = wr,
                Consistency = cst,
                Handling = han,
                Kicking = kic,
                Reflexes = rfx,
                GkPositioning = gkp,
                OneOnOnes = ooo,
                CommandOfArea = coa,
                RushingOut = ro,
                PenaltySaving = ps,
                Overall = ovr,
                Potential = pot,
                Value = val,
                Wage = wage,
                ReleaseClause = releaseClause,
                ContractYears = contract,
                SquadNumber = number,
                Status = 0,
                Morale = 50,
                Fitness = 5,
                Form = 7.0,
                Personality = 1,
                TotalGoals = 0,
                TotalAssists = 0,
                TotalAppearances = 0
            });
        }

        // ── AddSimple: genera atributos desde OVR + posición ────
        void AddSimple(int tid, string fn, string ln, string cn, int age, string nat,
                       int h, int w, int ft, int pos, int secP,
                       int ovr, int pot, long val, int wage, long rc, int cnt, int num)
        {
            var prevState = UnityEngine.Random.state;
            UnityEngine.Random.InitState(tid * 100000 + num * 100 + pos);

            int R(int b) => Mathf.Clamp(b + UnityEngine.Random.Range(-5, 6), 20, 99);
            int RA(int b, int d) => Mathf.Clamp(b + d + UnityEngine.Random.Range(-5, 6), 20, 99);

            int phyB, tecB;
            if (pos == GK)
            {
                phyB = ovr - 12; tecB = ovr - 4;
            }
            else
            {
                phyB = ovr; tecB = ovr;
                switch (pos)
                {
                    case CB: phyB += 2; tecB -= 2; break;
                    case LB: case RB: case LWB: case RWB: phyB += 3; break;
                    case CDM: tecB -= 1; break;
                    case CM: tecB += 2; break;
                    case CAM: tecB += 3; break;
                    case LM: case RM: phyB += 2; tecB += 2; break;
                    case LW: case RW: phyB += 4; tecB += 2; break;
                    case ST: case CF: phyB += 3; tecB += 1; break;
                }
            }

            int pac = RA(phyB, 0), acc = RA(phyB, 0), sta = RA(phyB, 0), str = RA(phyB, 1);
            int agi = RA(phyB, 1), bal = RA(phyB, 0), jmp = RA(phyB, 1), nf = RA(phyB, 0);

            int pas, cro, dri, fin, ftT, hed, ls, mar, pen, fk, cor, tac, tecA, fla;

            if (pos == GK)
            {
                pas = RA(tecB - 2, 0); cro = R(tecB - 8); dri = R(tecB - 8); fin = R(tecB - 10);
                ftT = RA(tecB - 2, 0); hed = R(tecB - 4); ls = R(tecB - 8); mar = R(tecB - 10);
                pen = R(tecB - 4); fk = RA(tecB - 6, 0); cor = R(tecB - 6); tac = R(tecB - 10);
                tecA = RA(tecB - 2, 0); fla = R(tecB - 6);
            }
            else
            {
                switch (pos)
                {
                    case CB:
                        pas = RA(tecB - 1, 0); cro = R(tecB - 3); dri = R(tecB - 4); fin = R(tecB - 6);
                        ftT = RA(tecB - 1, 0); hed = RA(tecB + 2, 0); ls = R(tecB - 4); mar = RA(tecB + 3, 0);
                        pen = R(tecB - 2); fk = R(tecB - 5); cor = R(tecB - 5); tac = RA(tecB + 3, 0);
                        tecA = RA(tecB - 1, 0); fla = R(tecB - 3); break;
                    case LB:
                    case RB:
                    case LWB:
                    case RWB:
                        pas = RA(tecB, 0); cro = RA(tecB + 1, 0); dri = RA(tecB, 0); fin = R(tecB - 4);
                        ftT = RA(tecB, 0); hed = R(tecB - 2); ls = R(tecB - 3); mar = RA(tecB + 1, 0);
                        pen = R(tecB - 3); fk = RA(tecB - 2, 0); cor = RA(tecB - 1, 0); tac = RA(tecB + 1, 0);
                        tecA = RA(tecB, 0); fla = R(tecB - 2); break;
                    case CDM:
                        pas = RA(tecB + 1, 0); cro = R(tecB - 2); dri = R(tecB - 2); fin = R(tecB - 5);
                        ftT = RA(tecB, 0); hed = RA(tecB - 1, 0); ls = R(tecB - 3); mar = RA(tecB + 2, 0);
                        pen = R(tecB - 3); fk = R(tecB - 4); cor = R(tecB - 4); tac = RA(tecB + 2, 0);
                        tecA = RA(tecB, 0); fla = R(tecB - 3); break;
                    case CM:
                    case LM:
                    case RM:
                        pas = RA(tecB + 2, 0); cro = RA(tecB, 0); dri = RA(tecB + 1, 0); fin = R(tecB - 2);
                        ftT = RA(tecB + 1, 0); hed = R(tecB - 2); ls = RA(tecB - 1, 0); mar = RA(tecB - 1, 0);
                        pen = RA(tecB - 2, 0); fk = RA(tecB - 2, 0); cor = RA(tecB - 1, 0); tac = RA(tecB - 1, 0);
                        tecA = RA(tecB + 1, 0); fla = RA(tecB - 1, 0); break;
                    case CAM:
                        pas = RA(tecB + 3, 0); cro = RA(tecB + 1, 0); dri = RA(tecB + 2, 0); fin = RA(tecB + 1, 0);
                        ftT = RA(tecB + 2, 0); hed = R(tecB - 3); ls = RA(tecB + 1, 0); mar = RA(tecB - 2, 0);
                        pen = RA(tecB, 0); fk = RA(tecB + 1, 0); cor = RA(tecB, 0); tac = R(tecB - 3);
                        tecA = RA(tecB + 2, 0); fla = RA(tecB + 1, 0); break;
                    case LW:
                    case RW:
                        pas = RA(tecB + 1, 0); cro = RA(tecB + 2, 0); dri = RA(tecB + 4, 0); fin = R(tecB - 1);
                        ftT = RA(tecB + 2, 0); hed = R(tecB - 4); ls = RA(tecB, 0); mar = R(tecB - 4);
                        pen = R(tecB - 2); fk = RA(tecB - 1, 0); cor = RA(tecB - 1, 0); tac = R(tecB - 4);
                        tecA = RA(tecB + 3, 0); fla = RA(tecB + 2, 0); break;
                    default:
                        pas = RA(tecB - 1, 0); cro = R(tecB - 3); dri = RA(tecB, 0); fin = RA(tecB + 4, 0);
                        ftT = RA(tecB + 1, 0); hed = RA(tecB + 1, 0); ls = RA(tecB + 2, 0); mar = R(tecB - 6);
                        pen = RA(tecB + 1, 0); fk = R(tecB - 3); cor = R(tecB - 4); tac = R(tecB - 5);
                        tecA = RA(tecB, 0); fla = RA(tecB - 1, 0); break;
                }
            }

            int agg = RA(ovr, 0), ant = RA(ovr, 0), brv = RA(ovr, 0), cmp = RA(ovr, 0), cnc = RA(ovr, 0);
            int dec = RA(ovr, 0), det = RA(ovr, 0), lea = RA(ovr, 0), otb = RA(ovr, 0), posM = RA(ovr, 0);
            int tw = RA(ovr, 0), vis = RA(ovr, 0), wr = RA(ovr, 0), cst = RA(ovr, 0);

            int han = R(pos == GK ? ovr : 1), kic = R(pos == GK ? ovr : 1), rfx = R(pos == GK ? ovr : 1);
            int gkp = R(pos == GK ? ovr : 1), ooo = R(pos == GK ? ovr : 1), coa = R(pos == GK ? ovr : 1);
            int ro = R(pos == GK ? ovr : 1), ps = R(pos == GK ? ovr : 1);

            UnityEngine.Random.state = prevState;

            Add(tid, fn, ln, cn, age, nat, h, w, ft, pos, secP,
                pac, acc, sta, str, agi, bal, jmp, nf,
                pas, cro, dri, fin, ftT, hed, ls, mar,
                pen, fk, cor, tac, tecA, fla,
                agg, ant, brv, cmp, cnc, dec, det, lea,
                otb, posM, tw, vis, wr, cst,
                han, kic, rfx, gkp, ooo, coa, ro, ps,
                ovr, pot, val, wage, rc, cnt, num);
        }

        // ══════════════════════════════════════════════════════════
        // REAL MADRID (TeamId=1) — Primera División
        // ══════════════════════════════════════════════════════════
        int RMA = 1;

        AddSimple(RMA, "Thibaut", "Courtois", "Courtois", 32, "BEL", 200, 96, LEFT, GK, GK_f, 90, 98, 75_000_000, 12_000_000, 1_000_000_000, 3, 1);
        AddSimple(RMA, "Andriy", "Lunin", "Lunin", 25, "UKR", 191, 87, RIGHT, GK, GK_f, 78, 84, 15_000_000, 4_000_000, 500_000_000, 4, 13);
        AddSimple(RMA, "Éder", "Militão", "Militão", 26, "BRA", 186, 79, RIGHT, CB, RB_f, 85, 90, 60_000_000, 10_000_000, 1_000_000_000, 4, 3);
        AddSimple(RMA, "Trent", "Alexander-Arnold", "Alexander-Arnold", 26, "GBR", 180, 72, RIGHT, RB, RM_f, 85, 88, 75_000_000, 15_000_000, 1_000_000_000, 5, 2);
        AddSimple(RMA, "Denzel", "Dumfries", "Dumfries", 28, "NLD", 189, 80, RIGHT, RB, RWB_f, 83, 88, 25_000_000, 6_000_000, 300_000_000, 3, 5);
        AddSimple(RMA, "Raúl", "Asencio", "Asencio", 21, "ESP", 184, 74, RIGHT, CB, RB_f, 70, 82, 5_000_000, 1_500_000, 300_000_000, 5, 35);
        AddSimple(RMA, "Álvaro", "Carreras", "Carreras", 21, "ESP", 179, 70, LEFT, LB, LM_f, 82, 89, 8_000_000, 1_500_000, 300_000_000, 4, 20);
        AddSimple(RMA, "Fran", "García", "F. García", 25, "ESP", 169, 65, LEFT, LB, LM_f, 76, 80, 15_000_000, 3_000_000, 500_000_000, 4, 23);
        AddSimple(RMA, "Antonio", "Rüdiger", "Rüdiger", 31, "DEU", 190, 85, RIGHT, CB, RB_f, 84, 86, 35_000_000, 14_000_000, 500_000_000, 2, 22);
        AddSimple(RMA, "Ferland", "Mendy", "Mendy", 29, "FRA", 180, 73, LEFT, LB, CB_f, 80, 82, 30_000_000, 8_000_000, 500_000_000, 3, 4);
        AddSimple(RMA, "Dean", "Huijsen", "Huijsen", 19, "ESP", 195, 82, RIGHT, CB, CDM_f, 80, 85, 20_000_000, 2_500_000, 500_000_000, 5, 34);
        AddSimple(RMA, "Marc", "Cucurella", "Cucurella", 26, "ESP", 175, 68, LEFT, LB, LM_f | LWB_f, 78, 80, 25_000_000, 6_000_000, 300_000_000, 4, 6);
        AddSimple(RMA, "Ibrahima", "Konaté", "Konaté", 25, "FRA", 194, 89, RIGHT, CB, CDM_f, 82, 88, 45_000_000, 8_000_000, 500_000_000, 4, 15);
        AddSimple(RMA, "Jude", "Bellingham", "Bellingham", 21, "GBR", 186, 75, RIGHT, CAM, CM_f | LW_f, 90, 94, 180_000_000, 20_000_000, 1_000_000_000, 5, 5);
        AddSimple(RMA, "Eduardo", "Camavinga", "Camavinga", 22, "FRA", 182, 75, LEFT, CM, CDM_f | LB_f, 82, 90, 80_000_000, 10_000_000, 1_000_000_000, 5, 6);
        AddSimple(RMA, "Federico", "Valverde", "Valverde", 26, "URY", 182, 78, RIGHT, CM, RM_f | CDM_f, 86, 90, 120_000_000, 14_000_000, 1_000_000_000, 5, 8);
        AddSimple(RMA, "Aurélien", "Tchouaméni", "Tchouaméni", 24, "FRA", 187, 81, RIGHT, CDM, CM_f | CB_f, 84, 90, 90_000_000, 12_000_000, 1_000_000_000, 5, 18);
        AddSimple(RMA, "Arda", "Güler", "Güler", 19, "TUR", 176, 66, LEFT, CAM, RW_f | CM_f, 86, 92, 50_000_000, 5_000_000, 500_000_000, 5, 15);
        AddSimple(RMA, "Franco", "Mastantuono", "Mastantuono", 17, "ARG", 178, 68, LEFT, CAM, LW_f, 78, 92, 15_000_000, 1_000_000, 500_000_000, 5, 30);
        AddSimple(RMA, "Bernardo", "Silva", "Bernardo Silva", 30, "PRT", 173, 64, LEFT, CM, CM_f | CAM_f, 86, 86, 80_000_000, 18_000_000, 1_000_000_000, 3, 20);
        AddSimple(RMA, "Vinícius", "Júnior", "Vinícius Jr.", 24, "BRA", 176, 73, RIGHT, LW, RW_f, 92, 93, 200_000_000, 15_000_000, 1_000_000_000, 1, 7);
        AddSimple(RMA, "Endrick", "Felipe", "Endrick", 18, "BRA", 173, 68, LEFT, ST, CF_f | LW_f, 80, 90, 60_000_000, 6_000_000, 1_000_000_000, 5, 16);
        AddSimple(RMA, "Kylian", "Mbappé", "Mbappé", 26, "FRA", 178, 75, RIGHT, ST, LW_f | RW_f, 98, 99, 180_000_000, 35_000_000, 1_000_000_000, 4, 9);
        AddSimple(RMA, "Rodrygo", "Silva", "Rodrygo", 23, "BRA", 174, 68, RIGHT, RW, LW_f | CF_f, 84, 88, 100_000_000, 12_000_000, 1_000_000_000, 5, 11);
        AddSimple(RMA, "Gonzalo", "García", "Gonzalo", 20, "ESP", 182, 74, RIGHT, ST, CF_f, 66, 80, 3_000_000, 500_000, 100_000_000, 5, 38);
        AddSimple(RMA, "Brahim", "Díaz", "Brahim Díaz", 25, "ESP", 171, 67, RIGHT, CAM, RW_f | LW_f, 78, 84, 30_000_000, 6_000_000, 500_000_000, 4, 21);

        // ══════════════════════════════════════════════════════════
        // FC BARCELONA (TeamId=2)
        // ══════════════════════════════════════════════════════════
        int BAR = 2;
        AddSimple(BAR, "Joan", "García", "Joan García", 23, "ESP", 184, 76, RIGHT, GK, 0, 76, 83, 7_000_000, 1_200_000, 100_000_000, 4, 13);
        AddSimple(BAR, "Wojciech", "Szczesny", "Szczesny", 35, "POL", 196, 90, RIGHT, GK, 0, 82, 82, 4_000_000, 5_000_000, 50_000_000, 1, 25);
        AddSimple(BAR, "Pau", "Cubarsí", "Cubarsí", 18, "ESP", 184, 76, RIGHT, CB, 0, 76, 90, 30_000_000, 3_000_000, 500_000_000, 5, 2);
        AddSimple(BAR, "Eric", "García", "E. García", 23, "ESP", 182, 76, RIGHT, CB, CDM_f, 77, 84, 15_000_000, 2_500_000, 400_000_000, 4, 24);
        AddSimple(BAR, "Gerard", "Martín", "G. Martín", 22, "ESP", 180, 72, LEFT, LB, 0, 72, 80, 4_000_000, 800_000, 100_000_000, 4, 3);
        AddSimple(BAR, "Ronald", "Araujo", "Araujo", 25, "URY", 188, 84, RIGHT, CB, RB_f, 86, 90, 70_000_000, 8_000_000, 1_000_000_000, 4, 4);
        AddSimple(BAR, "Andreas", "Christensen", "Christensen", 28, "DNK", 188, 80, RIGHT, CB, CDM_f, 83, 84, 40_000_000, 6_000_000, 500_000_000, 3, 15);
        AddSimple(BAR, "Alejandro", "Balde", "Balde", 20, "ESP", 175, 68, LEFT, LB, LM_f, 81, 89, 50_000_000, 4_000_000, 1_000_000_000, 5, 3);
        AddSimple(BAR, "Jules", "Koundé", "Koundé", 25, "FRA", 180, 74, RIGHT, RB, CB_f, 85, 88, 60_000_000, 10_000_000, 1_000_000_000, 4, 23);
        AddSimple(BAR, "João", "Cancelo", "Cancelo", 30, "PRT", 182, 72, RIGHT, RB, LM_f, 83, 83, 30_000_000, 8_000_000, 300_000_000, 2, 2);
        AddSimple(BAR, "Marc", "Bernal", "M. Bernal", 17, "ESP", 185, 74, LEFT, CDM, 0, 68, 85, 5_000_000, 500_000, 500_000_000, 5, 16);
        AddSimple(BAR, "Marc", "Casadó", "Casadó", 20, "ESP", 180, 72, RIGHT, CDM, CM_f, 72, 82, 8_000_000, 1_000_000, 300_000_000, 5, 17);
        AddSimple(BAR, "Pedri", "", "Pedri", 21, "ESP", 174, 66, RIGHT, CM, CAM_f, 88, 93, 100_000_000, 14_000_000, 1_000_000_000, 5, 8);
        AddSimple(BAR, "Frenkie", "de Jong", "F. de Jong", 27, "NLD", 180, 74, RIGHT, CM, CDM_f, 86, 88, 60_000_000, 15_000_000, 1_000_000_000, 3, 21);
        AddSimple(BAR, "Gavi", "", "Gavi", 19, "ESP", 173, 68, RIGHT, CM, CAM_f, 83, 91, 70_000_000, 8_000_000, 1_000_000_000, 5, 6);
        AddSimple(BAR, "Fermín", "López", "Fermín", 21, "ESP", 176, 68, RIGHT, CM, CAM_f, 78, 87, 25_000_000, 3_000_000, 400_000_000, 5, 16);
        AddSimple(BAR, "Dani", "Olmo", "Dani Olmo", 26, "ESP", 180, 72, RIGHT, CAM, LW_f, 85, 87, 50_000_000, 10_000_000, 500_000_000, 4, 20);
        AddSimple(BAR, "Raphinha", "", "Raphinha", 27, "BRA", 176, 68, LEFT, RW, LW_f, 86, 86, 60_000_000, 12_000_000, 1_000_000_000, 4, 11);
        AddSimple(BAR, "Anthony", "Gordon", "Gordon", 23, "GBR", 183, 72, RIGHT, LW, RW_f, 83, 88, 50_000_000, 8_000_000, 500_000_000, 5, 10);
        AddSimple(BAR, "Lamine", "Yamal", "Lamine Yamal", 17, "ESP", 178, 68, LEFT, RW, LW_f, 83, 96, 120_000_000, 12_000_000, 1_000_000_000, 5, 19);
        AddSimple(BAR, "Roony", "Bardghji", "Bardghji", 18, "SWE", 173, 66, LEFT, RW, LW_f, 74, 89, 15_000_000, 2_000_000, 500_000_000, 5, 14);
        AddSimple(BAR, "Ferran", "Torres", "Ferran Torres", 24, "ESP", 184, 74, RIGHT, LW, RW_f, 81, 85, 30_000_000, 5_000_000, 500_000_000, 4, 7);

        // ══════════════════════════════════════════════════════════
        // ATLÉTICO DE MADRID (TeamId=3)
        // ══════════════════════════════════════════════════════════
        int ATM = 3;
        AddSimple(ATM, "Jan", "Oblak", "Oblak", 31, "SVN", 188, 84, RIGHT, GK, 0, 88, 88, 40_000_000, 12_000_000, 500_000_000, 3, 13);
        AddSimple(ATM, "Juan", "Musso", "Musso", 30, "ARG", 192, 84, RIGHT, GK, 0, 80, 82, 8_000_000, 3_000_000, 100_000_000, 2, 1);
        AddSimple(ATM, "Dávid", "Hancko", "Hancko", 26, "SVK", 188, 78, LEFT, CB, LB_f, 81, 85, 35_000_000, 4_000_000, 200_000_000, 4, 16);
        AddSimple(ATM, "Marc", "Pubill", "Pubill", 21, "ESP", 187, 76, RIGHT, RB, CB_f, 76, 84, 20_000_000, 2_000_000, 200_000_000, 5, 2);
        AddSimple(ATM, "Robin", "Le Normand", "Le Normand", 27, "ESP", 187, 80, RIGHT, CB, 0, 83, 85, 35_000_000, 6_000_000, 300_000_000, 4, 24);
        AddSimple(ATM, "José María", "Giménez", "Giménez", 29, "URY", 185, 80, RIGHT, CB, 0, 84, 85, 30_000_000, 7_000_000, 300_000_000, 3, 2);
        AddSimple(ATM, "Clément", "Lenglet", "Lenglet", 29, "FRA", 186, 81, LEFT, CB, 0, 77, 78, 10_000_000, 4_000_000, 100_000_000, 2, 15);
        AddSimple(ATM, "Matteo", "Ruggeri", "Ruggeri", 22, "ITA", 178, 70, LEFT, LB, LM_f, 78, 84, 25_000_000, 3_000_000, 200_000_000, 5, 3);
        AddSimple(ATM, "Marcos", "Llorente", "Llorente", 29, "ESP", 184, 76, RIGHT, CM, RB_f, 82, 83, 25_000_000, 7_000_000, 300_000_000, 3, 14);
        AddSimple(ATM, "Nahuel", "Molina", "Molina", 26, "ARG", 175, 70, RIGHT, RB, RM_f, 82, 84, 30_000_000, 5_000_000, 300_000_000, 4, 16);
        AddSimple(ATM, "Pablo", "Barrios", "Barrios", 21, "ESP", 181, 72, RIGHT, CM, CDM_f, 78, 86, 30_000_000, 3_000_000, 300_000_000, 5, 8);
        AddSimple(ATM, "Rodrigo", "Mendoza", "Mendoza", 19, "ARG", 175, 68, RIGHT, CM, 0, 68, 82, 5_000_000, 600_000, 100_000_000, 5, 26);
        AddSimple(ATM, "Johnny", "Cardoso", "Cardoso", 23, "USA", 185, 78, RIGHT, CDM, CM_f, 76, 82, 15_000_000, 2_000_000, 150_000_000, 4, 4);
        AddSimple(ATM, "Obed", "Vargas", "Vargas", 19, "USA", 178, 70, RIGHT, CDM, CM_f, 66, 80, 3_000_000, 400_000, 80_000_000, 5, 28);
        AddSimple(ATM, "Koke", "", "Koke", 32, "ESP", 176, 72, RIGHT, CM, CDM_f, 83, 83, 15_000_000, 10_000_000, 200_000_000, 2, 6);
        AddSimple(ATM, "Álex", "Baena", "Baena", 23, "ESP", 174, 66, RIGHT, CAM, LW_f, 82, 87, 40_000_000, 5_000_000, 300_000_000, 5, 20);
        AddSimple(ATM, "Ademola", "Lookman", "Lookman", 26, "NGA", 174, 72, RIGHT, LW, RW_f, 83, 85, 35_000_000, 6_000_000, 300_000_000, 3, 11);
        AddSimple(ATM, "Nico", "González", "Nico González", 22, "ESP", 188, 76, LEFT, CM, CAM_f, 79, 85, 30_000_000, 3_500_000, 300_000_000, 5, 18);
        AddSimple(ATM, "Thiago", "Almada", "Almada", 23, "ARG", 171, 66, RIGHT, CAM, RW_f, 81, 87, 35_000_000, 4_000_000, 300_000_000, 5, 10);
        AddSimple(ATM, "Giuliano", "Simeone", "G. Simeone", 21, "ARG", 175, 68, RIGHT, RW, LW_f, 74, 82, 12_000_000, 1_500_000, 150_000_000, 5, 17);
        AddSimple(ATM, "Antoine", "Griezmann", "Griezmann", 33, "FRA", 176, 68, LEFT, ST, LW_f, 87, 87, 25_000_000, 12_000_000, 200_000_000, 2, 7);
        AddSimple(ATM, "Julián", "Alvarez", "Julián", 24, "ARG", 170, 72, RIGHT, ST, CF_f, 84, 89, 75_000_000, 10_000_000, 500_000_000, 5, 19);
        AddSimple(ATM, "Alexander", "Sørloth", "Sørloth", 29, "NOR", 195, 90, LEFT, ST, CF_f, 80, 81, 20_000_000, 5_000_000, 200_000_000, 3, 9);

        // ══════════════════════════════════════════════════════════
        // REAL SOCIEDAD (TeamId=4)
        // ══════════════════════════════════════════════════════════
        int RSO = 4;
        AddSimple(RSO, "Álex", "Remiro", "Remiro", 29, "ESP", 192, 82, RIGHT, GK, 0, 84, 85, 25_000_000, 6_000_000, 200_000_000, 4, 1);
        AddSimple(RSO, "Unai", "Marrero", "Marrero", 23, "ESP", 188, 80, RIGHT, GK, 0, 72, 80, 4_000_000, 800_000, 60_000_000, 5, 13);
        AddSimple(RSO, "Jon", "Martín", "Jon Martín", 18, "ESP", 184, 76, RIGHT, CB, 0, 68, 84, 5_000_000, 600_000, 100_000_000, 5, 3);
        AddSimple(RSO, "Igor", "Zubeldia", "Zubeldia", 27, "ESP", 185, 76, RIGHT, CB, 0, 82, 84, 25_000_000, 5_000_000, 200_000_000, 4, 5);
        AddSimple(RSO, "Duje", "Caleta-Car", "Caleta-Car", 28, "HRV", 192, 84, RIGHT, CB, 0, 77, 78, 8_000_000, 3_000_000, 80_000_000, 2, 15);
        AddSimple(RSO, "Aritz", "Elustondo", "Elustondo", 30, "ESP", 185, 76, RIGHT, CB, RB_f, 78, 79, 6_000_000, 3_000_000, 60_000_000, 2, 6);
        AddSimple(RSO, "Sergio", "Gómez", "S. Gómez", 32, "ESP", 172, 68, LEFT, LB, LM_f, 78, 78, 5_000_000, 3_000_000, 50_000_000, 2, 17);
        AddSimple(RSO, "Aihen", "Muñoz", "Aihen", 27, "ESP", 175, 68, LEFT, LB, 0, 76, 78, 5_000_000, 1_500_000, 60_000_000, 3, 12);
        AddSimple(RSO, "Jon", "Aramburu", "Aramburu", 22, "ESP", 178, 70, RIGHT, RB, LB_f, 76, 82, 12_000_000, 1_500_000, 100_000_000, 5, 19);
        AddSimple(RSO, "Álvaro", "Odriozola", "Odriozola", 28, "ESP", 178, 70, RIGHT, RB, 0, 77, 78, 6_000_000, 2_500_000, 60_000_000, 3, 2);
        AddSimple(RSO, "Jon", "Gorrotxategi", "Gorrotxategi", 22, "ESP", 178, 72, RIGHT, CDM, CM_f, 72, 80, 5_000_000, 1_000_000, 80_000_000, 5, 22);
        AddSimple(RSO, "Beñat", "Turrientes", "Turrientes", 22, "ESP", 180, 72, RIGHT, CM, CDM_f, 78, 85, 20_000_000, 2_500_000, 200_000_000, 5, 8);
        AddSimple(RSO, "Luka", "Sucic", "Sucic", 22, "HRV", 185, 76, RIGHT, CM, CAM_f, 79, 86, 25_000_000, 3_000_000, 200_000_000, 5, 7);
        AddSimple(RSO, "Carlos", "Soler", "Carlos Soler", 27, "ESP", 180, 74, RIGHT, CM, CAM_f, 80, 81, 15_000_000, 5_000_000, 100_000_000, 3, 16);
        AddSimple(RSO, "Yangel", "Herrera", "Herrera", 26, "VEN", 184, 74, RIGHT, CM, CDM_f, 78, 80, 10_000_000, 3_000_000, 80_000_000, 3, 23);
        AddSimple(RSO, "Arsen", "Zakharyan", "Zakharyan", 21, "RUS", 180, 70, RIGHT, CAM, CM_f, 76, 84, 15_000_000, 2_000_000, 150_000_000, 5, 10);
        AddSimple(RSO, "Brais", "Méndez", "Brais Méndez", 27, "ESP", 182, 72, LEFT, CAM, RW_f, 82, 83, 20_000_000, 5_000_000, 150_000_000, 3, 21);
        AddSimple(RSO, "Pablo", "Marín", "P. Marín", 21, "ESP", 172, 66, RIGHT, CM, CAM_f, 72, 82, 6_000_000, 1_000_000, 80_000_000, 5, 27);
        AddSimple(RSO, "Ander", "Barrenetxea", "Barrenetxea", 22, "ESP", 175, 68, LEFT, LW, RW_f, 78, 85, 20_000_000, 3_000_000, 200_000_000, 5, 11);
        AddSimple(RSO, "Gonçalo", "Guedes", "Guedes", 27, "PRT", 178, 68, RIGHT, LW, RW_f, 77, 78, 8_000_000, 3_000_000, 80_000_000, 2, 9);
        AddSimple(RSO, "Wesley", "", "Wesley", 25, "BRA", 175, 70, RIGHT, LW, RW_f, 76, 78, 6_000_000, 2_000_000, 60_000_000, 3, 24);
        AddSimple(RSO, "Takefusa", "Kubo", "Kubo", 23, "JPN", 173, 66, LEFT, RW, LW_f, 82, 87, 40_000_000, 5_000_000, 300_000_000, 5, 14);
        AddSimple(RSO, "Mikel", "Oyarzabal", "Oyarzabal", 27, "ESP", 181, 74, LEFT, LW, RW_f, 85, 86, 35_000_000, 8_000_000, 300_000_000, 4, 10);
        AddSimple(RSO, "Orri", "Óskarsson", "Óskarsson", 20, "ISL", 190, 80, RIGHT, ST, CF_f, 74, 84, 12_000_000, 1_500_000, 100_000_000, 5, 18);
        AddSimple(RSO, "Jon", "Karrikaburu", "Karrikaburu", 22, "ESP", 186, 78, RIGHT, ST, 0, 72, 80, 4_000_000, 800_000, 60_000_000, 5, 20);

        // ══════════════════════════════════════════════════════════
        // ATHLETIC CLUB (TeamId=5)
        // ══════════════════════════════════════════════════════════
        int ATH = 5;
        AddSimple(ATH, "Unai", "Simón", "Unai Simón", 27, "ESP", 190, 82, RIGHT, GK, 0, 86, 88, 35_000_000, 8_000_000, 300_000_000, 4, 1);
        AddSimple(ATH, "Álex", "Padilla", "Padilla", 21, "ESP", 190, 80, RIGHT, GK, 0, 70, 80, 4_000_000, 600_000, 80_000_000, 5, 13);
        AddSimple(ATH, "Dani", "Vivian", "Vivian", 25, "ESP", 184, 78, RIGHT, CB, RB_f, 82, 85, 30_000_000, 4_000_000, 200_000_000, 5, 3);
        AddSimple(ATH, "Aymeric", "Laporte", "Laporte", 30, "ESP", 191, 85, LEFT, CB, 0, 84, 84, 20_000_000, 8_000_000, 200_000_000, 3, 14);
        AddSimple(ATH, "Aitor", "Paredes", "Paredes", 24, "ESP", 184, 76, RIGHT, CB, 0, 78, 82, 15_000_000, 2_500_000, 120_000_000, 5, 4);
        AddSimple(ATH, "Yeray", "Álvarez", "Yeray", 29, "ESP", 183, 78, RIGHT, CB, 0, 80, 81, 15_000_000, 4_000_000, 150_000_000, 3, 5);
        AddSimple(ATH, "Unai", "Egiluz", "Egiluz", 22, "ESP", 186, 78, RIGHT, CB, 0, 72, 80, 5_000_000, 800_000, 80_000_000, 5, 21);
        AddSimple(ATH, "Adama", "Boiro", "Boiro", 22, "ESP", 180, 72, LEFT, LB, LM_f, 72, 80, 4_000_000, 800_000, 80_000_000, 5, 19);
        AddSimple(ATH, "Yuri", "Berchiche", "Yuri", 34, "ESP", 181, 74, LEFT, LB, LM_f, 79, 79, 5_000_000, 4_000_000, 80_000_000, 2, 17);
        AddSimple(ATH, "Jesús", "Areso", "Areso", 25, "ESP", 182, 74, RIGHT, RB, 0, 74, 78, 5_000_000, 1_200_000, 60_000_000, 4, 15);
        AddSimple(ATH, "Andoni", "Gorosabel", "Gorosabel", 28, "ESP", 174, 70, RIGHT, RB, 0, 76, 77, 6_000_000, 2_000_000, 60_000_000, 3, 2);
        AddSimple(ATH, "Iñigo", "Lekue", "Lekue", 31, "ESP", 180, 74, RIGHT, RB, LB_f, 76, 76, 4_000_000, 2_500_000, 50_000_000, 2, 16);
        AddSimple(ATH, "Mikel", "Vesga", "Vesga", 31, "ESP", 191, 82, LEFT, CDM, CM_f, 78, 78, 6_000_000, 3_000_000, 60_000_000, 2, 6);
        AddSimple(ATH, "Mikel", "Jauregizar", "Jauregizar", 20, "ESP", 175, 68, RIGHT, CM, CAM_f, 74, 82, 10_000_000, 1_200_000, 100_000_000, 5, 20);
        AddSimple(ATH, "Beñat", "Prados", "Prados", 23, "ESP", 178, 72, RIGHT, CM, CDM_f, 76, 82, 12_000_000, 2_000_000, 100_000_000, 5, 24);
        AddSimple(ATH, "Iñigo", "Ruiz de Galarreta", "Ruiz de Galarreta", 31, "ESP", 174, 70, RIGHT, CM, CDM_f, 76, 76, 3_000_000, 2_000_000, 40_000_000, 2, 22);
        AddSimple(ATH, "Oihan", "Sancet", "Sancet", 24, "ESP", 188, 78, RIGHT, CAM, CM_f, 82, 87, 40_000_000, 5_000_000, 300_000_000, 5, 8);
        AddSimple(ATH, "Unai", "Gómez", "U. Gómez", 21, "ESP", 182, 74, RIGHT, CM, CAM_f, 76, 84, 15_000_000, 2_000_000, 150_000_000, 5, 23);
        AddSimple(ATH, "Nico", "Williams", "Nico Williams", 22, "ESP", 181, 72, RIGHT, LW, RW_f, 85, 90, 70_000_000, 8_000_000, 500_000_000, 5, 10);
        AddSimple(ATH, "Álex", "Berenguer", "Berenguer", 29, "ESP", 175, 72, RIGHT, RW, LW_f, 80, 81, 12_000_000, 3_500_000, 100_000_000, 3, 7);
        AddSimple(ATH, "Nico", "Serrano", "N. Serrano", 21, "ESP", 176, 68, RIGHT, LW, RW_f, 72, 82, 6_000_000, 1_000_000, 80_000_000, 5, 18);
        AddSimple(ATH, "Iñaki", "Williams", "Iñaki Williams", 30, "GHA", 186, 78, RIGHT, ST, RW_f, 82, 82, 15_000_000, 7_000_000, 150_000_000, 3, 9);
        AddSimple(ATH, "Robert", "Navarro", "R. Navarro", 22, "ESP", 178, 72, RIGHT, RW, LW_f, 74, 82, 8_000_000, 1_500_000, 100_000_000, 5, 11);
        AddSimple(ATH, "Gorka", "Guruzeta", "Guruzeta", 28, "ESP", 188, 80, RIGHT, ST, 0, 78, 79, 8_000_000, 2_500_000, 80_000_000, 3, 12);
        AddSimple(ATH, "Maroan", "Sannadi", "Sannadi", 23, "ESP", 192, 84, RIGHT, ST, 0, 72, 78, 4_000_000, 800_000, 50_000_000, 4, 25);

        // ══════════════════════════════════════════════════════════
        // REAL BETIS (TeamId=6)
        // ══════════════════════════════════════════════════════════
        int BET = 6;
        AddSimple(BET, "Álvaro", "Valles", "Valles", 27, "ESP", 188, 80, RIGHT, GK, 0, 80, 82, 12_000_000, 3_000_000, 100_000_000, 4, 1);
        AddSimple(BET, "Pau", "López", "Pau López", 29, "ESP", 190, 82, LEFT, GK, 0, 78, 79, 5_000_000, 3_000_000, 60_000_000, 2, 13);
        AddSimple(BET, "Adrián", "San Miguel", "Adrián", 37, "ESP", 190, 86, RIGHT, GK, 0, 75, 75, 1_000_000, 2_000_000, 20_000_000, 1, 25);
        AddSimple(BET, "Natan", "", "Natan", 23, "BRA", 188, 82, RIGHT, CB, 0, 76, 82, 15_000_000, 2_000_000, 120_000_000, 5, 4);
        AddSimple(BET, "Valentín", "Gómez", "V. Gómez", 21, "ARG", 186, 78, LEFT, CB, 0, 74, 84, 15_000_000, 2_000_000, 150_000_000, 5, 5);
        AddSimple(BET, "Diego", "Llorente", "D. Llorente", 31, "ESP", 186, 80, RIGHT, CB, 0, 79, 80, 6_000_000, 3_500_000, 60_000_000, 2, 6);
        AddSimple(BET, "Marc", "Bartra", "Bartra", 33, "ESP", 184, 76, RIGHT, CB, 0, 78, 78, 3_000_000, 3_000_000, 40_000_000, 2, 15);
        AddSimple(BET, "Junior", "Firpo", "Firpo", 28, "DOM", 184, 74, LEFT, LB, LM_f, 78, 79, 8_000_000, 3_000_000, 80_000_000, 3, 3);
        AddSimple(BET, "Ricardo", "Rodríguez", "R. Rodríguez", 32, "CHE", 180, 76, LEFT, LB, CB_f, 76, 76, 3_000_000, 2_500_000, 40_000_000, 2, 12);
        AddSimple(BET, "Héctor", "Bellerín", "Bellerín", 29, "ESP", 178, 74, RIGHT, RB, 0, 78, 79, 6_000_000, 3_500_000, 60_000_000, 3, 2);
        AddSimple(BET, "Sofyan", "Amrabat", "Amrabat", 28, "MAR", 185, 76, RIGHT, CDM, CM_f, 79, 80, 12_000_000, 4_000_000, 100_000_000, 2, 8);
        AddSimple(BET, "Marc", "Roca", "Marc Roca", 27, "ESP", 184, 76, LEFT, CM, CDM_f, 78, 80, 10_000_000, 3_000_000, 80_000_000, 3, 21);
        AddSimple(BET, "Sergi", "Altimira", "Altimira", 23, "ESP", 182, 74, RIGHT, CM, CDM_f, 74, 80, 6_000_000, 1_500_000, 60_000_000, 4, 16);
        AddSimple(BET, "Nelson", "Deossa", "Deossa", 24, "COL", 182, 76, RIGHT, CM, CDM_f, 74, 80, 6_000_000, 1_500_000, 60_000_000, 4, 24);
        AddSimple(BET, "Pablo", "Fornals", "Fornals", 28, "ESP", 178, 70, RIGHT, CM, CAM_f, 79, 80, 10_000_000, 4_000_000, 80_000_000, 3, 10);
        AddSimple(BET, "Giovani", "Lo Celso", "Lo Celso", 28, "ARG", 177, 70, LEFT, CAM, CM_f, 81, 82, 15_000_000, 5_000_000, 100_000_000, 3, 20);
        AddSimple(BET, "Isco", "", "Isco", 32, "ESP", 176, 70, RIGHT, CAM, CM_f, 83, 83, 10_000_000, 7_000_000, 100_000_000, 2, 22);
        AddSimple(BET, "Abde", "Ezzalzouli", "Abde", 22, "MAR", 177, 68, RIGHT, LW, RW_f, 78, 84, 20_000_000, 2_500_000, 150_000_000, 5, 11);
        AddSimple(BET, "Rodrigo", "Riquelme", "Riquelme", 24, "ESP", 174, 66, RIGHT, LW, RW_f, 78, 82, 15_000_000, 3_000_000, 120_000_000, 5, 18);
        AddSimple(BET, "Antony", "", "Antony", 24, "BRA", 174, 66, LEFT, RW, 0, 78, 82, 20_000_000, 6_000_000, 150_000_000, 3, 7);
        AddSimple(BET, "Aitor", "Ruibal", "Ruibal", 28, "ESP", 175, 72, RIGHT, RB, RW_f, 76, 77, 5_000_000, 2_000_000, 50_000_000, 3, 23);
        AddSimple(BET, "Cucho", "Hernández", "Cucho", 25, "COL", 175, 72, RIGHT, ST, LW_f, 80, 83, 20_000_000, 3_500_000, 150_000_000, 4, 9);
        AddSimple(BET, "Chimy", "Ávila", "Chimy Ávila", 30, "ARG", 172, 72, RIGHT, ST, LW_f, 77, 78, 5_000_000, 3_000_000, 60_000_000, 2, 17);
        AddSimple(BET, "Cédric", "Bakambu", "Bakambu", 33, "COD", 182, 76, RIGHT, ST, 0, 76, 76, 3_000_000, 2_500_000, 40_000_000, 2, 14);

        // ══════════════════════════════════════════════════════════
        // VILLARREAL CF (TeamId=7)
        // ══════════════════════════════════════════════════════════
        int VIL = 7;
        AddSimple(VIL, "Luiz", "Júnior", "Luiz Jr.", 23, "BRA", 196, 86, RIGHT, GK, 0, 78, 84, 12_000_000, 2_000_000, 100_000_000, 5, 1);
        AddSimple(VIL, "Diego", "Conde", "D. Conde", 25, "ESP", 188, 80, RIGHT, GK, 0, 76, 80, 5_000_000, 1_500_000, 60_000_000, 4, 13);
        AddSimple(VIL, "Renato", "Veiga", "Veiga", 21, "PRT", 190, 82, LEFT, CB, CDM_f, 78, 86, 30_000_000, 3_000_000, 300_000_000, 5, 5);
        AddSimple(VIL, "Logan", "Costa", "L. Costa", 23, "CPV", 191, 82, RIGHT, CB, 0, 75, 82, 12_000_000, 1_500_000, 100_000_000, 5, 3);
        AddSimple(VIL, "Pau", "Navarro", "Pau Navarro", 19, "ESP", 182, 74, RIGHT, CB, RB_f, 68, 82, 5_000_000, 600_000, 100_000_000, 5, 26);
        AddSimple(VIL, "Rafa", "Marín", "Rafa Marín", 22, "ESP", 191, 82, RIGHT, CB, 0, 76, 84, 15_000_000, 2_000_000, 150_000_000, 5, 4);
        AddSimple(VIL, "Juan", "Foyth", "Foyth", 26, "ARG", 188, 78, RIGHT, RB, CB_f, 80, 83, 20_000_000, 4_000_000, 150_000_000, 3, 8);
        AddSimple(VIL, "Willy", "Kambwala", "Kambwala", 20, "FRA", 192, 84, RIGHT, CB, RB_f, 72, 84, 8_000_000, 1_000_000, 100_000_000, 5, 14);
        AddSimple(VIL, "Sergi", "Cardona", "Cardona", 25, "ESP", 186, 76, LEFT, LB, LM_f, 76, 80, 8_000_000, 1_800_000, 80_000_000, 4, 2);
        AddSimple(VIL, "Alfonso", "Pedraza", "Pedraza", 28, "ESP", 182, 74, LEFT, LB, LM_f, 78, 79, 10_000_000, 3_000_000, 100_000_000, 2, 24);
        AddSimple(VIL, "Santiago", "Mouriño", "Mouriño", 22, "URY", 188, 78, RIGHT, CB, CDM_f, 74, 82, 10_000_000, 1_500_000, 100_000_000, 5, 6);
        AddSimple(VIL, "Alex", "Freeman", "Freeman", 20, "GBR", 180, 72, RIGHT, RB, RM_f, 70, 80, 4_000_000, 800_000, 80_000_000, 5, 22);
        AddSimple(VIL, "Thomas", "Partey", "Partey", 31, "GHA", 185, 76, RIGHT, CDM, CM_f, 81, 82, 15_000_000, 6_000_000, 150_000_000, 2, 20);
        AddSimple(VIL, "Pape", "Gueye", "P. Gueye", 25, "SEN", 189, 80, RIGHT, CDM, CM_f, 78, 82, 15_000_000, 2_500_000, 120_000_000, 4, 18);
        AddSimple(VIL, "Santi", "Comesaña", "Comesaña", 27, "ESP", 182, 76, RIGHT, CM, CDM_f, 76, 79, 8_000_000, 2_500_000, 80_000_000, 3, 10);
        AddSimple(VIL, "Dani", "Parejo", "Parejo", 35, "ESP", 182, 76, RIGHT, CM, 0, 82, 82, 5_000_000, 5_000_000, 80_000_000, 2, 11);
        AddSimple(VIL, "Alberto", "Moleiro", "Moleiro", 21, "ESP", 172, 64, RIGHT, CAM, LW_f, 78, 87, 30_000_000, 3_000_000, 300_000_000, 5, 16);
        AddSimple(VIL, "Tajon", "Buchanan", "Buchanan", 25, "CAN", 183, 70, RIGHT, RW, RB_f, 75, 80, 8_000_000, 1_500_000, 80_000_000, 3, 12);
        AddSimple(VIL, "Nicolas", "Pépé", "Pépé", 29, "CIV", 183, 72, LEFT, RW, LW_f, 76, 77, 6_000_000, 3_000_000, 60_000_000, 2, 19);
        AddSimple(VIL, "Georges", "Mikautadze", "Mikautadze", 24, "GEO", 176, 70, RIGHT, CF, ST_f, 78, 82, 15_000_000, 2_500_000, 120_000_000, 4, 9);
        AddSimple(VIL, "Tani", "Oluwaseyi", "Oluwaseyi", 24, "CAN", 188, 80, RIGHT, ST, 0, 74, 80, 6_000_000, 1_200_000, 60_000_000, 4, 21);
        AddSimple(VIL, "Ayoze", "Pérez", "Ayoze", 31, "ESP", 179, 72, RIGHT, ST, LW_f, 80, 80, 10_000_000, 4_000_000, 100_000_000, 2, 7);
        AddSimple(VIL, "Gerard", "Moreno", "Gerard", 32, "ESP", 177, 72, RIGHT, ST, CF_f, 83, 83, 12_000_000, 6_000_000, 120_000_000, 2, 17);

        // ══════════════════════════════════════════════════════════
        // SEVILLA FC (TeamId=8)
        // ══════════════════════════════════════════════════════════
        int SEV = 8;
        AddSimple(SEV, "Odysseas", "Vlachodimos", "Vlachodimos", 30, "GRC", 188, 80, RIGHT, GK, 0, 76, 77, 4_000_000, 2_000_000, 40_000_000, 2, 1);
        AddSimple(SEV, "Ørjan", "Nyland", "Nyland", 34, "NOR", 192, 86, RIGHT, GK, 0, 76, 76, 2_000_000, 2_000_000, 30_000_000, 2, 13);
        AddSimple(SEV, "Kike", "Salas", "Kike Salas", 22, "ESP", 186, 78, LEFT, CB, LB_f, 76, 84, 15_000_000, 2_000_000, 150_000_000, 5, 4);
        AddSimple(SEV, "Andrés", "Castrín", "Castrín", 21, "ESP", 184, 76, RIGHT, CB, 0, 68, 80, 3_000_000, 600_000, 60_000_000, 5, 25);
        AddSimple(SEV, "Marcão", "", "Marcão", 28, "BRA", 186, 80, LEFT, CB, 0, 76, 78, 6_000_000, 2_500_000, 60_000_000, 2, 3);
        AddSimple(SEV, "Fábio", "Cardoso", "F. Cardoso", 30, "PRT", 188, 80, RIGHT, CB, CDM_f, 77, 78, 5_000_000, 2_500_000, 50_000_000, 2, 14);
        AddSimple(SEV, "Tanguy", "Nianzou", "Nianzou", 22, "FRA", 191, 82, RIGHT, CB, CDM_f, 76, 82, 12_000_000, 2_500_000, 100_000_000, 4, 15);
        AddSimple(SEV, "César", "Azpilicueta", "Azpilicueta", 35, "ESP", 178, 74, RIGHT, RB, CB_f, 80, 80, 3_000_000, 5_000_000, 30_000_000, 1, 12);
        AddSimple(SEV, "Joaquín", "Martínez Gauna", "Oso", 22, "ESP", 185, 76, RIGHT, CB, 0, 72, 80, 5_000_000, 800_000, 60_000_000, 5, 24);
        AddSimple(SEV, "Gabriel", "Suazo", "Suazo", 27, "CHL", 176, 70, LEFT, LB, LM_f, 78, 80, 10_000_000, 2_500_000, 80_000_000, 4, 21);
        AddSimple(SEV, "José Ángel", "Carmona", "Carmona", 22, "ESP", 178, 72, RIGHT, RB, RW_f, 76, 82, 12_000_000, 1_500_000, 100_000_000, 5, 2);
        AddSimple(SEV, "Juanlu", "Sánchez", "Juanlu", 21, "ESP", 180, 72, RIGHT, RB, CM_f, 76, 84, 18_000_000, 2_000_000, 150_000_000, 5, 6);
        AddSimple(SEV, "Lucien", "Agoumé", "Agoumé", 22, "FRA", 185, 78, RIGHT, CDM, CM_f, 76, 82, 12_000_000, 2_000_000, 100_000_000, 5, 18);
        AddSimple(SEV, "Batista", "Mendy", "B. Mendy", 24, "SEN", 190, 82, RIGHT, CDM, CM_f, 75, 82, 10_000_000, 1_500_000, 80_000_000, 4, 22);
        AddSimple(SEV, "Nemanja", "Gudelj", "Gudelj", 32, "SRB", 187, 80, RIGHT, CM, CDM_f, 78, 78, 4_000_000, 3_000_000, 50_000_000, 2, 10);
        AddSimple(SEV, "Djibril", "Sow", "Sow", 27, "CHE", 184, 76, RIGHT, CM, CDM_f, 78, 79, 8_000_000, 3_000_000, 60_000_000, 3, 8);
        AddSimple(SEV, "Joan", "Jordán", "Jordán", 30, "ESP", 184, 76, RIGHT, CM, CDM_f, 78, 78, 5_000_000, 3_500_000, 50_000_000, 2, 20);
        AddSimple(SEV, "Rubén", "Vargas", "Vargas", 26, "CHE", 178, 74, RIGHT, RW, 0, 77, 80, 8_000_000, 2_000_000, 60_000_000, 4, 7);
        AddSimple(SEV, "Chidera", "Ejuke", "Ejuke", 26, "NGA", 176, 68, RIGHT, LW, RW_f, 76, 78, 6_000_000, 2_000_000, 50_000_000, 3, 11);
        AddSimple(SEV, "Adnan", "Januzaj", "Januzaj", 29, "BEL", 186, 74, LEFT, RW, 0, 76, 77, 4_000_000, 2_500_000, 40_000_000, 2, 16);
        AddSimple(SEV, "Peque", "Fernández", "Peque", 21, "ESP", 178, 72, RIGHT, ST, LW_f, 74, 84, 12_000_000, 1_500_000, 100_000_000, 5, 17);
        AddSimple(SEV, "Alexis", "Sánchez", "Alexis", 35, "CHL", 168, 64, RIGHT, ST, LW_f, 80, 80, 3_000_000, 5_000_000, 30_000_000, 2, 9);
        AddSimple(SEV, "Akor", "Adams", "Adams", 24, "NGA", 190, 82, RIGHT, ST, 0, 74, 80, 6_000_000, 1_500_000, 60_000_000, 4, 26);
        AddSimple(SEV, "Isaac", "Romero", "Isaac", 24, "ESP", 182, 76, RIGHT, ST, LW_f, 76, 82, 10_000_000, 2_000_000, 80_000_000, 5, 19);
        AddSimple(SEV, "Neal", "Maupay", "Maupay", 28, "FRA", 173, 68, RIGHT, ST, 0, 76, 77, 5_000_000, 2_500_000, 40_000_000, 2, 23);

        // ══════════════════════════════════════════════════════════
        // VALENCIA CF (TeamId=9)
        // ══════════════════════════════════════════════════════════
        int VAL = 9;
        AddSimple(VAL, "Julen", "Agirrezabala", "Agirrezabala", 23, "ESP", 187, 80, RIGHT, GK, 0, 78, 85, 18_000_000, 2_500_000, 100_000_000, 5, 1);
        AddSimple(VAL, "Stole", "Dimitrievski", "Dimitrievski", 30, "MKD", 188, 82, RIGHT, GK, 0, 75, 76, 3_000_000, 1_500_000, 40_000_000, 2, 13);
        AddSimple(VAL, "César", "Tárrega", "Tárrega", 22, "ESP", 186, 78, RIGHT, CB, 0, 72, 80, 5_000_000, 800_000, 80_000_000, 5, 3);
        AddSimple(VAL, "José", "Copete", "Copete", 25, "ESP", 188, 80, RIGHT, CB, 0, 74, 78, 5_000_000, 1_000_000, 60_000_000, 3, 12);
        AddSimple(VAL, "Unai", "Núñez", "U. Núñez", 27, "ESP", 186, 78, RIGHT, CB, 0, 76, 78, 6_000_000, 1_500_000, 70_000_000, 3, 5);
        AddSimple(VAL, "Eray", "Cömert", "Cömert", 26, "CHE", 183, 78, RIGHT, CB, 0, 75, 78, 5_000_000, 1_500_000, 60_000_000, 2, 15);
        AddSimple(VAL, "Mouctar", "Diakhaby", "Diakhaby", 27, "GIN", 192, 84, RIGHT, CB, 0, 76, 78, 6_000_000, 2_000_000, 80_000_000, 2, 4);
        AddSimple(VAL, "José", "Gayà", "Gayà", 29, "ESP", 172, 66, LEFT, LB, LM_f, 83, 84, 25_000_000, 6_000_000, 200_000_000, 3, 14);
        AddSimple(VAL, "Jesús", "Vázquez", "J. Vázquez", 21, "ESP", 178, 70, LEFT, LB, 0, 70, 78, 3_000_000, 600_000, 60_000_000, 5, 21);
        AddSimple(VAL, "Thierry", "Correia", "Correia", 25, "PRT", 176, 70, RIGHT, RB, 0, 73, 78, 5_000_000, 1_200_000, 60_000_000, 3, 2);
        AddSimple(VAL, "Dimitri", "Foulquier", "Foulquier", 31, "FRA", 183, 74, RIGHT, RB, 0, 73, 73, 2_500_000, 1_500_000, 40_000_000, 2, 20);
        AddSimple(VAL, "Pepelu", "", "Pepelu", 26, "ESP", 186, 76, RIGHT, CDM, CM_f, 78, 80, 12_000_000, 3_000_000, 100_000_000, 4, 8);
        AddSimple(VAL, "Guido", "Rodríguez", "G. Rodríguez", 30, "ARG", 185, 80, RIGHT, CDM, CM_f, 80, 81, 12_000_000, 4_000_000, 100_000_000, 2, 6);
        AddSimple(VAL, "Baptiste", "Santamaría", "Santamaría", 29, "FRA", 183, 76, RIGHT, CM, CDM_f, 77, 78, 8_000_000, 2_500_000, 80_000_000, 2, 10);
        AddSimple(VAL, "Javi", "Guerra", "J. Guerra", 21, "ESP", 182, 74, RIGHT, CM, CAM_f, 77, 86, 25_000_000, 2_500_000, 200_000_000, 5, 18);
        AddSimple(VAL, "Filip", "Ugrinic", "Ugrinic", 25, "CHE", 182, 74, RIGHT, CM, 0, 74, 78, 5_000_000, 1_500_000, 60_000_000, 3, 16);
        AddSimple(VAL, "André", "Almeida", "A. Almeida", 24, "PRT", 172, 66, RIGHT, CM, CAM_f, 76, 81, 12_000_000, 2_000_000, 100_000_000, 5, 17);
        AddSimple(VAL, "Largie", "Ramazani", "Ramazani", 23, "BEL", 168, 64, RIGHT, LW, RW_f, 75, 80, 8_000_000, 1_500_000, 80_000_000, 4, 7);
        AddSimple(VAL, "Arnaut", "Danjuma", "Danjuma", 27, "NLD", 178, 70, RIGHT, LW, RW_f, 78, 80, 12_000_000, 3_000_000, 100_000_000, 2, 11);
        AddSimple(VAL, "Diego", "López", "Diego López", 20, "ESP", 176, 68, RIGHT, RW, LW_f, 74, 84, 12_000_000, 1_500_000, 150_000_000, 5, 19);
        AddSimple(VAL, "Luis", "Rioja", "Rioja", 30, "ESP", 176, 68, RIGHT, LW, 0, 77, 77, 5_000_000, 2_000_000, 60_000_000, 2, 22);
        AddSimple(VAL, "Dani", "Raba", "Raba", 27, "ESP", 174, 68, RIGHT, RW, CAM_f, 74, 76, 4_000_000, 1_500_000, 50_000_000, 2, 23);
        AddSimple(VAL, "Hugo", "Duro", "Duro", 24, "ESP", 182, 74, RIGHT, ST, CF_f, 78, 82, 15_000_000, 3_000_000, 150_000_000, 4, 9);
        AddSimple(VAL, "Lucas", "Beltrán", "Beltrán", 23, "ARG", 178, 72, RIGHT, ST, CF_f, 75, 82, 12_000_000, 2_000_000, 120_000_000, 5, 24);
        AddSimple(VAL, "Umar", "Sadiq", "Sadiq", 27, "NGA", 192, 82, RIGHT, ST, 0, 76, 78, 6_000_000, 2_000_000, 60_000_000, 2, 25);

        // ══════════════════════════════════════════════════════════
        // CA OSASUNA (TeamId=10)
        // ══════════════════════════════════════════════════════════
        int OSA = 10;
        AddSimple(OSA, "Sergio", "Herrera", "S. Herrera", 31, "ESP", 192, 84, RIGHT, GK, 0, 80, 81, 8_000_000, 3_000_000, 60_000_000, 3, 1);
        AddSimple(OSA, "Aitor", "Fernández", "A. Fernández", 33, "ESP", 186, 80, RIGHT, GK, 0, 76, 77, 2_000_000, 1_500_000, 20_000_000, 2, 13);
        AddSimple(OSA, "Enzo", "Boyomo", "Boyomo", 23, "CMR", 186, 78, RIGHT, CB, 0, 76, 82, 10_000_000, 1_500_000, 80_000_000, 5, 4);
        AddSimple(OSA, "Jorge", "Herrando", "Herrando", 23, "ESP", 188, 80, RIGHT, CB, 0, 74, 82, 8_000_000, 1_200_000, 80_000_000, 5, 3);
        AddSimple(OSA, "Alejandro", "Catena", "Catena", 29, "ESP", 186, 80, RIGHT, CB, 0, 78, 80, 6_000_000, 2_500_000, 60_000_000, 3, 6);
        AddSimple(OSA, "Abel", "Bretones", "Bretones", 24, "ESP", 178, 72, LEFT, LB, LM_f, 76, 82, 8_000_000, 1_500_000, 80_000_000, 5, 22);
        AddSimple(OSA, "Javi", "Galán", "Javi Galán", 29, "ESP", 172, 68, LEFT, LB, LM_f, 78, 79, 6_000_000, 2_500_000, 50_000_000, 3, 12);
        AddSimple(OSA, "Juan", "Cruz", "Juan Cruz", 32, "ESP", 182, 74, RIGHT, RB, 0, 76, 76, 3_000_000, 2_000_000, 30_000_000, 2, 2);
        AddSimple(OSA, "Valentin", "Rosier", "Rosier", 28, "FRA", 176, 70, RIGHT, RB, RM_f, 76, 78, 5_000_000, 2_000_000, 50_000_000, 3, 15);
        AddSimple(OSA, "Iñigo", "Arguibide", "Arguibide", 22, "ESP", 180, 72, RIGHT, RB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 24);
        AddSimple(OSA, "Lucas", "Torró", "Torró", 30, "ESP", 190, 82, RIGHT, CDM, CM_f, 80, 81, 8_000_000, 2_500_000, 60_000_000, 3, 5);
        AddSimple(OSA, "Iker", "Muñoz", "I. Muñoz", 22, "ESP", 182, 74, RIGHT, CM, CDM_f, 76, 84, 12_000_000, 1_500_000, 100_000_000, 5, 16);
        AddSimple(OSA, "Jon", "Moncayola", "Moncayola", 26, "ESP", 183, 76, RIGHT, CM, CDM_f, 80, 83, 15_000_000, 3_000_000, 100_000_000, 4, 7);
        AddSimple(OSA, "Asier", "Osambela", "Osambela", 21, "ESP", 180, 72, RIGHT, CM, CAM_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 23);
        AddSimple(OSA, "Aimar", "Oroz", "Oroz", 23, "ESP", 175, 68, RIGHT, CAM, CM_f, 80, 85, 20_000_000, 2_500_000, 150_000_000, 5, 10);
        AddSimple(OSA, "Moi", "Gómez", "Moi Gómez", 30, "ESP", 176, 70, RIGHT, CAM, LW_f, 78, 79, 6_000_000, 2_500_000, 50_000_000, 2, 11);
        AddSimple(OSA, "Víctor", "Muñoz", "V. Muñoz", 21, "ESP", 176, 68, RIGHT, LW, RW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 19);
        AddSimple(OSA, "Raúl", "Moro", "Raúl Moro", 22, "ESP", 170, 64, RIGHT, LW, RW_f, 78, 85, 15_000_000, 2_000_000, 120_000_000, 5, 14);
        AddSimple(OSA, "Rubén", "García", "R. García", 31, "ESP", 176, 70, RIGHT, RW, LW_f, 78, 79, 5_000_000, 2_500_000, 40_000_000, 2, 17);
        AddSimple(OSA, "Kike", "Barja", "Barja", 27, "ESP", 178, 72, RIGHT, RW, 0, 76, 78, 5_000_000, 1_500_000, 40_000_000, 3, 20);
        AddSimple(OSA, "Iker", "Benito", "Benito", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 21);
        AddSimple(OSA, "Raúl", "García", "Raúl García", 38, "ESP", 184, 82, RIGHT, ST, CAM_f, 78, 78, 2_000_000, 3_000_000, 20_000_000, 1, 9);
        AddSimple(OSA, "Ante", "Budimir", "Budimir", 33, "HRV", 190, 84, RIGHT, ST, 0, 82, 82, 6_000_000, 4_000_000, 50_000_000, 2, 17);

        // ══════════════════════════════════════════════════════════
        // RC CELTA DE VIGO (TeamId=11)
        // ══════════════════════════════════════════════════════════
        int CEL = 11;
        AddSimple(CEL, "Ionuț", "Radu", "Radu", 27, "ROU", 188, 82, RIGHT, GK, 0, 74, 78, 3_000_000, 1_200_000, 30_000_000, 3, 13);
        AddSimple(CEL, "Iván", "Villar", "I. Villar", 27, "ESP", 186, 78, RIGHT, GK, 0, 74, 78, 3_000_000, 1_200_000, 30_000_000, 3, 1);
        AddSimple(CEL, "Javi", "Rodríguez", "J. Rodríguez", 22, "ESP", 184, 76, RIGHT, CB, 0, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 2);
        AddSimple(CEL, "Carl", "Starfelt", "Starfelt", 29, "SWE", 187, 80, RIGHT, CB, 0, 78, 80, 6_000_000, 2_500_000, 50_000_000, 3, 4);
        AddSimple(CEL, "Yoel", "Lago", "Lago", 21, "ESP", 182, 74, RIGHT, CB, 0, 70, 80, 3_000_000, 500_000, 40_000_000, 5, 3);
        AddSimple(CEL, "Carlos", "Domínguez", "C. Domínguez", 23, "ESP", 186, 78, RIGHT, CB, 0, 74, 82, 8_000_000, 1_200_000, 80_000_000, 5, 5);
        AddSimple(CEL, "Manu", "Fernández", "M. Fernández", 22, "ESP", 178, 72, LEFT, LB, LM_f, 72, 80, 4_000_000, 800_000, 50_000_000, 5, 18);
        AddSimple(CEL, "Marcos", "Alonso", "Marcos Alonso", 33, "ESP", 188, 80, LEFT, LB, CB_f, 78, 78, 3_000_000, 3_000_000, 30_000_000, 2, 20);
        AddSimple(CEL, "Joseph", "Aidoo", "Aidoo", 29, "GHA", 184, 78, RIGHT, CB, 0, 77, 78, 5_000_000, 2_000_000, 40_000_000, 2, 15);
        AddSimple(CEL, "Mihailo", "Ristić", "Ristić", 29, "SRB", 183, 76, LEFT, LB, LM_f, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 17);
        AddSimple(CEL, "Óscar", "Mingueza", "Mingueza", 26, "ESP", 184, 76, RIGHT, RB, CB_f, 76, 80, 6_000_000, 2_000_000, 60_000_000, 4, 3);
        AddSimple(CEL, "Sergio", "Carreira", "Carreira", 23, "ESP", 174, 68, RIGHT, RB, RM_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 22);
        AddSimple(CEL, "Javi", "Rueda", "Rueda", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 72, 80, 4_000_000, 800_000, 50_000_000, 5, 6);
        AddSimple(CEL, "Álvaro", "Núñez", "Á. Núñez", 24, "ESP", 182, 74, RIGHT, CM, CDM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 8);
        AddSimple(CEL, "Ilaix", "Moriba", "Moriba", 21, "GIN", 185, 76, RIGHT, CM, CAM_f, 74, 84, 10_000_000, 1_500_000, 100_000_000, 5, 12);
        AddSimple(CEL, "Hugo", "Sotelo", "Sotelo", 21, "ESP", 180, 72, RIGHT, CM, CDM_f, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 16);
        AddSimple(CEL, "Matías", "Vecino", "Vecino", 33, "URY", 187, 80, RIGHT, CM, CDM_f, 78, 78, 3_000_000, 3_000_000, 30_000_000, 2, 14);
        AddSimple(CEL, "Fer", "López", "Fer López", 20, "ESP", 174, 66, RIGHT, CAM, LW_f, 72, 84, 8_000_000, 800_000, 80_000_000, 5, 23);
        AddSimple(CEL, "Williot", "Swedberg", "Swedberg", 20, "SWE", 186, 76, RIGHT, CAM, CM_f, 74, 84, 10_000_000, 1_200_000, 100_000_000, 5, 19);
        AddSimple(CEL, "Hugo", "Álvarez", "H. Álvarez", 21, "ESP", 176, 68, RIGHT, LW, RW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 21);
        AddSimple(CEL, "Franco", "Cervi", "Cervi", 30, "ARG", 166, 62, LEFT, LW, RW_f, 76, 77, 4_000_000, 2_000_000, 40_000_000, 2, 11);
        AddSimple(CEL, "Ferran", "Jutglà", "Jutglà", 25, "ESP", 178, 72, RIGHT, ST, LW_f, 76, 80, 6_000_000, 1_500_000, 60_000_000, 4, 9);
        AddSimple(CEL, "Jones", "El-Abdellaoui", "El-Abdellaoui", 22, "NOR", 182, 74, RIGHT, RW, LW_f, 72, 80, 4_000_000, 800_000, 50_000_000, 5, 24);
        AddSimple(CEL, "Pablo", "Durán", "P. Durán", 23, "ESP", 178, 72, RIGHT, ST, LW_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 4, 7);
        AddSimple(CEL, "Borja", "Iglesias", "Borja Iglesias", 31, "ESP", 186, 82, RIGHT, ST, 0, 78, 78, 5_000_000, 3_000_000, 50_000_000, 2, 10);
        AddSimple(CEL, "Iago", "Aspas", "Aspas", 37, "ESP", 176, 68, LEFT, ST, CAM_f, 82, 82, 3_000_000, 5_000_000, 30_000_000, 1, 17);

        // ══════════════════════════════════════════════════════════
        // RCD ESPANYOL (TeamId=12)
        // ══════════════════════════════════════════════════════════
        int ESP = 12;
        AddSimple(ESP, "Marko", "Dmitrović", "Dmitrović", 32, "SRB", 194, 86, RIGHT, GK, 0, 77, 78, 4_000_000, 2_500_000, 40_000_000, 2, 1);
        AddSimple(ESP, "Ángel", "Fortuño", "Fortuño", 23, "ESP", 186, 78, RIGHT, GK, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 13);
        AddSimple(ESP, "Pol", "Tristán", "Tristán", 22, "ESP", 184, 76, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 2);
        AddSimple(ESP, "Clemens", "Riedel", "Riedel", 21, "DEU", 188, 80, RIGHT, CB, 0, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 3);
        AddSimple(ESP, "Fernando", "Calero", "Calero", 29, "ESP", 184, 76, RIGHT, CB, 0, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 4);
        AddSimple(ESP, "Miguel", "Rubio", "M. Rubio", 26, "ESP", 186, 78, RIGHT, CB, 0, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 5);
        AddSimple(ESP, "Leandro", "Cabrera", "Cabrera", 33, "URY", 188, 82, LEFT, CB, 0, 78, 78, 3_000_000, 2_500_000, 30_000_000, 2, 6);
        AddSimple(ESP, "Carlos", "Romero", "C. Romero", 23, "ESP", 178, 72, LEFT, LB, LM_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 14);
        AddSimple(ESP, "José", "Salinas", "Salinas", 22, "ESP", 180, 72, LEFT, LB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 21);
        AddSimple(ESP, "Omar", "El Hilali", "El Hilali", 21, "MAR", 176, 68, RIGHT, RB, RM_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 19);
        AddSimple(ESP, "Rubén", "Sánchez", "R. Sánchez", 22, "ESP", 178, 72, RIGHT, RB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 22);
        AddSimple(ESP, "Urko", "González de Zárate", "Urko", 23, "ESP", 186, 80, RIGHT, CDM, CM_f, 76, 82, 8_000_000, 1_200_000, 80_000_000, 5, 8);
        AddSimple(ESP, "Charles", "Pickel", "Pickel", 27, "CHE", 184, 76, RIGHT, CDM, CM_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 3, 15);
        AddSimple(ESP, "Pol", "Lozano", "P. Lozano", 25, "ESP", 176, 70, RIGHT, CM, CAM_f, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 10);
        AddSimple(ESP, "Edu", "Expósito", "Expósito", 28, "ESP", 180, 72, RIGHT, CM, CAM_f, 78, 80, 8_000_000, 2_500_000, 60_000_000, 3, 11);
        AddSimple(ESP, "Ramón", "Terrats", "Terrats", 24, "ESP", 180, 74, RIGHT, CM, CDM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 16);
        AddSimple(ESP, "Javi", "Puado", "Puado", 26, "ESP", 177, 70, RIGHT, LW, RW_f, 78, 82, 12_000_000, 3_000_000, 100_000_000, 4, 7);
        AddSimple(ESP, "Pere", "Milla", "P. Milla", 32, "ESP", 179, 72, RIGHT, LW, RW_f, 76, 76, 3_000_000, 2_000_000, 30_000_000, 2, 18);
        AddSimple(ESP, "Tyrhys", "Dolan", "Dolan", 23, "GBR", 170, 64, RIGHT, RW, LW_f, 76, 82, 8_000_000, 1_200_000, 80_000_000, 5, 9);
        AddSimple(ESP, "Cyril", "Ngonge", "Ngonge", 24, "BEL", 182, 74, RIGHT, RW, LW_f, 76, 80, 6_000_000, 1_500_000, 60_000_000, 4, 20);
        AddSimple(ESP, "Jofre", "Carreras", "J. Carreras", 23, "ESP", 174, 66, RIGHT, RW, LW_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 17);
        AddSimple(ESP, "Antoniu", "Roca", "Roca", 22, "ESP", 178, 72, RIGHT, ST, CF_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 24);
        AddSimple(ESP, "Roberto", "Fernández", "R. Fernández", 22, "ESP", 184, 76, RIGHT, ST, 0, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 12);
        AddSimple(ESP, "Kike", "García", "Kike García", 35, "ESP", 186, 80, RIGHT, ST, 0, 76, 76, 2_000_000, 2_500_000, 20_000_000, 1, 23);

        // ══════════════════════════════════════════════════════════
        // RAYO VALLECANO (TeamId=13)
        // ══════════════════════════════════════════════════════════
        int RAY = 13;
        AddSimple(RAY, "Augusto", "Batalla", "Batalla", 28, "ARG", 186, 80, RIGHT, GK, 0, 76, 80, 5_000_000, 1_500_000, 40_000_000, 3, 1);
        AddSimple(RAY, "Dani", "Cárdenas", "Cárdenas", 27, "ESP", 186, 78, RIGHT, GK, 0, 74, 78, 3_000_000, 1_200_000, 30_000_000, 3, 13);
        AddSimple(RAY, "Nobel", "Mendy", "Mendy", 20, "SEN", 188, 80, RIGHT, CB, 0, 72, 84, 8_000_000, 800_000, 80_000_000, 5, 2);
        AddSimple(RAY, "Abdul", "Mumin", "Mumin", 26, "GHA", 188, 80, RIGHT, CB, 0, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 3);
        AddSimple(RAY, "Luiz", "Felipe", "Luiz Felipe", 27, "BRA", 186, 80, RIGHT, CB, 0, 78, 80, 8_000_000, 2_500_000, 60_000_000, 3, 4);
        AddSimple(RAY, "Jozhua", "Vertrouwd", "Vertrouwd", 22, "NLD", 184, 76, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 5);
        AddSimple(RAY, "Florian", "Lejeune", "Lejeune", 33, "FRA", 190, 84, RIGHT, CB, 0, 80, 80, 5_000_000, 3_000_000, 40_000_000, 2, 8);
        AddSimple(RAY, "Pep", "Chavarría", "Chavarría", 26, "ESP", 176, 68, LEFT, LB, LM_f, 76, 78, 5_000_000, 1_500_000, 40_000_000, 3, 6);
        AddSimple(RAY, "Alfonso", "Espino", "Espino", 32, "URY", 174, 68, LEFT, LB, 0, 76, 76, 3_000_000, 2_000_000, 30_000_000, 2, 12);
        AddSimple(RAY, "Andrei", "Rațiu", "Rațiu", 26, "ROU", 180, 72, RIGHT, RB, RM_f, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 2);
        AddSimple(RAY, "Iván", "Balliu", "Balliu", 32, "ALB", 174, 70, RIGHT, RB, 0, 76, 77, 3_000_000, 2_000_000, 30_000_000, 2, 15);
        AddSimple(RAY, "Gerard", "Gumbau", "Gumbau", 29, "ESP", 186, 76, LEFT, CM, CDM_f, 76, 78, 5_000_000, 2_000_000, 50_000_000, 3, 14);
        AddSimple(RAY, "Pedro", "Díaz", "P. Díaz", 26, "ESP", 178, 72, RIGHT, CDM, CM_f, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 16);
        AddSimple(RAY, "Unai", "López", "Unai López", 29, "ESP", 178, 70, RIGHT, CM, CAM_f, 78, 80, 6_000_000, 2_500_000, 50_000_000, 3, 11);
        AddSimple(RAY, "Óscar", "Valentín", "Valentín", 30, "ESP", 182, 74, RIGHT, CDM, CM_f, 78, 79, 5_000_000, 2_500_000, 40_000_000, 3, 18);
        AddSimple(RAY, "Pathé", "Ciss", "Ciss", 30, "SEN", 186, 76, RIGHT, CDM, CM_f, 76, 78, 4_000_000, 2_000_000, 40_000_000, 3, 21);
        AddSimple(RAY, "Samu", "Becerra", "Becerra", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 19);
        AddSimple(RAY, "Randy", "Nteka", "Nteka", 27, "FRA", 182, 76, RIGHT, CAM, ST_f, 76, 78, 5_000_000, 1_500_000, 40_000_000, 3, 10);
        AddSimple(RAY, "Óscar", "Trejo", "Trejo", 36, "ARG", 178, 74, RIGHT, CAM, CM_f, 78, 78, 2_000_000, 2_500_000, 20_000_000, 1, 8);
        AddSimple(RAY, "Carlos", "Martín", "C. Martín", 22, "ESP", 180, 74, RIGHT, ST, LW_f, 76, 84, 10_000_000, 1_500_000, 100_000_000, 5, 9);
        AddSimple(RAY, "Álvaro", "García", "Álvaro García", 31, "ESP", 168, 64, RIGHT, LW, RW_f, 78, 78, 5_000_000, 2_500_000, 40_000_000, 2, 17);
        AddSimple(RAY, "Ilias", "Akhomach", "Akhomach", 20, "MAR", 175, 66, LEFT, LW, RW_f, 76, 85, 15_000_000, 1_500_000, 120_000_000, 5, 7);
        AddSimple(RAY, "Jorge", "de Frutos", "De Frutos", 27, "ESP", 180, 72, RIGHT, RW, LW_f, 78, 80, 8_000_000, 2_500_000, 60_000_000, 4, 20);
        AddSimple(RAY, "Fran", "Pérez", "Fran Pérez", 23, "ESP", 178, 72, RIGHT, RW, LW_f, 76, 82, 8_000_000, 1_500_000, 80_000_000, 5, 22);
        AddSimple(RAY, "Isi", "Palazón", "Isi", 29, "ESP", 168, 62, RIGHT, RW, LW_f, 80, 81, 10_000_000, 3_000_000, 80_000_000, 3, 23);
        AddSimple(RAY, "Alemão", "", "Alemão", 26, "BRA", 188, 80, RIGHT, ST, 0, 76, 78, 5_000_000, 1_500_000, 40_000_000, 3, 24);
        AddSimple(RAY, "Sergio", "Camello", "Camello", 23, "ESP", 182, 76, RIGHT, ST, CF_f, 76, 82, 8_000_000, 1_500_000, 80_000_000, 5, 25);

        // ══════════════════════════════════════════════════════════
        // LEVANTE UD (TeamId=14)
        // ══════════════════════════════════════════════════════════
        int LEV = 14;
        AddSimple(LEV, "Mathew", "Ryan", "Ryan", 32, "AUS", 184, 78, RIGHT, GK, 0, 78, 79, 4_000_000, 3_000_000, 40_000_000, 2, 1);
        AddSimple(LEV, "Pablo", "Campos", "P. Campos", 22, "ESP", 186, 78, RIGHT, GK, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 13);
        AddSimple(LEV, "Matías", "Moreno", "M. Moreno", 21, "ARG", 184, 76, RIGHT, CB, 0, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 2);
        AddSimple(LEV, "Alan", "Matturro", "Matturro", 20, "URY", 186, 78, LEFT, CB, 0, 72, 84, 8_000_000, 800_000, 80_000_000, 5, 3);
        AddSimple(LEV, "Adrián", "Dela", "Dela", 25, "ESP", 184, 76, RIGHT, CB, 0, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 4);
        AddSimple(LEV, "Unai", "Elgezabal", "Elgezabal", 31, "ESP", 186, 78, RIGHT, CB, 0, 74, 75, 2_000_000, 1_200_000, 20_000_000, 2, 5);
        AddSimple(LEV, "Manu", "Sánchez", "Manu Sánchez", 24, "ESP", 180, 72, LEFT, LB, LM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 19);
        AddSimple(LEV, "Diego", "Pampín", "Pampín", 24, "ESP", 176, 68, LEFT, LB, LM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 15);
        AddSimple(LEV, "Jeremy", "Toljan", "Toljan", 30, "DEU", 182, 74, RIGHT, RB, 0, 76, 77, 3_000_000, 2_000_000, 30_000_000, 2, 6);
        AddSimple(LEV, "Víctor", "García", "V. García", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 7);
        AddSimple(LEV, "Nacho", "Pérez", "Nacho Pérez", 22, "ESP", 174, 66, RIGHT, RW, LW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 21);
        AddSimple(LEV, "Ugo", "Raghouber", "Raghouber", 21, "FRA", 182, 74, RIGHT, CDM, CM_f, 72, 82, 5_000_000, 800_000, 50_000_000, 5, 8);
        AddSimple(LEV, "Kervin", "Arriaga", "Arriaga", 24, "HND", 182, 74, RIGHT, CDM, CM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 14);
        AddSimple(LEV, "Oriol", "Rey", "Oriol Rey", 26, "ESP", 178, 70, RIGHT, CM, CDM_f, 76, 80, 5_000_000, 1_500_000, 50_000_000, 4, 10);
        AddSimple(LEV, "Pablo", "Martínez", "P. Martínez", 26, "ESP", 182, 74, RIGHT, CM, CAM_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 3, 17);
        AddSimple(LEV, "Jon Ander", "Olasagasti", "Olasagasti", 24, "ESP", 180, 72, RIGHT, CM, CDM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 22);
        AddSimple(LEV, "Unai", "Vencedor", "Vencedor", 24, "ESP", 176, 68, RIGHT, CM, CDM_f, 76, 82, 8_000_000, 1_500_000, 80_000_000, 5, 16);
        AddSimple(LEV, "Carlos", "Álvarez", "C. Álvarez", 21, "ESP", 178, 72, RIGHT, CAM, LW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 18);
        AddSimple(LEV, "Roger", "Brugué", "Brugué", 27, "ESP", 172, 66, RIGHT, LW, RW_f, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 11);
        AddSimple(LEV, "Paco", "Cortés", "Cortés", 25, "ESP", 178, 72, RIGHT, LW, RW_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 20);
        AddSimple(LEV, "Kareem", "Tunde", "Tunde", 23, "NGA", 182, 74, RIGHT, ST, 0, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 24);
        AddSimple(LEV, "Iker", "Losada", "Losada", 23, "ESP", 174, 68, RIGHT, RW, LW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 12);
        AddSimple(LEV, "Carlos", "Espí", "Espí", 20, "ESP", 182, 74, RIGHT, ST, CF_f, 72, 82, 5_000_000, 800_000, 50_000_000, 5, 23);
        AddSimple(LEV, "Karl", "Etta Eyong", "Etta Eyong", 22, "CMR", 186, 78, RIGHT, ST, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 25);
        AddSimple(LEV, "Iván", "Romero", "I. Romero", 23, "ESP", 180, 74, RIGHT, ST, LW_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 4, 9);
        AddSimple(LEV, "José Luis", "Morales", "Morales", 37, "ESP", 180, 72, RIGHT, ST, LW_f, 78, 78, 2_000_000, 3_000_000, 20_000_000, 1, 11);

        // ══════════════════════════════════════════════════════════
        // GETAFE CF (TeamId=15)
        // ══════════════════════════════════════════════════════════
        int GET = 15;
        AddSimple(GET, "David", "Soria", "D. Soria", 31, "ESP", 192, 84, RIGHT, GK, 0, 80, 81, 8_000_000, 3_000_000, 60_000_000, 3, 1);
        AddSimple(GET, "Jiri", "Letacek", "Letacek", 25, "CZE", 190, 82, RIGHT, GK, 0, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 13);
        AddSimple(GET, "Abdel", "Abqar", "Abqar", 25, "MAR", 188, 80, RIGHT, CB, 0, 76, 82, 8_000_000, 1_500_000, 60_000_000, 4, 2);
        AddSimple(GET, "Sebastián", "Boselli", "Boselli", 21, "URY", 186, 78, RIGHT, CB, 0, 70, 80, 4_000_000, 800_000, 50_000_000, 5, 3);
        AddSimple(GET, "Dakonam", "Djené", "Djené", 32, "TGO", 184, 78, RIGHT, CB, RB_f, 78, 78, 3_000_000, 2_500_000, 30_000_000, 2, 4);
        AddSimple(GET, "Domingos", "Duarte", "Duarte", 29, "PRT", 192, 82, RIGHT, CB, 0, 75, 76, 4_000_000, 2_000_000, 40_000_000, 2, 6);
        AddSimple(GET, "Diego", "Rico", "Diego Rico", 31, "ESP", 182, 74, LEFT, LB, LM_f, 76, 77, 3_000_000, 2_000_000, 30_000_000, 2, 16);
        AddSimple(GET, "Davinchi", "", "Davinchi", 21, "COL", 178, 72, LEFT, LB, LM_f, 70, 80, 3_000_000, 600_000, 40_000_000, 5, 22);
        AddSimple(GET, "Juan", "Iglesias", "Juan Iglesias", 26, "ESP", 182, 74, RIGHT, RB, 0, 76, 78, 5_000_000, 1_500_000, 40_000_000, 3, 21);
        AddSimple(GET, "Kiko", "Femenía", "Femenía", 33, "ESP", 174, 70, RIGHT, RB, RM_f, 76, 76, 2_000_000, 2_000_000, 20_000_000, 2, 12);
        AddSimple(GET, "Ismael", "Bekhoucha", "Bekhoucha", 22, "MAR", 182, 74, RIGHT, RB, 0, 68, 78, 2_000_000, 500_000, 30_000_000, 5, 26);
        AddSimple(GET, "Allan", "Nyom", "Nyom", 36, "CMR", 188, 82, RIGHT, RB, LB_f, 74, 74, 1_000_000, 1_500_000, 15_000_000, 1, 5);
        AddSimple(GET, "Mario", "Martín", "Mario Martín", 20, "ESP", 180, 72, RIGHT, CM, CDM_f, 72, 82, 6_000_000, 1_000_000, 60_000_000, 5, 8);
        AddSimple(GET, "Mauro", "Arambarri", "Arambarri", 29, "URY", 178, 74, RIGHT, CDM, CM_f, 80, 81, 12_000_000, 3_500_000, 80_000_000, 3, 18);
        AddSimple(GET, "Luis", "Milla", "Milla", 29, "ESP", 175, 70, RIGHT, CM, 0, 78, 79, 6_000_000, 2_500_000, 50_000_000, 3, 10);
        AddSimple(GET, "Veljko", "Birmancevic", "Birmancevic", 26, "SRB", 180, 72, RIGHT, CAM, LW_f, 76, 78, 5_000_000, 1_500_000, 40_000_000, 3, 23);
        AddSimple(GET, "Adrián", "Liso", "Liso", 21, "ESP", 176, 68, RIGHT, LW, RW_f, 70, 80, 3_000_000, 600_000, 40_000_000, 5, 25);
        AddSimple(GET, "Juanmi", "", "Juanmi", 31, "ESP", 169, 66, RIGHT, LW, ST_f, 77, 77, 4_000_000, 2_500_000, 30_000_000, 2, 7);
        AddSimple(GET, "Abu", "Kamara", "Kamara", 22, "SLE", 182, 74, RIGHT, LW, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 24);
        AddSimple(GET, "Álex", "Sancris", "Sancris", 23, "ESP", 172, 66, RIGHT, RW, 0, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 20);
        AddSimple(GET, "Martín", "Satriano", "Satriano", 23, "URY", 187, 78, RIGHT, ST, 0, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 9);
        AddSimple(GET, "Borja", "Mayoral", "Mayoral", 27, "ESP", 182, 74, RIGHT, ST, 0, 80, 82, 12_000_000, 4_000_000, 100_000_000, 3, 19);
        AddSimple(GET, "Luis", "Vázquez", "L. Vázquez", 27, "ESP", 188, 80, RIGHT, ST, 0, 74, 76, 3_000_000, 1_500_000, 30_000_000, 2, 14);

        // ══════════════════════════════════════════════════════════
        // DEPORTIVO ALAVÉS (TeamId=16)
        // ══════════════════════════════════════════════════════════
        int ALA = 16;
        AddSimple(ALA, "Antonio", "Sivera", "Sivera", 28, "ESP", 184, 78, RIGHT, GK, 0, 75, 78, 3_000_000, 1_500_000, 40_000_000, 3, 1);
        AddSimple(ALA, "Raúl", "Fernández", "R. Fernández", 36, "ESP", 188, 82, RIGHT, GK, 0, 73, 73, 500_000, 1_000_000, 15_000_000, 1, 31);
        AddSimple(ALA, "Jon", "Pacheco", "Pacheco", 23, "ESP", 186, 78, LEFT, CB, 0, 74, 82, 8_000_000, 1_200_000, 80_000_000, 5, 4);
        AddSimple(ALA, "Ville", "Koski", "Koski", 22, "FIN", 179, 72, RIGHT, CB, 0, 72, 80, 5_000_000, 800_000, 60_000_000, 5, 5);
        AddSimple(ALA, "Facundo", "Garcés", "Garcés", 26, "ARG", 186, 80, RIGHT, CB, 0, 76, 80, 4_000_000, 1_500_000, 50_000_000, 3, 22);
        AddSimple(ALA, "Youssef", "Enríquez", "Enríquez", 23, "ESP", 182, 74, RIGHT, CB, CDM_f, 72, 80, 4_000_000, 800_000, 50_000_000, 5, 3);
        AddSimple(ALA, "Nahuel", "Tenaglia", "Tenaglia", 28, "ARG", 183, 76, RIGHT, RB, RM_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 3, 2);
        AddSimple(ALA, "Jonny", "Otto", "Jonny", 30, "ESP", 175, 70, RIGHT, LB, LM_f, 77, 78, 4_000_000, 2_000_000, 40_000_000, 2, 12);
        AddSimple(ALA, "Antonio", "Blanco", "Blanco", 26, "ESP", 186, 76, RIGHT, CDM, CM_f, 76, 80, 6_000_000, 1_500_000, 60_000_000, 4, 8);
        AddSimple(ALA, "Carlos", "Protesoni", "Protesoni", 25, "URY", 178, 72, RIGHT, CM, CAM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 6);
        AddSimple(ALA, "Pablo", "Ibáñez", "P. Ibáñez", 26, "ESP", 182, 74, RIGHT, CM, CDM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 3, 14);
        AddSimple(ALA, "Carles", "Aleñá", "Aleñá", 26, "ESP", 180, 72, RIGHT, CM, CAM_f, 78, 81, 8_000_000, 2_500_000, 60_000_000, 3, 10);
        AddSimple(ALA, "Ander", "Guevara", "Guevara", 27, "ESP", 180, 72, RIGHT, CM, CDM_f, 76, 78, 5_000_000, 1_800_000, 50_000_000, 3, 18);
        AddSimple(ALA, "Jon", "Guridi", "Guridi", 29, "ESP", 178, 72, RIGHT, CM, CAM_f, 76, 77, 4_000_000, 1_500_000, 40_000_000, 2, 15);
        AddSimple(ALA, "Calebe", "", "Calebe", 24, "BRA", 178, 70, RIGHT, CAM, LW_f, 74, 80, 6_000_000, 1_200_000, 50_000_000, 4, 21);
        AddSimple(ALA, "Denis", "Suárez", "Denis Suárez", 30, "ESP", 176, 70, RIGHT, CAM, CM_f, 78, 79, 5_000_000, 2_500_000, 40_000_000, 2, 11);
        AddSimple(ALA, "Abde", "Rebbach", "Rebbach", 25, "MAR", 178, 70, RIGHT, LW, RW_f, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 7);
        AddSimple(ALA, "Ángel", "Pérez", "Ángel Pérez", 21, "ESP", 174, 66, RIGHT, LW, RW_f, 72, 80, 4_000_000, 600_000, 40_000_000, 5, 19);
        AddSimple(ALA, "Toni", "Martínez", "Toni Martínez", 27, "ESP", 182, 76, RIGHT, ST, CF_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 3, 9);
        AddSimple(ALA, "Lucas", "Boyé", "Boyé", 28, "ARG", 186, 80, RIGHT, ST, LW_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 3, 17);
        AddSimple(ALA, "Ibrahim", "Diabaté", "Diabaté", 25, "CIV", 192, 84, RIGHT, ST, 0, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 20);
        AddSimple(ALA, "Aitor", "Mañas", "Mañas", 22, "ESP", 178, 72, RIGHT, ST, CF_f, 70, 80, 3_000_000, 500_000, 30_000_000, 5, 28);
        AddSimple(ALA, "Mariano", "Díaz", "Mariano", 31, "DOM", 178, 74, RIGHT, ST, 0, 76, 76, 3_000_000, 2_500_000, 30_000_000, 2, 24);

        // ══════════════════════════════════════════════════════════
        // ELCHE CF (TeamId=17)
        // ══════════════════════════════════════════════════════════
        int ELC = 17;
        AddSimple(ELC, "Iñaki", "Peña", "I. Peña", 25, "ESP", 184, 78, RIGHT, GK, 0, 74, 82, 5_000_000, 1_500_000, 60_000_000, 4, 1);
        AddSimple(ELC, "Matías", "Dituro", "Dituro", 37, "ARG", 186, 82, RIGHT, GK, 0, 73, 73, 500_000, 1_000_000, 10_000_000, 1, 13);
        AddSimple(ELC, "David", "Affengruber", "Affengruber", 23, "AUT", 186, 78, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 2);
        AddSimple(ELC, "Víctor", "Chust", "Chust", 24, "ESP", 184, 76, RIGHT, CB, 0, 74, 80, 5_000_000, 1_200_000, 50_000_000, 5, 4);
        AddSimple(ELC, "John", "Donald", "Donald", 22, "SCO", 186, 78, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 5);
        AddSimple(ELC, "Pedro", "Bigas", "Bigas", 34, "ESP", 182, 76, RIGHT, CB, 0, 75, 75, 1_000_000, 1_500_000, 15_000_000, 1, 6);
        AddSimple(ELC, "Adrià", "Pedrosa", "Pedrosa", 26, "ESP", 182, 74, LEFT, LB, LM_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 4, 3);
        AddSimple(ELC, "Léo", "Pétrot", "Pétrot", 27, "FRA", 180, 72, LEFT, LB, 0, 74, 78, 3_000_000, 1_200_000, 30_000_000, 3, 18);
        AddSimple(ELC, "Héctor", "Fort", "Fort", 18, "ESP", 180, 72, RIGHT, RB, CB_f, 72, 86, 12_000_000, 1_000_000, 100_000_000, 5, 22);
        AddSimple(ELC, "Buba", "Sangaré", "Sangaré", 22, "GIN", 180, 72, RIGHT, RB, RM_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 21);
        AddSimple(ELC, "Marc", "Aguado", "Aguado", 24, "ESP", 180, 72, RIGHT, CM, CDM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 8);
        AddSimple(ELC, "Federico", "Redondo", "F. Redondo", 21, "ARG", 184, 76, RIGHT, CDM, CM_f, 74, 84, 10_000_000, 1_200_000, 80_000_000, 5, 15);
        AddSimple(ELC, "Aleix", "Febas", "Febas", 28, "ESP", 176, 70, RIGHT, CM, 0, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 10);
        AddSimple(ELC, "Martim", "Neto", "Neto", 21, "PRT", 178, 72, RIGHT, CM, CAM_f, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 20);
        AddSimple(ELC, "Gonzalo", "Villar", "G. Villar", 26, "ESP", 180, 72, RIGHT, CAM, CM_f, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 14);
        AddSimple(ELC, "Germán", "Valera", "Valera", 22, "ESP", 174, 66, RIGHT, LW, RW_f, 74, 82, 8_000_000, 1_200_000, 80_000_000, 5, 7);
        AddSimple(ELC, "Lucas", "Cepeda", "Cepeda", 22, "CHL", 182, 74, RIGHT, LW, RW_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 11);
        AddSimple(ELC, "Tete", "Morente", "Morente", 27, "ESP", 174, 68, RIGHT, RW, LW_f, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 19);
        AddSimple(ELC, "Yago", "Santiago", "Yago", 21, "ESP", 178, 72, RIGHT, LW, RW_f, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 17);
        AddSimple(ELC, "Grady", "Diangana", "Diangana", 26, "COD", 180, 72, RIGHT, RW, LW_f, 76, 78, 5_000_000, 1_500_000, 50_000_000, 3, 9);
        AddSimple(ELC, "Josan", "", "Josan", 33, "ESP", 175, 70, RIGHT, RW, 0, 74, 74, 1_000_000, 1_500_000, 15_000_000, 1, 16);
        AddSimple(ELC, "Adam", "Boayar", "Boayar", 24, "SEN", 188, 80, RIGHT, ST, 0, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 12);
        AddSimple(ELC, "Álvaro", "Rodríguez", "Á. Rodríguez", 20, "URY", 188, 80, RIGHT, ST, CF_f, 74, 84, 10_000_000, 1_200_000, 100_000_000, 5, 23);
        AddSimple(ELC, "Rafa", "Mir", "Rafa Mir", 27, "ESP", 191, 82, RIGHT, ST, 0, 76, 78, 5_000_000, 2_000_000, 50_000_000, 3, 24);
        AddSimple(ELC, "André", "Silva", "André Silva", 29, "PRT", 184, 76, RIGHT, ST, 0, 78, 79, 8_000_000, 3_000_000, 60_000_000, 3, 26);

        // ══════════════════════════════════════════════════════════
        // MÁLAGA CF (TeamId=18)
        // ══════════════════════════════════════════════════════════
        int MAL = 18;
        AddSimple(MAL, "Alfonso", "Herrero", "Herrero", 30, "ESP", 184, 78, RIGHT, GK, 0, 74, 76, 2_000_000, 1_200_000, 20_000_000, 2, 1);
        AddSimple(MAL, "Carlos", "López", "C. López", 22, "ESP", 186, 78, RIGHT, GK, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 13);
        AddSimple(MAL, "Diego", "Murillo", "Murillo", 22, "ESP", 184, 76, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 2);
        AddSimple(MAL, "Javi", "Montero", "J. Montero", 25, "ESP", 186, 78, LEFT, CB, 0, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 3);
        AddSimple(MAL, "Einar", "Galilea", "Galilea", 30, "ESP", 184, 76, RIGHT, CB, 0, 74, 75, 2_000_000, 1_200_000, 20_000_000, 2, 5);
        AddSimple(MAL, "Álex", "Pastor", "Pastor", 22, "ESP", 182, 74, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 4);
        AddSimple(MAL, "Ángel", "Recio", "Recio", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 17);
        AddSimple(MAL, "Dani", "Sánchez", "D. Sánchez", 23, "ESP", 180, 72, LEFT, LB, LM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 15);
        AddSimple(MAL, "Víctor", "García", "Víctor García", 26, "ESP", 176, 70, RIGHT, RB, 0, 74, 78, 4_000_000, 1_200_000, 40_000_000, 3, 18);
        AddSimple(MAL, "Rafita", "Garrido", "Rafita", 23, "ESP", 176, 68, RIGHT, RB, RM_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 20);
        AddSimple(MAL, "Carlos", "Puga", "Puga", 22, "ESP", 178, 72, RIGHT, RB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 21);
        AddSimple(MAL, "Jokin", "Gabilondo", "Gabilondo", 25, "ESP", 174, 68, LEFT, LB, LM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 22);
        AddSimple(MAL, "Luismi", "Sánchez", "Luismi", 32, "ESP", 184, 76, RIGHT, CDM, CM_f, 76, 76, 2_000_000, 1_500_000, 20_000_000, 2, 6);
        AddSimple(MAL, "Izan", "Merino", "Merino", 21, "ESP", 176, 70, RIGHT, CM, CDM_f, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 8);
        AddSimple(MAL, "Carlos", "Dotor", "Dotor", 23, "ESP", 182, 74, RIGHT, CM, CAM_f, 74, 82, 6_000_000, 1_200_000, 60_000_000, 5, 10);
        AddSimple(MAL, "Juanpe", "", "Juanpe", 33, "ESP", 182, 76, RIGHT, CM, CDM_f, 74, 74, 1_000_000, 1_500_000, 15_000_000, 1, 14);
        AddSimple(MAL, "Ramón", "Enríquez", "Enríquez", 23, "ESP", 180, 72, RIGHT, CM, CDM_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 16);
        AddSimple(MAL, "Darko", "Brasanac", "Brasanac", 32, "SRB", 186, 80, RIGHT, CM, 0, 74, 74, 1_000_000, 1_500_000, 15_000_000, 2, 7);
        AddSimple(MAL, "Dani", "Lorenzo", "Lorenzo", 21, "ESP", 180, 72, RIGHT, CM, CAM_f, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 23);
        AddSimple(MAL, "Rafa", "Rodríguez", "Rafa Rodríguez", 22, "ESP", 178, 72, RIGHT, CAM, LW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 9);
        AddSimple(MAL, "Aarón", "Ochoa", "Ochoa", 22, "ESP", 174, 66, RIGHT, LW, RW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 11);
        AddSimple(MAL, "Joaquín", "Muñoz", "J. Muñoz", 25, "ESP", 176, 68, RIGHT, LW, RW_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 12);
        AddSimple(MAL, "Julen", "Lobete", "Lobete", 24, "ESP", 178, 72, RIGHT, RW, LW_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 19);
        AddSimple(MAL, "David", "Larrubia", "Larrubia", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 24);
        AddSimple(MAL, "Josué", "Dorrio", "Dorrio", 30, "ESP", 174, 68, RIGHT, RW, 0, 74, 75, 2_000_000, 1_200_000, 20_000_000, 2, 14);
        AddSimple(MAL, "Haitam", "Abaida", "Abaida", 22, "ESP", 180, 72, RIGHT, LW, RW_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 25);
        AddSimple(MAL, "Chupe", "", "Chupe", 25, "ESP", 178, 72, RIGHT, ST, CF_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 28);
        AddSimple(MAL, "Adrián", "Niño", "Adrián Niño", 21, "ESP", 184, 76, RIGHT, ST, 0, 72, 82, 5_000_000, 800_000, 50_000_000, 5, 17);
        AddSimple(MAL, "Eneko", "Jauregi", "Jauregi", 22, "ESP", 182, 74, RIGHT, ST, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 29);

        // ══════════════════════════════════════════════════════════
        // DEPORTIVO DE LA CORUÑA (TeamId=19)
        // ══════════════════════════════════════════════════════════
        int DEP = 19;
        AddSimple(DEP, "Germán", "Parreño", "Parreño", 31, "ESP", 186, 80, RIGHT, GK, 0, 74, 75, 2_000_000, 1_200_000, 20_000_000, 2, 1);
        AddSimple(DEP, "Eric", "Puerto", "Puerto", 24, "ESP", 184, 76, RIGHT, GK, 0, 70, 78, 2_000_000, 500_000, 25_000_000, 4, 13);
        AddSimple(DEP, "Lucas", "Noubi", "Noubi", 23, "FRA", 186, 78, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 2);
        AddSimple(DEP, "Dani", "Barcia", "Barcia", 21, "ESP", 184, 76, LEFT, CB, 0, 70, 82, 5_000_000, 600_000, 50_000_000, 5, 3);
        AddSimple(DEP, "Arnau", "Comas", "Comas", 24, "ESP", 188, 80, RIGHT, CB, 0, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 5);
        AddSimple(DEP, "Giacomo", "Quagliata", "Quagliata", 24, "ITA", 182, 74, LEFT, LB, LM_f, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 15);
        AddSimple(DEP, "Adrià", "Altimira", "Altimira", 23, "ESP", 180, 72, RIGHT, RB, RM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 6);
        AddSimple(DEP, "Miguel", "Loureiro", "Loureiro", 27, "ESP", 178, 72, RIGHT, RB, RM_f, 74, 76, 3_000_000, 1_200_000, 30_000_000, 3, 4);
        AddSimple(DEP, "Ximo", "Navarro", "Ximo Navarro", 34, "ESP", 182, 76, RIGHT, RB, CB_f, 74, 74, 1_000_000, 1_500_000, 15_000_000, 1, 22);
        AddSimple(DEP, "Riki", "Rodríguez", "Riki", 27, "ESP", 176, 70, RIGHT, CM, CDM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 8);
        AddSimple(DEP, "José", "Gragera", "Gragera", 24, "ESP", 184, 76, RIGHT, CDM, CM_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 5, 20);
        AddSimple(DEP, "Diego", "Villares", "Villares", 26, "ESP", 176, 70, RIGHT, CM, CDM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 3, 16);
        AddSimple(DEP, "Noé", "Carrillo", "Carrillo", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 17);
        AddSimple(DEP, "Charlie", "Patiño", "Patiño", 21, "GBR", 182, 74, RIGHT, CM, CAM_f, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 18);
        AddSimple(DEP, "José Ángel", "Jurado", "Jurado", 23, "ESP", 178, 72, RIGHT, CM, CDM_f, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 14);
        AddSimple(DEP, "Mario", "Soriano", "Soriano", 22, "ESP", 174, 68, RIGHT, CAM, LW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 10);
        AddSimple(DEP, "Yeremay", "Hernández", "Yeremay", 21, "ESP", 172, 66, RIGHT, LW, RW_f, 74, 84, 8_000_000, 1_000_000, 80_000_000, 5, 11);
        AddSimple(DEP, "David", "Mella", "Mella", 19, "ESP", 176, 68, RIGHT, RW, LW_f, 72, 85, 10_000_000, 600_000, 100_000_000, 5, 7);
        AddSimple(DEP, "Luismi", "Cruz", "Luismi Cruz", 23, "ESP", 174, 66, RIGHT, RW, LW_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 12);
        AddSimple(DEP, "Stoichkov", "", "Stoichkov", 31, "ESP", 178, 72, RIGHT, LW, RW_f, 74, 75, 2_000_000, 1_200_000, 20_000_000, 2, 9);
        AddSimple(DEP, "Bil", "Nsongo", "Nsongo", 23, "BEL", 188, 80, RIGHT, ST, 0, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 21);
        AddSimple(DEP, "Zakaria", "Eddahchouri", "Eddahchouri", 24, "MAR", 182, 76, RIGHT, ST, CF_f, 72, 78, 3_000_000, 800_000, 30_000_000, 4, 19);
        AddSimple(DEP, "Samuele", "Mulattieri", "Mulattieri", 24, "ITA", 182, 74, RIGHT, ST, CF_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // RACING DE SANTANDER (TeamId=20)
        // ══════════════════════════════════════════════════════════
        int RAC = 20;
        AddSimple(RAC, "Jokin", "Ezkieta", "Ezkieta", 28, "ESP", 190, 82, RIGHT, GK, 0, 74, 78, 3_000_000, 1_200_000, 30_000_000, 3, 1);
        AddSimple(RAC, "Simon", "Eriksson", "Eriksson", 24, "SWE", 188, 80, RIGHT, GK, 0, 72, 80, 3_000_000, 800_000, 30_000_000, 4, 13);
        AddSimple(RAC, "Facundo", "González", "F. González", 21, "URY", 186, 78, LEFT, CB, 0, 72, 82, 6_000_000, 800_000, 60_000_000, 5, 2);
        AddSimple(RAC, "Javi", "Castro", "J. Castro", 22, "ESP", 184, 76, RIGHT, CB, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 3);
        AddSimple(RAC, "Manu", "Hernando", "Hernando", 26, "ESP", 186, 80, RIGHT, CB, 0, 74, 78, 4_000_000, 1_200_000, 40_000_000, 3, 4);
        AddSimple(RAC, "Pablo", "Ramón", "Ramón", 23, "ESP", 184, 76, RIGHT, CB, 0, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 5);
        AddSimple(RAC, "Jorge", "Salinas", "Salinas", 22, "ESP", 178, 72, LEFT, LB, LM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 15);
        AddSimple(RAC, "Mario", "García", "Mario García", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 6);
        AddSimple(RAC, "Álvaro", "Mantilla", "Mantilla", 24, "ESP", 182, 74, RIGHT, RB, CB_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 16);
        AddSimple(RAC, "Marco", "Sangalli", "Sangalli", 32, "ESP", 180, 74, RIGHT, RB, 0, 74, 74, 1_000_000, 1_500_000, 15_000_000, 2, 17);
        AddSimple(RAC, "Maguette", "Gueye", "Gueye", 25, "SEN", 186, 78, RIGHT, CDM, CM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 8);
        AddSimple(RAC, "Damián", "Rodríguez", "D. Rodríguez", 25, "URY", 180, 72, RIGHT, CM, CDM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 18);
        AddSimple(RAC, "Gustavo", "Puerta", "Puerta", 21, "COL", 182, 74, RIGHT, CM, CAM_f, 72, 82, 5_000_000, 800_000, 50_000_000, 5, 19);
        AddSimple(RAC, "Sergio", "Martínez", "S. Martínez", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 20);
        AddSimple(RAC, "Aritz", "Aldasoro", "Aldasoro", 25, "ESP", 178, 72, RIGHT, CM, CDM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 4, 21);
        AddSimple(RAC, "Íñigo", "Sainz-Maza", "Sainz-Maza", 26, "ESP", 180, 74, RIGHT, CM, CDM_f, 74, 78, 4_000_000, 1_200_000, 40_000_000, 3, 10);
        AddSimple(RAC, "Peio", "Canales", "Canales", 21, "ESP", 176, 68, RIGHT, CAM, LW_f, 74, 82, 6_000_000, 1_000_000, 60_000_000, 5, 11);
        AddSimple(RAC, "Íñigo", "Vicente", "Vicente", 26, "ESP", 178, 70, RIGHT, LW, RW_f, 78, 80, 6_000_000, 1_500_000, 50_000_000, 3, 7);
        AddSimple(RAC, "Andrés", "Martín", "A. Martín", 24, "ESP", 174, 66, RIGHT, LW, RW_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 22);
        AddSimple(RAC, "Suleiman", "Camara", "Camara", 22, "GIN", 182, 74, RIGHT, ST, 0, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 23);
        AddSimple(RAC, "Giorgi", "Guliashvili", "Guliashvili", 23, "GEO", 186, 78, RIGHT, ST, LW_f, 74, 80, 5_000_000, 1_000_000, 50_000_000, 5, 9);
        AddSimple(RAC, "Asier", "Villalibre", "Villalibre", 27, "ESP", 182, 80, RIGHT, ST, 0, 76, 78, 4_000_000, 1_500_000, 40_000_000, 3, 12);
        AddSimple(RAC, "Manex", "Lozano", "Lozano", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 72, 80, 4_000_000, 800_000, 40_000_000, 5, 24);
        AddSimple(RAC, "Juan Carlos", "Arana", "Arana", 24, "ESP", 176, 68, RIGHT, RW, LW_f, 74, 80, 5_000_000, 1_200_000, 50_000_000, 4, 14);
        AddSimple(RAC, "Jaime", "Mata", "Jaime Mata", 35, "ESP", 184, 78, RIGHT, ST, 0, 76, 76, 2_000_000, 2_000_000, 20_000_000, 1, 26);

        // ══════════════════════════════════════════════════════════
        // GIRONA FC (TeamId=21) — Segunda División
        // ══════════════════════════════════════════════════════════
        int GIR = 21;
        AddSimple(GIR, "Paulo", "Gazzaniga", "Gazzaniga", 34, "ARG", 196, 90, RIGHT, GK, 0, 76, 78, 2_500_000, 2_000_000, 30_000_000, 2, 13);
        AddSimple(GIR, "Rubén", "Blanco", "Rubén Blanco", 30, "ESP", 188, 80, RIGHT, GK, 0, 74, 78, 2_000_000, 1_500_000, 25_000_000, 2, 1);
        AddSimple(GIR, "Vladyslav", "Krapyvtsov", "Krapyvtsov", 20, "UKR", 190, 82, RIGHT, GK, 0, 68, 80, 500_000, 400_000, 20_000_000, 5, 25);
        AddSimple(GIR, "Juan Carlos", "", "Juan Carlos", 37, "ESP", 186, 80, RIGHT, GK, 0, 72, 72, 150_000, 800_000, 10_000_000, 1, 26);
        AddSimple(GIR, "Alejandro", "Francés", "Francés", 23, "ESP", 182, 74, RIGHT, RB, CB_f, 74, 82, 2_000_000, 800_000, 30_000_000, 5, 16);
        AddSimple(GIR, "Daley", "Blind", "Blind", 35, "NLD", 180, 72, LEFT, CB, CDM_f, 78, 78, 1_500_000, 4_000_000, 20_000_000, 1, 17);
        AddSimple(GIR, "David", "López", "D. López", 35, "ESP", 185, 78, RIGHT, CB, 0, 76, 76, 1_000_000, 2_500_000, 15_000_000, 1, 5);
        AddSimple(GIR, "Álex", "Moreno", "Álex Moreno", 32, "ESP", 179, 70, LEFT, LB, LM_f, 78, 79, 5_000_000, 3_000_000, 50_000_000, 2, 12);
        AddSimple(GIR, "Arnau", "Martínez", "Arnau Martínez", 22, "ESP", 183, 74, RIGHT, RB, CB_f, 76, 84, 10_000_000, 1_500_000, 80_000_000, 3, 4);
        AddSimple(GIR, "Hugo", "Rincón", "Rincón", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 72, 80, 2_000_000, 600_000, 25_000_000, 3, 29);
        AddSimple(GIR, "Azzedine", "Ounahi", "Ounahi", 26, "MAR", 182, 72, RIGHT, CM, CAM_f, 78, 82, 8_000_000, 3_000_000, 60_000_000, 3, 14);
        AddSimple(GIR, "Iván", "Martín", "Iván Martín", 26, "ESP", 178, 70, RIGHT, CM, CAM_f, 78, 81, 6_000_000, 2_500_000, 50_000_000, 4, 23);
        AddSimple(GIR, "Fran", "Beltrán", "Beltrán", 25, "ESP", 170, 64, RIGHT, CDM, CM_f, 76, 80, 5_000_000, 1_500_000, 40_000_000, 4, 20);
        AddSimple(GIR, "Donny", "van de Beek", "Van de Beek", 28, "NLD", 184, 76, RIGHT, CM, CAM_f, 76, 78, 5_000_000, 3_000_000, 50_000_000, 3, 6);
        AddSimple(GIR, "Lass", "Kourouma", "Kourouma", 21, "MLI", 182, 74, RIGHT, CDM, CM_f, 70, 82, 2_000_000, 400_000, 30_000_000, 5, 27);
        AddSimple(GIR, "Bryan", "Gil", "Bryan Gil", 25, "ESP", 175, 68, LEFT, LW, RW_f, 78, 82, 12_000_000, 3_500_000, 80_000_000, 5, 21);
        AddSimple(GIR, "Joel", "Roca", "Joel Roca", 20, "ESP", 174, 66, RIGHT, LW, RW_f, 72, 84, 3_000_000, 600_000, 40_000_000, 5, 30);
        AddSimple(GIR, "Viktor", "Tsygankov", "Tsygankov", 27, "UKR", 178, 70, LEFT, RW, LW_f, 82, 84, 18_000_000, 4_000_000, 120_000_000, 3, 8);
        AddSimple(GIR, "Portu", "", "Portu", 33, "ESP", 178, 72, RIGHT, RW, LW_f, 76, 77, 2_000_000, 2_000_000, 25_000_000, 2, 24);
        AddSimple(GIR, "Vladyslav", "Vanat", "Vanat", 24, "UKR", 184, 76, RIGHT, ST, CF_f, 76, 80, 6_000_000, 1_500_000, 50_000_000, 4, 19);
        AddSimple(GIR, "Abel", "Ruiz", "Abel Ruiz", 25, "ESP", 182, 74, RIGHT, ST, CF_f, 76, 80, 4_000_000, 1_500_000, 40_000_000, 4, 9);
        AddSimple(GIR, "Cristhian", "Stuani", "Stuani", 39, "URY", 184, 80, RIGHT, ST, CF_f, 78, 78, 1_000_000, 3_000_000, 15_000_000, 1, 7);

        // ══════════════════════════════════════════════════════════
        // UD LAS PALMAS (TeamId=22)
        // ══════════════════════════════════════════════════════════
        int PAL = 22;
        AddSimple(PAL, "Dinko", "Horkas", "Horkas", 27, "HRV", 194, 86, RIGHT, GK, 0, 74, 78, 2_500_000, 1_200_000, 30_000_000, 3, 1);
        AddSimple(PAL, "José Antonio", "Caro", "Caro", 32, "ESP", 186, 80, RIGHT, GK, 0, 72, 74, 400_000, 1_000_000, 10_000_000, 2, 13);
        AddSimple(PAL, "Adri", "Suárez", "Adri Suárez", 21, "ESP", 184, 76, RIGHT, GK, 0, 66, 78, 200_000, 200_000, 10_000_000, 5, 35);
        AddSimple(PAL, "Mika", "Mármol", "Mármol", 24, "ESP", 181, 72, LEFT, CB, LB_f, 76, 84, 10_000_000, 1_500_000, 80_000_000, 2, 3);
        AddSimple(PAL, "Juanma", "Herzog", "Herzog", 22, "ESP", 186, 78, RIGHT, CB, 0, 72, 82, 5_000_000, 800_000, 50_000_000, 4, 15);
        AddSimple(PAL, "Álex", "Suárez", "Álex Suárez", 33, "ESP", 179, 72, RIGHT, CB, 0, 74, 75, 600_000, 1_500_000, 15_000_000, 3, 4);
        AddSimple(PAL, "Enrique", "Clemente", "Clemente", 27, "ESP", 184, 76, LEFT, CB, LB_f, 74, 78, 2_500_000, 1_500_000, 30_000_000, 4, 5);
        AddSimple(PAL, "Cristian", "Gutiérrez", "C. Gutiérrez", 25, "ESP", 177, 70, LEFT, LB, LM_f, 74, 78, 1_200_000, 800_000, 15_000_000, 3, 23);
        AddSimple(PAL, "Viti", "Rozada", "Rozada", 28, "ESP", 171, 70, RIGHT, RB, RM_f, 74, 77, 1_500_000, 1_200_000, 20_000_000, 3, 17);
        AddSimple(PAL, "Marvin", "Park", "Marvin", 25, "ESP", 177, 66, RIGHT, RB, RM_f, 74, 80, 1_200_000, 1_000_000, 15_000_000, 3, 2);
        AddSimple(PAL, "Enzo", "Loiodice", "Loiodice", 25, "FRA", 176, 64, RIGHT, CM, CDM_f, 76, 80, 3_000_000, 1_500_000, 30_000_000, 2, 12);
        AddSimple(PAL, "Iñaki", "González", "Iñaki González", 21, "ESP", 180, 72, RIGHT, CM, CAM_f, 68, 78, 500_000, 300_000, 10_000_000, 5, 26);
        AddSimple(PAL, "Nicolás", "Benedetti", "Benedetti", 29, "COL", 180, 70, RIGHT, CAM, LW_f, 76, 78, 2_000_000, 1_500_000, 20_000_000, 2, 7);
        AddSimple(PAL, "Kirian", "Rodríguez", "Kirian", 30, "ESP", 180, 72, RIGHT, CM, CAM_f, 76, 78, 1_000_000, 2_000_000, 20_000_000, 3, 20);
        AddSimple(PAL, "Iván", "Gil", "Iván Gil", 26, "ESP", 168, 62, RIGHT, CM, CAM_f, 74, 78, 800_000, 1_200_000, 10_000_000, 3, 8);
        AddSimple(PAL, "Jonathan", "Viera", "Jonathan Viera", 36, "ESP", 171, 66, RIGHT, CAM, LW_f, 76, 76, 300_000, 2_000_000, 10_000_000, 1, 21);
        AddSimple(PAL, "Jeremía", "Recoba", "Recoba", 22, "URY", 176, 74, LEFT, LW, RW_f, 74, 82, 2_500_000, 600_000, 30_000_000, 4, 9);
        AddSimple(PAL, "Jesé", "", "Jesé", 33, "ESP", 178, 73, RIGHT, LW, RW_f, 74, 75, 300_000, 1_500_000, 10_000_000, 1, 10);
        AddSimple(PAL, "Manu", "Fuster", "Fuster", 28, "ESP", 169, 64, RIGHT, RW, LW_f, 76, 78, 2_000_000, 1_500_000, 20_000_000, 3, 14);
        AddSimple(PAL, "Taisei", "Miyashiro", "Miyashiro", 26, "JPN", 178, 72, RIGHT, RW, ST_f, 74, 78, 1_800_000, 1_200_000, 20_000_000, 2, 18);
        AddSimple(PAL, "Ale", "García", "Ale García", 23, "ESP", 179, 70, RIGHT, RW, LW_f, 72, 80, 1_500_000, 600_000, 15_000_000, 5, 22);
        AddSimple(PAL, "Pejiño", "", "Pejiño", 29, "ESP", 178, 68, RIGHT, LW, RW_f, 74, 78, 600_000, 1_200_000, 10_000_000, 2, 24);
        AddSimple(PAL, "Sandro", "Ramírez", "Sandro", 30, "ESP", 175, 72, RIGHT, ST, LW_f, 74, 76, 700_000, 1_500_000, 10_000_000, 1, 19);

        // ══════════════════════════════════════════════════════════
        // RCD MALLORCA (TeamId=23)
        // ══════════════════════════════════════════════════════════
        int MLL = 23;
        AddSimple(MLL, "Leo", "Román", "Leo Román", 25, "ESP", 189, 80, RIGHT, GK, 0, 76, 82, 6_000_000, 1_500_000, 60_000_000, 5, 1);
        AddSimple(MLL, "Lucas", "Bergström", "Bergström", 23, "FIN", 205, 96, RIGHT, GK, 0, 72, 80, 1_500_000, 600_000, 20_000_000, 3, 13);
        AddSimple(MLL, "Iván", "Cuéllar", "Cuéllar", 42, "ESP", 187, 82, LEFT, GK, 0, 70, 70, 100_000, 500_000, 5_000_000, 1, 25);
        AddSimple(MLL, "Martin", "Valjent", "Valjent", 30, "SVK", 187, 76, RIGHT, CB, 0, 78, 80, 4_500_000, 2_500_000, 40_000_000, 4, 24);
        AddSimple(MLL, "David", "López", "D. López", 23, "ESP", 195, 82, RIGHT, CB, 0, 74, 80, 3_000_000, 800_000, 30_000_000, 3, 27);
        AddSimple(MLL, "Antonio", "Raíllo", "Raíllo", 34, "ESP", 187, 80, RIGHT, CB, 0, 78, 78, 1_500_000, 2_000_000, 20_000_000, 2, 21);
        AddSimple(MLL, "Johan", "Mojica", "Mojica", 33, "COL", 185, 72, LEFT, LB, LM_f, 76, 78, 1_800_000, 2_000_000, 20_000_000, 2, 22);
        AddSimple(MLL, "Toni", "Lato", "Lato", 28, "ESP", 171, 68, LEFT, LB, LM_f, 76, 78, 1_500_000, 1_500_000, 20_000_000, 2, 3);
        AddSimple(MLL, "Pablo", "Maffeo", "Maffeo", 28, "ARG", 172, 70, RIGHT, RB, RM_f, 78, 80, 5_000_000, 2_500_000, 50_000_000, 2, 23);
        AddSimple(MLL, "Mateu", "Morey", "Morey", 26, "ESP", 173, 68, RIGHT, RB, RM_f, 74, 78, 1_500_000, 1_200_000, 20_000_000, 2, 2);
        AddSimple(MLL, "Samú", "Costa", "Samú Costa", 25, "PRT", 185, 78, RIGHT, CDM, CM_f, 78, 84, 15_000_000, 2_500_000, 100_000_000, 4, 12);
        AddSimple(MLL, "Omar", "Mascarell", "Mascarell", 33, "GNQ", 181, 76, RIGHT, CDM, CM_f, 76, 77, 700_000, 2_000_000, 10_000_000, 2, 5);
        AddSimple(MLL, "Sergi", "Darder", "Darder", 32, "ESP", 180, 72, RIGHT, CM, CAM_f, 78, 80, 3_000_000, 3_000_000, 30_000_000, 3, 10);
        AddSimple(MLL, "Manu", "Morlanes", "Morlanes", 27, "ESP", 178, 74, RIGHT, CM, CDM_f, 76, 78, 2_400_000, 1_500_000, 20_000_000, 3, 8);
        AddSimple(MLL, "Antonio", "Sánchez", "A. Sánchez", 29, "ESP", 179, 76, RIGHT, CM, CAM_f, 74, 78, 2_000_000, 1_500_000, 20_000_000, 2, 6);
        AddSimple(MLL, "Jan", "Salas", "Salas", 20, "ESP", 182, 76, RIGHT, CM, CAM_f, 70, 82, 1_200_000, 400_000, 15_000_000, 5, 41);
        AddSimple(MLL, "Pablo", "Torre", "Pablo Torre", 23, "ESP", 173, 62, RIGHT, CAM, CM_f, 76, 84, 5_000_000, 1_200_000, 50_000_000, 5, 20);
        AddSimple(MLL, "Jan", "Virgili", "Virgili", 19, "ESP", 177, 72, RIGHT, LW, RW_f, 74, 86, 15_000_000, 1_000_000, 100_000_000, 5, 17);
        AddSimple(MLL, "Javi", "Llabrés", "Llabrés", 23, "ESP", 174, 70, RIGHT, LW, RW_f, 74, 80, 800_000, 800_000, 10_000_000, 2, 19);
        AddSimple(MLL, "Takuma", "Asano", "Asano", 31, "JPN", 173, 70, RIGHT, RW, LW_f, 76, 78, 1_800_000, 2_000_000, 20_000_000, 2, 11);
        AddSimple(MLL, "Justin", "Kalumba", "Kalumba", 21, "FRA", 182, 74, RIGHT, RW, LW_f, 72, 82, 2_000_000, 500_000, 25_000_000, 5, 30);
        AddSimple(MLL, "Abdón", "Prats", "Abdón", 33, "ESP", 181, 78, RIGHT, ST, 0, 76, 78, 900_000, 2_000_000, 10_000_000, 3, 9);

        // ══════════════════════════════════════════════════════════
        // GRANADA CF (TeamId=24)
        // ══════════════════════════════════════════════════════════
        int GRA = 24;
        AddSimple(GRA, "Luca", "Zidane", "Luca Zidane", 26, "FRA", 183, 76, RIGHT, GK, 0, 72, 76, 1_200_000, 800_000, 10_000_000, 3, 13);
        AddSimple(GRA, "Ander", "Astralaga", "Astralaga", 21, "ESP", 186, 78, RIGHT, GK, 0, 68, 80, 800_000, 300_000, 10_000_000, 5, 1);
        AddSimple(GRA, "Loïc", "Williams", "Loïc", 23, "ESP", 186, 78, RIGHT, CB, LB_f, 72, 80, 2_500_000, 600_000, 25_000_000, 5, 2);
        AddSimple(GRA, "Manu", "Lama", "Lama", 22, "ESP", 184, 76, RIGHT, CB, 0, 70, 80, 1_000_000, 300_000, 15_000_000, 5, 3);
        AddSimple(GRA, "Oscar", "Naasei", "Naasei", 22, "GHA", 188, 80, RIGHT, CB, 0, 68, 78, 800_000, 300_000, 10_000_000, 5, 4);
        AddSimple(GRA, "Bambo", "Diaby", "Diaby", 27, "SEN", 190, 84, RIGHT, CB, 0, 74, 78, 2_000_000, 800_000, 20_000_000, 3, 5);
        AddSimple(GRA, "Juanjo", "Flores", "Juanjo", 22, "ESP", 178, 72, LEFT, LB, LM_f, 72, 80, 1_200_000, 400_000, 15_000_000, 5, 14);
        AddSimple(GRA, "Baïla", "Diallo", "Diallo", 22, "SEN", 180, 72, LEFT, LB, LM_f, 70, 80, 1_000_000, 300_000, 10_000_000, 5, 15);
        AddSimple(GRA, "Diego", "Hormigo", "Hormigo", 21, "ESP", 176, 68, RIGHT, RB, RM_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 16);
        AddSimple(GRA, "Pau", "Casadesús", "Casadesús", 20, "ESP", 180, 72, RIGHT, RB, 0, 68, 78, 500_000, 200_000, 8_000_000, 5, 17);
        AddSimple(GRA, "Álvaro", "Lemos", "Lemos", 31, "ESP", 178, 72, RIGHT, RB, RM_f, 74, 76, 800_000, 1_200_000, 10_000_000, 2, 20);
        AddSimple(GRA, "Pedro", "Alemañ", "Alemañ", 22, "ESP", 180, 72, RIGHT, CM, CDM_f, 72, 80, 1_500_000, 400_000, 15_000_000, 5, 8);
        AddSimple(GRA, "Izan", "González", "Izan", 21, "ESP", 176, 68, RIGHT, CM, CAM_f, 70, 82, 1_500_000, 400_000, 15_000_000, 5, 10);
        AddSimple(GRA, "Sergio", "Ruiz", "S. Ruiz", 30, "ESP", 180, 72, RIGHT, CDM, CM_f, 74, 76, 1_500_000, 1_000_000, 15_000_000, 2, 6);
        AddSimple(GRA, "Rubén", "Alcaraz", "Alcaraz", 33, "ESP", 180, 74, RIGHT, CM, CDM_f, 76, 77, 1_000_000, 1_500_000, 10_000_000, 2, 21);
        AddSimple(GRA, "Manu", "Trigueros", "Trigueros", 33, "ESP", 178, 72, RIGHT, CM, CAM_f, 76, 77, 800_000, 1_500_000, 10_000_000, 2, 11);
        AddSimple(GRA, "José Manuel", "Arnáiz", "Arnáiz", 29, "ESP", 176, 68, RIGHT, LW, RW_f, 74, 78, 1_500_000, 1_000_000, 15_000_000, 3, 7);
        AddSimple(GRA, "Sergio", "Rodelas", "Rodelas", 21, "ESP", 174, 66, RIGHT, LW, RW_f, 72, 82, 1_500_000, 400_000, 15_000_000, 5, 18);
        AddSimple(GRA, "Álex", "Sola", "Sola", 25, "ESP", 178, 72, RIGHT, RW, LW_f, 74, 78, 1_200_000, 800_000, 15_000_000, 3, 19);
        AddSimple(GRA, "Pablo", "Sáenz", "P. Sáenz", 23, "ESP", 182, 74, RIGHT, ST, LW_f, 72, 80, 1_500_000, 600_000, 15_000_000, 4, 9);
        AddSimple(GRA, "Jorge", "Pascual", "Pascual", 22, "ESP", 184, 76, RIGHT, ST, CF_f, 72, 80, 1_200_000, 400_000, 15_000_000, 5, 12);

        // ══════════════════════════════════════════════════════════
        // REAL VALLADOLID (TeamId=25)
        // ══════════════════════════════════════════════════════════
        int VLD = 25;
        AddSimple(VLD, "Guilherme", "Fernandes", "Guilherme", 24, "BRA", 186, 78, RIGHT, GK, 0, 72, 80, 1_500_000, 600_000, 15_000_000, 4, 1);
        AddSimple(VLD, "Álvaro", "Aceves", "Aceves", 22, "ESP", 184, 76, RIGHT, GK, 0, 68, 78, 500_000, 300_000, 8_000_000, 5, 13);
        AddSimple(VLD, "David", "Torres", "David Torres", 22, "ESP", 186, 78, RIGHT, CB, 0, 70, 80, 1_000_000, 300_000, 10_000_000, 5, 2);
        AddSimple(VLD, "Pablo", "Tomeo", "Tomeo", 21, "ESP", 180, 72, RIGHT, CB, CDM_f, 70, 80, 1_000_000, 300_000, 10_000_000, 5, 3);
        AddSimple(VLD, "Ramón", "Martínez", "Ramón", 23, "ESP", 184, 76, RIGHT, CB, 0, 72, 80, 1_200_000, 500_000, 15_000_000, 5, 4);
        AddSimple(VLD, "Mohamed", "Jaouab", "Jaouab", 22, "MAR", 188, 80, RIGHT, CB, 0, 70, 80, 1_000_000, 300_000, 10_000_000, 5, 5);
        AddSimple(VLD, "Guille", "Bueno", "Bueno", 22, "ESP", 178, 72, LEFT, LB, LM_f, 72, 82, 3_000_000, 600_000, 25_000_000, 5, 15);
        AddSimple(VLD, "Hugo", "San Modesto", "San Modesto", 21, "ESP", 176, 68, LEFT, LB, 0, 68, 78, 500_000, 200_000, 8_000_000, 5, 16);
        AddSimple(VLD, "Iván", "Garriel", "Garriel", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 6);
        AddSimple(VLD, "Carlos", "Clerc", "Clerc", 32, "ESP", 182, 74, LEFT, LB, LM_f, 74, 76, 500_000, 1_200_000, 10_000_000, 2, 12);
        AddSimple(VLD, "Iván", "Alejo", "Alejo", 25, "ESP", 176, 70, RIGHT, RW, RB_f, 74, 78, 1_200_000, 800_000, 15_000_000, 3, 7);
        AddSimple(VLD, "Stanko", "Juric", "Juric", 28, "HRV", 182, 74, RIGHT, CM, CDM_f, 74, 78, 1_000_000, 1_200_000, 15_000_000, 2, 8);
        AddSimple(VLD, "Mathis", "Lachuer", "Lachuer", 24, "FRA", 178, 72, RIGHT, CM, CAM_f, 72, 78, 800_000, 500_000, 10_000_000, 3, 14);
        AddSimple(VLD, "Julien", "Ponceau", "Ponceau", 24, "FRA", 178, 70, RIGHT, CM, CAM_f, 72, 78, 1_000_000, 500_000, 10_000_000, 3, 9);
        AddSimple(VLD, "Víctor", "Meseguer", "Meseguer", 26, "ESP", 184, 76, RIGHT, CM, CDM_f, 74, 78, 1_200_000, 800_000, 15_000_000, 3, 10);
        AddSimple(VLD, "Ibrahim", "Alani", "Alani", 22, "IRQ", 182, 74, RIGHT, CM, CAM_f, 68, 78, 500_000, 200_000, 8_000_000, 5, 18);
        AddSimple(VLD, "Mario", "Maroto", "Maroto", 21, "ESP", 176, 68, RIGHT, CM, CAM_f, 68, 80, 600_000, 200_000, 10_000_000, 5, 19);
        AddSimple(VLD, "Chuki", "", "Chuki", 22, "ESP", 174, 66, RIGHT, LW, RW_f, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 20);
        AddSimple(VLD, "Stipe", "Biuk", "Biuk", 22, "HRV", 184, 76, RIGHT, LW, RW_f, 74, 82, 2_500_000, 600_000, 25_000_000, 4, 21);
        AddSimple(VLD, "Lucas", "Sanseviero", "Sanseviero", 21, "URY", 178, 72, LEFT, LW, RW_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 22);
        AddSimple(VLD, "Amath", "Ndiaye", "Ndiaye", 28, "SEN", 173, 68, RIGHT, RW, LW_f, 74, 78, 1_500_000, 1_200_000, 15_000_000, 2, 11);
        AddSimple(VLD, "Juanmi", "Latasa", "Latasa", 23, "ESP", 186, 78, RIGHT, ST, 0, 74, 80, 2_000_000, 800_000, 20_000_000, 4, 23);
        AddSimple(VLD, "Vegard", "Erlien", "Erlien", 23, "NOR", 184, 76, RIGHT, ST, LW_f, 72, 78, 800_000, 500_000, 10_000_000, 4, 24);
        AddSimple(VLD, "Marcos", "André", "Marcos André", 28, "BRA", 184, 78, RIGHT, ST, 0, 74, 78, 1_500_000, 1_200_000, 15_000_000, 2, 17);

        // ══════════════════════════════════════════════════════════
        // CÁDIZ CF (TeamId=26)
        // ══════════════════════════════════════════════════════════
        int CAD = 26;
        AddSimple(CAD, "Victor", "Aznar", "V. Aznar", 22, "ESP", 186, 78, RIGHT, GK, 0, 68, 78, 500_000, 200_000, 8_000_000, 5, 1);
        AddSimple(CAD, "David", "Gil", "David Gil", 30, "ESP", 186, 80, RIGHT, GK, 0, 72, 74, 600_000, 800_000, 8_000_000, 2, 13);
        AddSimple(CAD, "Bojan", "Kovacevic", "Kovacevic", 22, "HRV", 188, 80, RIGHT, GK, 0, 70, 80, 800_000, 300_000, 10_000_000, 5, 26);
        AddSimple(CAD, "Iker", "Recio", "Recio", 22, "ESP", 182, 74, RIGHT, CB, 0, 70, 80, 800_000, 300_000, 10_000_000, 5, 2);
        AddSimple(CAD, "Jorge", "Moreno", "Jorge Moreno", 22, "ESP", 184, 76, RIGHT, CB, 0, 70, 80, 600_000, 300_000, 8_000_000, 5, 3);
        AddSimple(CAD, "Pelayo", "Fernández", "Pelayo", 22, "ESP", 184, 76, RIGHT, CB, 0, 72, 80, 1_200_000, 400_000, 15_000_000, 5, 4);
        AddSimple(CAD, "Sergio", "Arribas", "S. Arribas", 23, "ESP", 180, 72, RIGHT, CB, CDM_f, 72, 80, 1_000_000, 500_000, 10_000_000, 4, 5);
        AddSimple(CAD, "Mario", "Climent", "Climent", 22, "ESP", 176, 68, LEFT, LB, LM_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 14);
        AddSimple(CAD, "Raúl", "Pereira", "Pereira", 21, "ESP", 178, 72, LEFT, LB, 0, 68, 78, 500_000, 200_000, 8_000_000, 5, 15);
        AddSimple(CAD, "Iza", "Carcelén", "Iza", 31, "ESP", 176, 70, RIGHT, RB, RM_f, 74, 76, 600_000, 1_200_000, 10_000_000, 2, 20);
        AddSimple(CAD, "Juan", "Díaz", "Juan Díaz", 22, "ESP", 180, 72, RIGHT, RB, 0, 68, 78, 500_000, 200_000, 8_000_000, 5, 16);
        AddSimple(CAD, "Moussa", "Diakité", "Diakité", 22, "MLI", 186, 78, RIGHT, CDM, CM_f, 70, 80, 1_000_000, 300_000, 10_000_000, 5, 6);
        AddSimple(CAD, "Sergio", "Ortuño", "Ortuño", 23, "ESP", 178, 72, RIGHT, CM, CDM_f, 72, 80, 800_000, 400_000, 10_000_000, 4, 8);
        AddSimple(CAD, "Yussi", "Diarra", "Diarra", 22, "MLI", 182, 74, RIGHT, CM, CAM_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 17);
        AddSimple(CAD, "Rominigue", "Kouamé", "Kouamé", 27, "MLI", 184, 76, RIGHT, CM, CDM_f, 74, 78, 1_200_000, 800_000, 15_000_000, 3, 18);
        AddSimple(CAD, "Álex", "Fernández", "Álex Fernández", 32, "ESP", 180, 74, RIGHT, CM, CAM_f, 76, 77, 800_000, 1_500_000, 10_000_000, 2, 10);
        AddSimple(CAD, "Joaquín", "González", "Joaquín", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 21);
        AddSimple(CAD, "Brian", "Ocampo", "Ocampo", 25, "URY", 172, 66, LEFT, LW, RW_f, 74, 78, 1_500_000, 800_000, 15_000_000, 3, 11);
        AddSimple(CAD, "Javi", "Ontiveros", "Ontiveros", 27, "ESP", 176, 70, RIGHT, RW, LW_f, 74, 78, 1_200_000, 1_000_000, 15_000_000, 2, 9);
        AddSimple(CAD, "Iuri", "Tabatadze", "Tabatadze", 22, "GEO", 180, 72, RIGHT, RW, LW_f, 72, 80, 1_000_000, 300_000, 10_000_000, 5, 22);
        AddSimple(CAD, "José Antonio", "de la Rosa", "De la Rosa", 22, "ESP", 178, 72, RIGHT, LW, RW_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 23);
        AddSimple(CAD, "Suso", "", "Suso", 31, "ESP", 176, 70, LEFT, RW, LW_f, 78, 79, 2_500_000, 3_000_000, 30_000_000, 2, 7);
        AddSimple(CAD, "Jerónimo", "Dómina", "Dómina", 22, "ARG", 182, 74, RIGHT, ST, CF_f, 72, 80, 1_200_000, 400_000, 15_000_000, 5, 24);
        AddSimple(CAD, "Álvaro", "García Pascual", "Álvaro G.", 22, "ESP", 184, 76, RIGHT, ST, 0, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 25);

        // ══════════════════════════════════════════════════════════
        // UD ALMERÍA (TeamId=27)
        // ══════════════════════════════════════════════════════════
        int ALM = 27;
        AddSimple(ALM, "Bruno", "Iribarne", "Bruno", 22, "ESP", 184, 76, RIGHT, GK, 0, 70, 80, 800_000, 300_000, 10_000_000, 5, 1);
        AddSimple(ALM, "Andrés", "Fernández", "Andrés Fernández", 38, "ESP", 186, 82, RIGHT, GK, 0, 72, 72, 200_000, 800_000, 5_000_000, 1, 13);
        AddSimple(ALM, "Fernando", "Martínez", "F. Martínez", 34, "ESP", 188, 82, RIGHT, GK, 0, 72, 73, 300_000, 800_000, 5_000_000, 2, 25);
        AddSimple(ALM, "Federico", "Bonini", "Bonini", 23, "URY", 186, 78, RIGHT, CB, 0, 72, 80, 1_500_000, 500_000, 15_000_000, 5, 2);
        AddSimple(ALM, "Chumi", "", "Chumi", 25, "ESP", 186, 78, RIGHT, CB, 0, 74, 78, 1_200_000, 600_000, 10_000_000, 3, 3);
        AddSimple(ALM, "Nélson", "Monte", "Monte", 29, "PRT", 186, 80, RIGHT, CB, 0, 74, 78, 1_000_000, 800_000, 10_000_000, 3, 4);
        AddSimple(ALM, "Rodrigo", "Ely", "Ely", 31, "BRA", 188, 82, RIGHT, CB, 0, 74, 76, 800_000, 1_000_000, 10_000_000, 2, 5);
        AddSimple(ALM, "Pedro", "Fidel", "P. Fidel", 22, "ESP", 178, 72, LEFT, LB, LM_f, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 14);
        AddSimple(ALM, "Álex", "Centelles", "Centelles", 25, "ESP", 176, 70, LEFT, LB, LM_f, 74, 78, 1_500_000, 800_000, 15_000_000, 3, 15);
        AddSimple(ALM, "Álex", "Muñoz", "Álex Muñoz", 30, "ESP", 182, 76, LEFT, LB, CB_f, 74, 76, 800_000, 1_200_000, 10_000_000, 2, 20);
        AddSimple(ALM, "Daijiro", "Chirino", "Chirino", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 72, 80, 1_200_000, 400_000, 15_000_000, 5, 6);
        AddSimple(ALM, "Marcos", "Luna", "Luna", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 16);
        AddSimple(ALM, "Dion", "Lopy", "Lopy", 22, "SEN", 184, 76, RIGHT, CDM, CM_f, 74, 82, 3_000_000, 600_000, 25_000_000, 5, 7);
        AddSimple(ALM, "Stefan", "Dzodic", "Dzodic", 21, "SRB", 182, 74, RIGHT, CM, CDM_f, 70, 80, 800_000, 300_000, 10_000_000, 5, 8);
        AddSimple(ALM, "Iddrisu", "Baba", "Baba", 28, "GHA", 182, 76, RIGHT, CDM, CM_f, 74, 77, 1_200_000, 800_000, 10_000_000, 2, 17);
        AddSimple(ALM, "Gui", "Guedes", "Gui Guedes", 22, "PRT", 180, 72, RIGHT, CM, CAM_f, 72, 80, 1_500_000, 500_000, 15_000_000, 5, 9);
        AddSimple(ALM, "Sergio", "Arribas", "Arribas", 23, "ESP", 176, 68, RIGHT, CAM, RW_f, 74, 82, 3_000_000, 800_000, 25_000_000, 5, 10);
        AddSimple(ALM, "Nico", "Melamed", "Melamed", 23, "ESP", 180, 70, LEFT, CAM, LW_f, 74, 82, 2_500_000, 800_000, 20_000_000, 4, 11);
        AddSimple(ALM, "Jon", "Morcillo", "Morcillo", 26, "ESP", 184, 76, RIGHT, LW, RW_f, 74, 78, 1_200_000, 800_000, 15_000_000, 3, 18);
        AddSimple(ALM, "Arnau", "Puigmal", "Puigmal", 24, "ESP", 180, 72, RIGHT, RW, CM_f, 74, 78, 1_200_000, 600_000, 15_000_000, 3, 19);
        AddSimple(ALM, "Patrick", "Soko", "Soko", 27, "SEN", 182, 74, RIGHT, RW, LW_f, 72, 76, 500_000, 600_000, 8_000_000, 2, 21);
        AddSimple(ALM, "Léo", "Baptistão", "Baptistão", 32, "BRA", 185, 80, RIGHT, ST, 0, 74, 76, 800_000, 1_200_000, 10_000_000, 2, 12);
        AddSimple(ALM, "Adrián", "Embarba", "Embarba", 33, "ESP", 173, 68, RIGHT, RW, LW_f, 76, 77, 600_000, 1_500_000, 10_000_000, 2, 22);
        AddSimple(ALM, "Thalys", "", "Thalys", 21, "BRA", 178, 72, RIGHT, ST, LW_f, 72, 82, 2_000_000, 500_000, 20_000_000, 5, 23);
        AddSimple(ALM, "Miguel", "de la Fuente", "De la Fuente", 25, "ESP", 182, 74, RIGHT, ST, 0, 74, 78, 1_200_000, 600_000, 10_000_000, 3, 24);
        AddSimple(ALM, "Ibrahima", "Koné", "Koné", 25, "MLI", 190, 82, RIGHT, ST, 0, 72, 78, 1_000_000, 500_000, 10_000_000, 3, 26);

        // ══════════════════════════════════════════════════════════
        // CD LEGANÉS (TeamId=28)
        // ══════════════════════════════════════════════════════════
        int LEG = 28;
        AddSimple(LEG, "Juan", "Soriano", "Soriano", 27, "ESP", 185, 78, RIGHT, GK, GK_f, 72, 78, 6_000_000, 800_000, 50_000_000, 3, 1);
        AddSimple(LEG, "Miguel", "San Román", "San Román", 26, "ESP", 186, 76, RIGHT, GK, GK_f, 68, 76, 3_000_000, 400_000, 30_000_000, 4, 13);
        AddSimple(LEG, "Lalo", "Aguilar", "Aguilar", 21, "ESP", 183, 72, RIGHT, CB, RB_f, 65, 78, 2_500_000, 200_000, 25_000_000, 5, 2);
        AddSimple(LEG, "Marvelous", "Garzón", "Marvel", 21, "ESP", 187, 76, RIGHT, CB, RB_f, 63, 78, 2_000_000, 200_000, 25_000_000, 5, 3);
        AddSimple(LEG, "Jorge", "Sáenz", "J. Sáenz", 28, "ESP", 188, 78, RIGHT, CB, CDM_f, 68, 72, 4_000_000, 500_000, 30_000_000, 2, 4);
        AddSimple(LEG, "Rubén", "Pulido", "Pulido", 26, "ESP", 185, 76, RIGHT, CB, RB_f, 66, 72, 3_500_000, 350_000, 25_000_000, 3, 5);
        AddSimple(LEG, "Ignasi", "Miquel", "I. Miquel", 32, "ESP", 189, 80, RIGHT, CB, CDM_f, 70, 72, 3_000_000, 500_000, 20_000_000, 1, 6);
        AddSimple(LEG, "Enric", "Franquesa", "Franquesa", 27, "ESP", 177, 70, LEFT, LB, LM_f, 70, 74, 4_000_000, 600_000, 40_000_000, 3, 7);
        AddSimple(LEG, "Rubén", "Peña", "R. Peña", 33, "ESP", 180, 74, RIGHT, RB, RM_f, 70, 72, 2_000_000, 500_000, 30_000_000, 1, 8);
        AddSimple(LEG, "Sebastián", "Figueredo", "Figueredo", 22, "URY", 178, 72, RIGHT, RB, CDM_f, 64, 76, 2_000_000, 250_000, 25_000_000, 5, 9);
        AddSimple(LEG, "Marcos", "Leiva", "Leiva", 24, "ESP", 176, 68, RIGHT, CM, CDM_f, 66, 76, 2_500_000, 250_000, 25_000_000, 4, 10);
        AddSimple(LEG, "Amadou", "Diawara", "Diawara", 27, "GIN", 184, 76, RIGHT, CDM, CM_f, 68, 72, 3_500_000, 300_000, 20_000_000, 2, 11);
        AddSimple(LEG, "Carlos", "Guirao", "Guirao", 21, "ESP", 178, 70, RIGHT, CM, CAM_f, 62, 76, 1_500_000, 150_000, 20_000_000, 5, 12);
        AddSimple(LEG, "Seydouba", "Cissé", "S. Cissé", 23, "GIN", 175, 68, RIGHT, CM, CAM_f, 64, 74, 2_000_000, 200_000, 20_000_000, 4, 14);
        AddSimple(LEG, "Gonzalo", "Melero", "Melero", 30, "ESP", 183, 76, RIGHT, CM, CAM_f, 70, 72, 4_000_000, 600_000, 30_000_000, 2, 15);
        AddSimple(LEG, "Roberto", "López", "R. López", 24, "ESP", 176, 68, RIGHT, CAM, RW_f, 66, 76, 3_000_000, 300_000, 25_000_000, 4, 16);
        AddSimple(LEG, "Óscar", "Plano", "Ó. Plano", 33, "ESP", 177, 70, RIGHT, RW, LW_f, 68, 70, 2_000_000, 400_000, 20_000_000, 1, 17);
        AddSimple(LEG, "Daniel", "Rodríguez", "Dani Rodríguez", 36, "ESP", 175, 68, RIGHT, CM, CAM_f, 66, 68, 1_500_000, 400_000, 15_000_000, 1, 18);
        AddSimple(LEG, "Duk", "", "Duk", 24, "CPV", 172, 66, RIGHT, LW, RW_f, 64, 74, 2_500_000, 250_000, 25_000_000, 4, 19);
        AddSimple(LEG, "Juan", "Cruz", "Juan Cruz", 25, "ESP", 174, 66, RIGHT, LW, RW_f, 62, 72, 2_000_000, 250_000, 20_000_000, 3, 20);
        AddSimple(LEG, "Andrés", "Campos", "A. Campos", 21, "ESP", 176, 68, RIGHT, CM, CAM_f, 60, 74, 1_200_000, 120_000, 15_000_000, 5, 21);
        AddSimple(LEG, "Naim", "García", "N. García", 22, "ESP", 175, 66, RIGHT, LW, RW_f, 60, 74, 1_500_000, 150_000, 15_000_000, 5, 22);
        AddSimple(LEG, "Luis", "Asué", "Asué", 23, "GNQ", 182, 74, RIGHT, ST, CF_f, 62, 74, 2_000_000, 200_000, 20_000_000, 4, 23);
        AddSimple(LEG, "Diego", "García", "Diego García", 25, "ESP", 185, 76, RIGHT, ST, CF_f, 64, 72, 3_000_000, 300_000, 25_000_000, 3, 24);
        AddSimple(LEG, "Álex", "Millán", "Millán", 25, "ESP", 178, 72, RIGHT, ST, CF_f | LW_f, 64, 74, 2_500_000, 250_000, 25_000_000, 4, 25);

        // ══════════════════════════════════════════════════════════
        // SD EIBAR (TeamId=29)
        // ══════════════════════════════════════════════════════════
        int EIB = 29;
        AddSimple(EIB, "Jonmi", "Magunagoitia", "Magunagoitia", 25, "ESP", 186, 78, RIGHT, GK, 0, 72, 78, 800_000, 500_000, 8_000_000, 3, 1);
        AddSimple(EIB, "Luis", "López", "L. López", 30, "ESP", 188, 80, RIGHT, GK, 0, 72, 74, 500_000, 800_000, 8_000_000, 2, 13);
        AddSimple(EIB, "Marco", "Moreno", "Marco Moreno", 23, "ESP", 184, 76, RIGHT, CB, 0, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 2);
        AddSimple(EIB, "Aritz", "Arambarri", "Arambarri", 26, "ESP", 184, 76, RIGHT, CB, 0, 74, 78, 1_200_000, 600_000, 10_000_000, 3, 3);
        AddSimple(EIB, "Jair", "Amador", "Amador", 35, "ESP", 186, 80, RIGHT, CB, 0, 74, 74, 300_000, 800_000, 5_000_000, 2, 4);
        AddSimple(EIB, "Anaitz", "Arbilla", "Arbilla", 38, "ESP", 178, 72, RIGHT, RB, CB_f, 74, 74, 200_000, 600_000, 3_000_000, 1, 23);
        AddSimple(EIB, "Hodei", "Arrillaga", "Arrillaga", 22, "ESP", 180, 72, RIGHT, CB, 0, 70, 80, 600_000, 200_000, 8_000_000, 5, 5);
        AddSimple(EIB, "Juan", "Bernat", "Bernat", 32, "ESP", 170, 66, LEFT, LB, LM_f, 76, 78, 2_000_000, 2_500_000, 20_000_000, 2, 14);
        AddSimple(EIB, "Sergio", "Cubero", "Cubero", 23, "ESP", 178, 72, LEFT, LB, LM_f, 72, 80, 800_000, 400_000, 10_000_000, 5, 15);
        AddSimple(EIB, "Álvaro", "Rodríguez", "Álvaro R.", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 72, 80, 800_000, 300_000, 10_000_000, 5, 6);
        AddSimple(EIB, "Peru", "Nolaskoain", "Nolaskoain", 26, "ESP", 186, 78, RIGHT, CDM, CB_f, 74, 78, 1_200_000, 600_000, 10_000_000, 3, 7);
        AddSimple(EIB, "Sergio", "Álvarez", "S. Álvarez", 32, "ESP", 180, 74, RIGHT, CM, CDM_f, 74, 76, 500_000, 800_000, 8_000_000, 2, 8);
        AddSimple(EIB, "Aleix", "Garrido", "Garrido", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 72, 80, 1_000_000, 400_000, 10_000_000, 4, 16);
        AddSimple(EIB, "Ander", "Madariaga", "Madariaga", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 70, 80, 600_000, 200_000, 8_000_000, 5, 17);
        AddSimple(EIB, "Lander", "Olaetxea", "Olaetxea", 32, "ESP", 176, 70, RIGHT, CM, CDM_f, 74, 75, 400_000, 800_000, 5_000_000, 2, 22);
        AddSimple(EIB, "Jon", "Magunazelaia", "Magunazelaia", 23, "ESP", 180, 72, RIGHT, CAM, LW_f, 72, 80, 1_000_000, 400_000, 10_000_000, 4, 9);
        AddSimple(EIB, "Javi", "Martínez", "J. Martínez", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 72, 80, 800_000, 300_000, 10_000_000, 5, 10);
        AddSimple(EIB, "Toni", "Villa", "Toni Villa", 27, "ESP", 172, 64, RIGHT, LW, RW_f, 74, 78, 800_000, 800_000, 8_000_000, 3, 19);
        AddSimple(EIB, "Adu", "Ares", "Ares", 23, "ESP", 174, 68, RIGHT, RW, LW_f, 72, 80, 800_000, 400_000, 10_000_000, 4, 20);
        AddSimple(EIB, "Jon", "Guruzeta", "Guruzeta", 22, "ESP", 180, 72, RIGHT, ST, LW_f, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 11);
        AddSimple(EIB, "José", "Corpas", "Corpas", 33, "ESP", 176, 72, RIGHT, RW, LW_f, 74, 75, 300_000, 800_000, 5_000_000, 2, 21);
        AddSimple(EIB, "Javi", "Martón", "Martón", 25, "ESP", 184, 76, RIGHT, ST, 0, 74, 78, 1_000_000, 600_000, 10_000_000, 3, 12);
        AddSimple(EIB, "Jon", "Bautista", "Bautista", 29, "ESP", 182, 76, RIGHT, ST, 0, 74, 78, 800_000, 1_000_000, 10_000_000, 2, 18);

        // ══════════════════════════════════════════════════════════
        // SPORTING DE GIJÓN (TeamId=30)
        // ══════════════════════════════════════════════════════════
        int SPO = 30;
        AddSimple(SPO, "Rubén", "Yáñez", "Yáñez", 31, "ESP", 188, 82, RIGHT, GK, 0, 74, 78, 1_000_000, 1_200_000, 10_000_000, 2, 1);
        AddSimple(SPO, "Christian", "Joel", "Christian Joel", 25, "ESP", 186, 78, RIGHT, GK, 0, 70, 76, 500_000, 400_000, 8_000_000, 3, 13);
        AddSimple(SPO, "Lucas", "Perrin", "Perrin", 26, "FRA", 188, 80, RIGHT, CB, CDM_f, 74, 78, 1_500_000, 800_000, 15_000_000, 3, 2);
        AddSimple(SPO, "Diego", "Sánchez", "Diego Sánchez", 22, "ESP", 184, 76, RIGHT, CB, 0, 70, 80, 800_000, 300_000, 10_000_000, 5, 3);
        AddSimple(SPO, "Eric", "Curbelo", "Curbelo", 30, "ESP", 182, 76, RIGHT, CB, 0, 74, 76, 600_000, 800_000, 8_000_000, 2, 4);
        AddSimple(SPO, "Pablo", "Vázquez", "P. Vázquez", 30, "ESP", 186, 78, RIGHT, CB, 0, 74, 76, 800_000, 800_000, 8_000_000, 2, 5);
        AddSimple(SPO, "Pablo", "García", "Pablo García", 22, "ESP", 180, 72, LEFT, LB, LM_f, 72, 80, 800_000, 300_000, 10_000_000, 5, 14);
        AddSimple(SPO, "Brian", "Oliván", "Oliván", 30, "ESP", 178, 72, LEFT, LB, LM_f, 76, 78, 1_200_000, 1_200_000, 15_000_000, 2, 15);
        AddSimple(SPO, "Guille", "Rosas", "Rosas", 24, "ESP", 174, 68, RIGHT, RB, RM_f, 74, 78, 1_200_000, 600_000, 10_000_000, 3, 6);
        AddSimple(SPO, "Kevin", "Vázquez", "K. Vázquez", 31, "ESP", 180, 72, RIGHT, RB, RM_f, 74, 76, 600_000, 800_000, 8_000_000, 2, 16);
        AddSimple(SPO, "Mamadou", "Loum", "Loum", 28, "SEN", 188, 80, RIGHT, CDM, CM_f, 74, 78, 1_200_000, 800_000, 10_000_000, 3, 7);
        AddSimple(SPO, "Nacho", "Martín", "Nacho Martín", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 72, 80, 800_000, 300_000, 10_000_000, 5, 8);
        AddSimple(SPO, "Manu", "Rodríguez", "Manu Rodríguez", 22, "ESP", 180, 72, RIGHT, CM, CAM_f, 70, 80, 600_000, 200_000, 8_000_000, 5, 17);
        AddSimple(SPO, "Àlex", "Corredera", "Corredera", 28, "ESP", 176, 70, RIGHT, CM, CDM_f, 74, 78, 800_000, 800_000, 10_000_000, 2, 18);
        AddSimple(SPO, "Jesús", "Bernal", "Bernal", 22, "ESP", 180, 72, RIGHT, CDM, CM_f, 72, 80, 800_000, 400_000, 10_000_000, 5, 19);
        AddSimple(SPO, "César", "Gelabert", "Gelabert", 24, "ESP", 178, 70, RIGHT, CAM, CM_f, 74, 80, 1_200_000, 600_000, 15_000_000, 3, 20);
        AddSimple(SPO, "Gaspar", "Campos", "Campos", 24, "ESP", 178, 72, RIGHT, LW, RW_f, 74, 80, 1_200_000, 600_000, 10_000_000, 3, 10);
        AddSimple(SPO, "Dani", "Queipo", "Queipo", 22, "ESP", 174, 68, RIGHT, LW, RW_f, 72, 80, 800_000, 300_000, 10_000_000, 5, 21);
        AddSimple(SPO, "Jonathan", "Dubasin", "Dubasin", 24, "BEL", 182, 74, RIGHT, RW, LW_f, 74, 80, 1_200_000, 600_000, 15_000_000, 3, 11);
        AddSimple(SPO, "Juan", "Otero", "Otero", 30, "ESP", 174, 70, RIGHT, RW, LW_f, 76, 78, 1_000_000, 1_200_000, 10_000_000, 3, 22);
        AddSimple(SPO, "Amadou", "Coundoul", "Coundoul", 22, "SEN", 182, 74, RIGHT, ST, LW_f, 72, 80, 1_000_000, 400_000, 10_000_000, 5, 23);

        // ══════════════════════════════════════════════════════════
        // REAL OVIEDO (TeamId=31)
        // ══════════════════════════════════════════════════════════
        int OVI = 31;
        AddSimple(OVI, "Aarón", "Escandell", "Escandell", 29, "ESP", 182, 76, RIGHT, GK, GK_f, 72, 76, 1_000_000, 500_000, 5_000_000, 2, 1);
        AddSimple(OVI, "Horațiu", "Moldovan", "Moldovan", 26, "ROU", 188, 82, RIGHT, GK, GK_f, 70, 74, 600_000, 350_000, 4_000_000, 2, 13);
        AddSimple(OVI, "David", "Carmo", "Carmo", 25, "PRT", 196, 84, RIGHT, CB, CB_f, 74, 80, 3_000_000, 800_000, 15_000_000, 3, 4);
        AddSimple(OVI, "Eric", "Bailly", "Bailly", 30, "CIV", 187, 80, RIGHT, CB, RB_f, 76, 78, 2_500_000, 1_200_000, 15_000_000, 2, 5);
        AddSimple(OVI, "David", "Costas", "Costas", 29, "ESP", 184, 76, RIGHT, CB, CB_f, 72, 76, 1_200_000, 600_000, 8_000_000, 2, 15);
        AddSimple(OVI, "Dani", "Calvo", "Calvo", 30, "ESP", 186, 78, RIGHT, CB, CB_f, 72, 74, 1_000_000, 500_000, 6_000_000, 2, 12);
        AddSimple(OVI, "Rahim", "Alhassane", "Alhassane", 23, "GIN", 178, 72, LEFT, LB, LM_f, 70, 78, 800_000, 300_000, 5_000_000, 4, 3);
        AddSimple(OVI, "Nacho", "Vidal", "Nacho Vidal", 29, "ESP", 180, 74, RIGHT, RB, RM_f, 74, 76, 1_500_000, 700_000, 8_000_000, 2, 2);
        AddSimple(OVI, "Lucas", "Ahijado", "Ahijado", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 68, 76, 500_000, 200_000, 4_000_000, 5, 22);
        AddSimple(OVI, "Leander", "Dendoncker", "Dendoncker", 29, "BEL", 188, 80, RIGHT, CDM, CM_f, 76, 78, 3_000_000, 1_200_000, 15_000_000, 2, 8);
        AddSimple(OVI, "Alberto", "Reina", "Reina", 22, "ESP", 180, 72, RIGHT, CM, CDM_f, 70, 80, 800_000, 300_000, 5_000_000, 5, 16);
        AddSimple(OVI, "Kwasi", "Sibo", "Sibo", 26, "GHA", 184, 76, RIGHT, CM, CDM_f, 72, 76, 1_000_000, 400_000, 6_000_000, 3, 6);
        AddSimple(OVI, "Santi", "Cazorla", "Cazorla", 40, "ESP", 168, 64, RIGHT, CAM, CM_f, 78, 78, 1_000_000, 2_000_000, 8_000_000, 1, 10);
        AddSimple(OVI, "Luka", "Ilic", "Ilic", 25, "SRB", 182, 74, RIGHT, CAM, CM_f, 72, 78, 1_200_000, 500_000, 8_000_000, 3, 20);
        AddSimple(OVI, "Pablo", "Agudín", "Agudín", 22, "ESP", 176, 68, LEFT, LW, RW_f, 72, 80, 800_000, 300_000, 5_000_000, 5, 17);
        AddSimple(OVI, "Ilyas", "Chaira", "Chaira", 24, "MAR", 178, 70, RIGHT, LW, RW_f, 70, 76, 600_000, 300_000, 4_000_000, 3, 19);
        AddSimple(OVI, "Brandon", "Dominguès", "Brandon", 27, "ESP", 178, 72, RIGHT, RW, LW_f, 74, 78, 1_200_000, 600_000, 8_000_000, 2, 7);
        AddSimple(OVI, "Ovie", "Ejaria", "Ejaria", 27, "GBR", 182, 74, RIGHT, RW, LW_f, 72, 76, 1_000_000, 500_000, 6_000_000, 2, 14);
        AddSimple(OVI, "Haissem", "Hassan", "Hassan", 22, "FRA", 176, 68, RIGHT, RW, LW_f, 74, 82, 2_000_000, 500_000, 10_000_000, 4, 11);
        AddSimple(OVI, "Federico", "Viñas", "Viñas", 26, "URY", 176, 72, RIGHT, ST, CF_f, 74, 78, 1_500_000, 700_000, 8_000_000, 3, 9);
        AddSimple(OVI, "Álex", "Forés", "Forés", 23, "ESP", 184, 76, RIGHT, ST, CF_f, 72, 78, 1_000_000, 400_000, 6_000_000, 4, 18);

        // ══════════════════════════════════════════════════════════
        // CD TENERIFE (TeamId=32)
        // ══════════════════════════════════════════════════════════
        int TFE = 32;
        AddSimple(TFE, "Dani", "Martín", "Dani Martín", 26, "ESP", 184, 78, RIGHT, GK, GK_f, 72, 78, 800_000, 400_000, 5_000_000, 3, 1);
        AddSimple(TFE, "Gabriel", "De Vuyst", "De Vuyst", 22, "BEL", 188, 80, RIGHT, GK, GK_f, 68, 76, 400_000, 200_000, 3_000_000, 4, 13);
        AddSimple(TFE, "Anthony", "Landázuri", "Landázuri", 27, "ECU", 186, 78, RIGHT, CB, CB_f, 72, 76, 1_000_000, 500_000, 6_000_000, 2, 4);
        AddSimple(TFE, "José", "León", "José León", 30, "ESP", 184, 76, RIGHT, CB, CB_f, 72, 74, 800_000, 500_000, 5_000_000, 2, 5);
        AddSimple(TFE, "Facundo", "Agüero", "Agüero", 22, "ARG", 178, 72, LEFT, LB, LM_f, 70, 80, 800_000, 300_000, 5_000_000, 5, 3);
        AddSimple(TFE, "Marc", "Mateu", "Mateu", 23, "ESP", 176, 68, RIGHT, RB, RM_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 2);
        AddSimple(TFE, "David", "Rodríguez", "David Rodríguez", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 22);
        AddSimple(TFE, "César", "Álvarez", "C. Álvarez", 22, "ESP", 178, 72, RIGHT, CDM, CM_f, 70, 80, 800_000, 300_000, 5_000_000, 5, 8);
        AddSimple(TFE, "Juanjo", "Sánchez", "Juanjo", 22, "ESP", 180, 72, RIGHT, CDM, CM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 16);
        AddSimple(TFE, "Fabricio", "", "Fabricio", 24, "URY", 176, 70, RIGHT, CDM, CM_f, 72, 78, 1_000_000, 400_000, 6_000_000, 3, 6);
        AddSimple(TFE, "Alberto", "Ulloa", "Ulloa", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 14);
        AddSimple(TFE, "Javi", "Pérez", "Javi Pérez", 24, "ESP", 176, 68, RIGHT, CM, CDM_f, 72, 78, 800_000, 400_000, 5_000_000, 3, 10);
        AddSimple(TFE, "Aitor", "Sanz", "Sanz", 36, "ESP", 178, 72, RIGHT, CM, CDM_f, 74, 74, 500_000, 1_000_000, 4_000_000, 1, 15);
        AddSimple(TFE, "Maikel", "Mesa", "Mesa", 27, "ESP", 176, 70, RIGHT, CM, CAM_f, 74, 78, 1_200_000, 600_000, 8_000_000, 3, 11);
        AddSimple(TFE, "Nacho", "Gil", "Nacho Gil", 29, "ESP", 174, 68, RIGHT, CAM, LW_f, 72, 76, 800_000, 500_000, 5_000_000, 2, 20);
        AddSimple(TFE, "Alassan", "Manjam", "Manjam", 23, "GMB", 178, 70, LEFT, LW, RW_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 18);
        AddSimple(TFE, "Jeremy", "Jorge", "Jorge", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 23);
        AddSimple(TFE, "Iván", "Chapela", "Chapela", 25, "ESP", 180, 72, RIGHT, RW, LW_f, 72, 78, 800_000, 400_000, 5_000_000, 3, 17);
        AddSimple(TFE, "Jesús", "de Miguel", "De Miguel", 22, "ESP", 182, 74, RIGHT, ST, CF_f, 72, 80, 1_000_000, 400_000, 6_000_000, 5, 9);
        AddSimple(TFE, "Gastón", "Valles", "Valles", 24, "URY", 180, 72, RIGHT, ST, CF_f, 72, 78, 800_000, 400_000, 5_000_000, 3, 12);
        AddSimple(TFE, "Mahamadou", "Baldé", "Baldé", 22, "SEN", 186, 78, RIGHT, ST, CF_f, 68, 76, 500_000, 200_000, 3_000_000, 4, 24);
        AddSimple(TFE, "Enric", "Gallego", "Gallego", 38, "ESP", 186, 80, RIGHT, ST, CF_f, 74, 74, 400_000, 1_000_000, 3_000_000, 1, 7);

        // ══════════════════════════════════════════════════════════
        // ALBACETE BALOMPIÉ (TeamId=33)
        // ══════════════════════════════════════════════════════════
        int ALB = 33;
        AddSimple(ALB, "Raúl", "Lizoain", "Lizoain", 33, "ESP", 186, 80, RIGHT, GK, GK_f, 72, 74, 500_000, 500_000, 3_000_000, 2, 1);
        AddSimple(ALB, "Diego", "Mariño", "Mariño", 34, "ESP", 186, 80, RIGHT, GK, GK_f, 72, 72, 300_000, 600_000, 2_000_000, 1, 13);
        AddSimple(ALB, "Jesús", "Vallejo", "Vallejo", 27, "ESP", 184, 76, RIGHT, CB, CB_f, 72, 76, 1_000_000, 500_000, 6_000_000, 2, 4);
        AddSimple(ALB, "Pepe", "Sánchez", "Pepe Sánchez", 24, "ESP", 182, 74, RIGHT, CB, CB_f, 70, 76, 600_000, 300_000, 4_000_000, 4, 5);
        AddSimple(ALB, "Lluís", "López", "Lluís López", 27, "ESP", 186, 78, RIGHT, CB, CB_f, 72, 76, 800_000, 400_000, 5_000_000, 2, 14);
        AddSimple(ALB, "Javi", "Moreno", "Javi Moreno", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 22);
        AddSimple(ALB, "Carlos", "Neva", "Neva", 28, "ESP", 176, 70, LEFT, LB, LM_f, 74, 78, 1_200_000, 600_000, 8_000_000, 3, 3);
        AddSimple(ALB, "Jonathan", "Gómez", "J. Gómez", 22, "USA", 180, 72, RIGHT, RB, RM_f, 70, 80, 800_000, 300_000, 5_000_000, 4, 2);
        AddSimple(ALB, "Lorenzo", "Aguado", "Aguado", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 16);
        AddSimple(ALB, "Fran", "Gámez", "Fran Gámez", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 20);
        AddSimple(ALB, "Martín", "Fernández", "Martín F.", 22, "ESP", 176, 68, RIGHT, CDM, CM_f, 70, 78, 600_000, 200_000, 4_000_000, 5, 6);
        AddSimple(ALB, "Agus", "Medina", "Medina", 30, "ESP", 180, 72, RIGHT, CDM, CM_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 8);
        AddSimple(ALB, "Antonio", "Pacheco", "Pacheco", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 10);
        AddSimple(ALB, "Javi", "Villar", "Javi Villar", 22, "ESP", 180, 72, RIGHT, CM, CDM_f, 70, 80, 800_000, 300_000, 5_000_000, 5, 18);
        AddSimple(ALB, "Ale", "Meléndez", "Meléndez", 25, "ESP", 176, 68, RIGHT, CAM, LW_f, 72, 78, 800_000, 400_000, 5_000_000, 3, 7);
        AddSimple(ALB, "Capi", "", "Capi", 22, "ESP", 174, 66, RIGHT, CAM, RW_f, 70, 80, 800_000, 300_000, 5_000_000, 5, 17);
        AddSimple(ALB, "Víctor", "San Bartolomé", "V. San Bartolomé", 22, "ESP", 178, 72, RIGHT, CAM, LW_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 21);
        AddSimple(ALB, "José Carlos", "Lazo", "Lazo", 28, "ESP", 176, 68, LEFT, LW, RW_f, 74, 78, 1_200_000, 600_000, 8_000_000, 2, 11);
        AddSimple(ALB, "Antonio", "Puertas", "Puertas", 32, "ESP", 178, 72, RIGHT, LW, RW_f, 74, 76, 800_000, 600_000, 5_000_000, 2, 9);
        AddSimple(ALB, "Jefté", "Betancor", "Betancor", 31, "ESP", 182, 74, RIGHT, RW, LW_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 15);
        AddSimple(ALB, "Samuel", "Obeng", "Obeng", 27, "GHA", 186, 80, RIGHT, ST, CF_f, 72, 76, 1_000_000, 500_000, 6_000_000, 3, 12);
        AddSimple(ALB, "Higinio", "Marín", "Higinio", 31, "ESP", 182, 76, RIGHT, ST, CF_f, 74, 76, 1_000_000, 600_000, 6_000_000, 2, 19);

        // ══════════════════════════════════════════════════════════
        // BURGOS CF (TeamId=34)
        // ══════════════════════════════════════════════════════════
        int BUR = 34;
        AddSimple(BUR, "Ander", "Cantero", "Cantero", 30, "ESP", 186, 80, RIGHT, GK, GK_f, 72, 76, 600_000, 400_000, 4_000_000, 2, 1);
        AddSimple(BUR, "Jesús", "Ruiz", "Jesús Ruiz", 27, "ESP", 184, 78, RIGHT, GK, GK_f, 68, 74, 300_000, 250_000, 2_000_000, 3, 13);
        AddSimple(BUR, "Oier", "Luengo", "Luengo", 27, "ESP", 186, 78, RIGHT, CB, CB_f, 72, 76, 800_000, 400_000, 5_000_000, 3, 4);
        AddSimple(BUR, "Sergio", "González", "S. González", 32, "ESP", 184, 76, RIGHT, CB, CB_f, 72, 74, 600_000, 500_000, 4_000_000, 2, 5);
        AddSimple(BUR, "Grego", "Sierra", "Grego", 31, "ESP", 186, 78, RIGHT, CB, CB_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 14);
        AddSimple(BUR, "Florian", "Miguel", "Miguel", 28, "FRA", 178, 72, LEFT, LB, CB_f, 72, 76, 800_000, 400_000, 5_000_000, 2, 3);
        AddSimple(BUR, "Brais", "Martínez", "B. Martínez", 22, "ESP", 176, 68, LEFT, LB, LM_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 15);
        AddSimple(BUR, "Álex", "Lizancos", "Lizancos", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 68, 76, 300_000, 200_000, 2_000_000, 5, 22);
        AddSimple(BUR, "Aitor", "Buñuel", "Buñuel", 26, "ESP", 174, 68, RIGHT, RB, RM_f, 72, 76, 800_000, 400_000, 5_000_000, 3, 2);
        AddSimple(BUR, "David", "González", "David González", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 16);
        AddSimple(BUR, "Miguel", "Atienza", "Atienza", 25, "ESP", 180, 72, RIGHT, CDM, CM_f, 72, 78, 1_000_000, 400_000, 6_000_000, 3, 6);
        AddSimple(BUR, "Iván", "Morante", "Morante", 22, "ESP", 178, 72, RIGHT, CDM, CM_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 18);
        AddSimple(BUR, "Pablo", "Galdames", "Galdames", 27, "CHL", 176, 70, RIGHT, CM, CAM_f, 72, 76, 800_000, 400_000, 5_000_000, 2, 8);
        AddSimple(BUR, "Kévin", "Appin", "Appin", 26, "FRA", 182, 74, RIGHT, CM, CDM_f, 70, 76, 600_000, 300_000, 4_000_000, 3, 10);
        AddSimple(BUR, "Saúl", "del Cerro", "Del Cerro", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 20);
        AddSimple(BUR, "Mario", "Cantero", "M. Cantero", 22, "ESP", 176, 68, RIGHT, CAM, LW_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 19);
        AddSimple(BUR, "Marcelo", "Expósito", "Expósito", 22, "ESP", 180, 72, RIGHT, CAM, CM_f, 70, 78, 600_000, 200_000, 4_000_000, 5, 21);
        AddSimple(BUR, "Iñigo", "Córdoba", "I. Córdoba", 27, "ESP", 178, 70, LEFT, LW, RW_f, 74, 78, 1_200_000, 600_000, 8_000_000, 2, 7);
        AddSimple(BUR, "Curro", "Sánchez", "Curro", 28, "ESP", 176, 68, RIGHT, RW, LW_f, 72, 76, 800_000, 500_000, 5_000_000, 2, 11);
        AddSimple(BUR, "Víctor", "Mollejo", "Mollejo", 23, "ESP", 178, 72, RIGHT, RW, LW_f, 72, 78, 1_000_000, 400_000, 6_000_000, 4, 17);
        AddSimple(BUR, "Fermín", "García", "Fermín", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 23);
        AddSimple(BUR, "Fer", "Niño", "Fer Niño", 26, "ESP", 186, 78, RIGHT, ST, CF_f, 72, 78, 1_200_000, 500_000, 8_000_000, 2, 9);
        AddSimple(BUR, "Mario", "González", "Mario G.", 22, "ESP", 182, 74, RIGHT, ST, CF_f, 70, 78, 600_000, 300_000, 4_000_000, 5, 12);
        AddSimple(BUR, "Mateo", "Mejía", "Mejía", 22, "ESP", 178, 72, RIGHT, ST, CF_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 24);

        // ══════════════════════════════════════════════════════════
        // CÓRDOBA CF (TeamId=35)
        // ══════════════════════════════════════════════════════════
        int COR = 35;
        AddSimple(COR, "Iker", "Álvarez", "I. Álvarez", 23, "ESP", 184, 76, RIGHT, GK, GK_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 1);
        AddSimple(COR, "Álex", "Martín", "Álex Martín", 22, "ESP", 186, 78, RIGHT, GK, GK_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 13);
        AddSimple(COR, "Franck", "Fomeyem", "Fomeyem", 24, "CMR", 186, 78, RIGHT, CB, CB_f, 70, 76, 600_000, 300_000, 4_000_000, 3, 4);
        AddSimple(COR, "Xavi", "Sintes", "Sintes", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 15);
        AddSimple(COR, "Rubén", "Alves", "Alves", 22, "ESP", 184, 76, RIGHT, CB, CB_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 5);
        AddSimple(COR, "Ignasi", "Vilarrasa", "Vilarrasa", 25, "ESP", 178, 72, LEFT, LB, LM_f, 72, 78, 800_000, 400_000, 5_000_000, 3, 3);
        AddSimple(COR, "Diego", "Bri", "Bri", 22, "ESP", 176, 68, LEFT, LB, LM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 16);
        AddSimple(COR, "Juan María", "Alcedo", "Alcedo", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 2);
        AddSimple(COR, "Carlos", "Albarrán", "Albarrán", 31, "ESP", 176, 70, RIGHT, RB, RM_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 17);
        AddSimple(COR, "Trilli", "", "Trilli", 23, "ESP", 174, 68, RIGHT, RB, RM_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 20);
        AddSimple(COR, "Isma", "Ruiz", "Isma Ruiz", 23, "ESP", 178, 72, RIGHT, CDM, CM_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 6);
        AddSimple(COR, "Théo", "Zidane", "Théo Zidane", 22, "FRA", 182, 74, RIGHT, CM, CAM_f, 70, 80, 1_000_000, 400_000, 6_000_000, 4, 8);
        AddSimple(COR, "Pedro", "Ortiz", "Pedro Ortiz", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 10);
        AddSimple(COR, "Mikel", "Goti", "Goti", 23, "ESP", 180, 72, RIGHT, CM, CAM_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 14);
        AddSimple(COR, "Dalisson", "de Almeida", "Dalisson", 24, "BRA", 176, 70, RIGHT, CAM, LW_f, 70, 78, 800_000, 300_000, 5_000_000, 3, 7);
        AddSimple(COR, "Jacobo", "González", "Jacobo", 22, "ESP", 174, 66, RIGHT, CAM, RW_f, 70, 80, 800_000, 300_000, 5_000_000, 5, 21);
        AddSimple(COR, "Kevin", "Medina", "K. Medina", 22, "ESP", 176, 68, LEFT, LW, RW_f, 70, 78, 600_000, 300_000, 4_000_000, 5, 11);
        AddSimple(COR, "Adilson", "", "Adilson", 24, "CPV", 178, 72, RIGHT, RW, LW_f, 70, 76, 500_000, 250_000, 3_000_000, 3, 18);
        AddSimple(COR, "Cristian", "Carracedo", "Carracedo", 27, "ESP", 178, 70, RIGHT, RW, LW_f, 72, 76, 800_000, 400_000, 5_000_000, 2, 19);
        AddSimple(COR, "Adrián", "Fuentes", "Fuentes", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 70, 78, 600_000, 300_000, 4_000_000, 5, 9);
        AddSimple(COR, "Nikolay", "Obolskiy", "Obolskiy", 24, "RUS", 184, 76, RIGHT, ST, CF_f, 70, 76, 600_000, 300_000, 4_000_000, 3, 12);
        AddSimple(COR, "Sergi", "Guardiola", "Guardiola", 33, "ESP", 182, 76, RIGHT, ST, CF_f, 74, 74, 500_000, 600_000, 4_000_000, 2, 23);
        AddSimple(COR, "Diego", "Percan", "Percan", 23, "HRV", 180, 72, RIGHT, ST, CF_f, 70, 78, 600_000, 300_000, 4_000_000, 4, 22);

        // ══════════════════════════════════════════════════════════
        // CD CASTELLÓN (TeamId=36)
        // ══════════════════════════════════════════════════════════
        int CAS = 36;
        AddSimple(CAS, "Romain", "Matthys", "Matthys", 26, "BEL", 188, 80, RIGHT, GK, GK_f, 70, 76, 500_000, 300_000, 3_000_000, 3, 1);
        AddSimple(CAS, "Amir", "Abedzadeh", "Abedzadeh", 31, "IRN", 186, 80, RIGHT, GK, GK_f, 70, 72, 300_000, 400_000, 2_000_000, 2, 13);
        AddSimple(CAS, "Fabrizio", "Brignani", "Brignani", 23, "ITA", 186, 78, RIGHT, CB, CB_f, 70, 78, 800_000, 300_000, 5_000_000, 4, 4);
        AddSimple(CAS, "Agustín", "Sienra", "Sienra", 22, "URY", 184, 76, RIGHT, CB, CB_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 14);
        AddSimple(CAS, "Alberto", "Jiménez", "A. Jiménez", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 5);
        AddSimple(CAS, "Lucas", "Alcázar", "Alcázar", 22, "ESP", 178, 72, LEFT, LB, LM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 15);
        AddSimple(CAS, "Tincho", "Conde", "Conde", 22, "ESP", 176, 68, LEFT, LB, LM_f, 68, 76, 300_000, 200_000, 2_000_000, 5, 22);
        AddSimple(CAS, "Salva", "Ruiz", "Salva Ruiz", 29, "ESP", 178, 70, LEFT, LB, LM_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 3);
        AddSimple(CAS, "Jérémy", "Mellot", "Mellot", 30, "FRA", 178, 72, RIGHT, RB, RM_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 2);
        AddSimple(CAS, "Ronaldo", "", "Ronaldo", 24, "BRA", 180, 72, RIGHT, RB, RM_f, 68, 76, 500_000, 250_000, 3_000_000, 3, 16);
        AddSimple(CAS, "Diego", "Barri", "Barri", 22, "ESP", 178, 72, RIGHT, CDM, CM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 6);
        AddSimple(CAS, "Marc-Olivier", "Doué", "M. Doué", 22, "FRA", 180, 72, RIGHT, CM, CAM_f, 70, 80, 1_000_000, 400_000, 6_000_000, 4, 8);
        AddSimple(CAS, "Álex", "Calatrava", "Calatrava", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 10);
        AddSimple(CAS, "Israel", "Suero", "Suero", 22, "ESP", 174, 66, RIGHT, CM, CDM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 18);
        AddSimple(CAS, "Brian", "Cipenga", "Cipenga", 24, "AGO", 176, 70, RIGHT, CAM, LW_f, 70, 76, 500_000, 250_000, 3_000_000, 3, 20);
        AddSimple(CAS, "Raúl", "Sánchez", "Raúl Sánchez", 22, "ESP", 180, 72, RIGHT, CAM, LW_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 21);
        AddSimple(CAS, "Pablo", "Santiago", "Santiago", 22, "ESP", 174, 66, RIGHT, LW, RW_f, 70, 78, 500_000, 200_000, 4_000_000, 5, 11);
        AddSimple(CAS, "Awer", "Mabil", "Mabil", 29, "AUS", 178, 70, RIGHT, RW, LW_f, 72, 74, 600_000, 400_000, 4_000_000, 2, 7);
        AddSimple(CAS, "Douglas", "Aurélio", "Aurélio", 22, "BRA", 176, 68, RIGHT, RW, LW_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 17);
        AddSimple(CAS, "Ousmane", "Camara", "O. Camara", 22, "SEN", 182, 74, RIGHT, ST, CF_f, 70, 78, 600_000, 300_000, 4_000_000, 5, 9);
        AddSimple(CAS, "Adam", "Jakobsen", "Jakobsen", 25, "DNK", 186, 78, RIGHT, ST, CF_f, 70, 76, 600_000, 300_000, 4_000_000, 3, 12);
        AddSimple(CAS, "Álvaro", "García", "Álvaro García", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 19);

        // ══════════════════════════════════════════════════════════
        // FC ANDORRA (TeamId=37)
        // ══════════════════════════════════════════════════════════
        int AND = 37;
        AddSimple(AND, "Áron", "Yaakobishvili", "Yaakobishvili", 18, "HUN", 188, 80, RIGHT, GK, GK_f, 66, 82, 1_000_000, 200_000, 6_000_000, 5, 1);
        AddSimple(AND, "Jesús", "Owono", "Owono", 23, "GNQ", 184, 76, RIGHT, GK, GK_f, 68, 76, 400_000, 200_000, 3_000_000, 4, 13);
        AddSimple(AND, "Nico", "Ratti", "Ratti", 22, "FRA", 186, 78, RIGHT, GK, GK_f, 66, 74, 300_000, 150_000, 2_000_000, 4, 25);
        AddSimple(AND, "Gael", "Alonso", "Gael Alonso", 22, "ESP", 184, 76, RIGHT, CB, CB_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 4);
        AddSimple(AND, "Edgar", "González", "Edgar G.", 27, "ESP", 186, 78, RIGHT, CB, CDM_f, 72, 76, 800_000, 400_000, 5_000_000, 2, 5);
        AddSimple(AND, "Diego", "Alende", "Alende", 27, "ESP", 184, 76, RIGHT, CB, CB_f, 70, 74, 500_000, 300_000, 3_000_000, 2, 14);
        AddSimple(AND, "Imanol", "García de Albéniz", "Imanol", 24, "ESP", 178, 72, LEFT, LB, LM_f, 72, 78, 800_000, 300_000, 5_000_000, 4, 3);
        AddSimple(AND, "Martí", "Vilà", "Martí Vilà", 22, "ESP", 176, 68, LEFT, LB, LM_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 22);
        AddSimple(AND, "Thomas", "Carrique", "Carrique", 24, "FRA", 180, 72, RIGHT, RB, RM_f, 68, 76, 400_000, 200_000, 3_000_000, 3, 2);
        AddSimple(AND, "Álex", "Petxarroman", "Petxarroman", 27, "ESP", 178, 72, RIGHT, RB, RM_f, 72, 76, 600_000, 400_000, 4_000_000, 2, 16);
        AddSimple(AND, "Efe", "Akman", "Akman", 22, "TUR", 180, 72, RIGHT, CDM, CM_f, 70, 80, 800_000, 300_000, 5_000_000, 4, 6);
        AddSimple(AND, "Dani", "Villahermosa", "Villahermosa", 23, "ESP", 178, 72, RIGHT, CDM, CM_f, 68, 76, 400_000, 200_000, 3_000_000, 4, 8);
        AddSimple(AND, "Marc", "Doménech", "M. Doménech", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 10);
        AddSimple(AND, "Théo", "Le Normand", "T. Le Normand", 22, "FRA", 180, 72, RIGHT, CM, CDM_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 18);
        AddSimple(AND, "Min-su", "Kim", "Kim Min-su", 22, "KOR", 178, 70, RIGHT, CM, CAM_f, 68, 78, 600_000, 200_000, 4_000_000, 4, 20);
        AddSimple(AND, "Jastin", "Garcia", "Jastin", 22, "HND", 176, 68, RIGHT, LW, RW_f, 70, 78, 600_000, 200_000, 4_000_000, 4, 11);
        AddSimple(AND, "Álvaro", "Martín", "Álvaro Martín", 22, "ESP", 174, 66, RIGHT, LW, RW_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 17);
        AddSimple(AND, "Aingeru", "Olabarrieta", "Olabarrieta", 22, "ESP", 180, 72, RIGHT, RW, LW_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 21);
        AddSimple(AND, "Yeray", "Cabanzón", "Cabanzón", 22, "ESP", 178, 72, RIGHT, RW, LW_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 19);
        AddSimple(AND, "Álex", "Calvo", "Álex Calvo", 22, "ESP", 176, 68, RIGHT, ST, CF_f, 68, 78, 500_000, 200_000, 4_000_000, 5, 9);
        AddSimple(AND, "Lautaro", "de León", "De León", 22, "URY", 180, 72, RIGHT, ST, CF_f, 68, 76, 400_000, 200_000, 3_000_000, 5, 12);
        AddSimple(AND, "Marc", "Cardona", "Cardona", 29, "ESP", 182, 74, RIGHT, ST, CF_f, 72, 76, 800_000, 500_000, 5_000_000, 2, 7);

        // ══════════════════════════════════════════════════════════
        // CD ELDENSE (TeamId=38)
        // ══════════════════════════════════════════════════════════
        int ELD = 38;
        AddSimple(ELD, "Ramón", "Vila", "Ramón Vila", 22, "ESP", 184, 76, RIGHT, GK, GK_f, 66, 74, 300_000, 150_000, 2_000_000, 5, 1);
        AddSimple(ELD, "Pablo", "Valencia", "P. Valencia", 22, "ESP", 186, 78, RIGHT, GK, GK_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 13);
        AddSimple(ELD, "Floris", "Smand", "Smand", 22, "NLD", 186, 78, RIGHT, CB, CB_f, 68, 76, 400_000, 200_000, 3_000_000, 4, 4);
        AddSimple(ELD, "Álex", "Serradell", "Serradell", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 66, 74, 300_000, 150_000, 2_000_000, 5, 5);
        AddSimple(ELD, "Arnau", "Gaixas", "Gaixas", 22, "ESP", 184, 76, RIGHT, CB, CB_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 14);
        AddSimple(ELD, "Dario", "Dumic", "Dumic", 32, "SRB", 186, 80, RIGHT, CB, CB_f, 70, 72, 300_000, 300_000, 2_000_000, 2, 15);
        AddSimple(ELD, "Óscar", "Vega", "Ó. Vega", 22, "ESP", 176, 68, LEFT, LB, LM_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 3);
        AddSimple(ELD, "Jesús", "Clemente", "Clemente", 22, "ESP", 178, 72, LEFT, LB, LM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 16);
        AddSimple(ELD, "Guillermo", "Macho", "Macho", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 2);
        AddSimple(ELD, "Borja", "Calvo", "Borja Calvo", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 17);
        AddSimple(ELD, "Rubén", "Quintanilla", "Quintanilla", 22, "ESP", 180, 72, RIGHT, CDM, CM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 6);
        AddSimple(ELD, "Manu", "Molina", "Manu Molina", 32, "ESP", 176, 70, RIGHT, CM, CDM_f, 72, 74, 500_000, 400_000, 3_000_000, 2, 8);
        AddSimple(ELD, "Nacho", "Quintana", "Quintana", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 10);
        AddSimple(ELD, "Marcos", "Bustillo", "Bustillo", 22, "ESP", 176, 68, RIGHT, CM, CDM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 18);
        AddSimple(ELD, "Boston", "Billups", "Billups", 22, "USA", 180, 72, RIGHT, CAM, CM_f, 68, 76, 400_000, 200_000, 3_000_000, 4, 20);
        AddSimple(ELD, "Rafa", "Núñez", "Rafa Núñez", 22, "ESP", 174, 66, RIGHT, CAM, LW_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 21);
        AddSimple(ELD, "Fidel", "", "Fidel", 34, "ESP", 180, 72, RIGHT, LW, RW_f, 74, 74, 400_000, 800_000, 3_000_000, 1, 7);
        AddSimple(ELD, "Hamza", "Bellari", "Bellari", 22, "MAR", 178, 70, RIGHT, LW, RW_f, 66, 74, 300_000, 150_000, 2_000_000, 5, 11);
        AddSimple(ELD, "Alejandro", "Ibarrondo", "Ibarrondo", 22, "ESP", 180, 72, RIGHT, RW, LW_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 19);
        AddSimple(ELD, "Rober", "Ibáñez", "Rober Ibáñez", 31, "ESP", 174, 68, RIGHT, RW, LW_f, 72, 74, 500_000, 400_000, 3_000_000, 2, 12);
        AddSimple(ELD, "Hugo", "Alba", "Hugo Alba", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 68, 76, 300_000, 150_000, 2_000_000, 5, 9);
        AddSimple(ELD, "Israel", "Salazar", "Salazar", 22, "ESP", 182, 74, RIGHT, ST, CF_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 22);
        AddSimple(ELD, "Dioni", "Villalba", "Dioni", 34, "ESP", 178, 72, RIGHT, ST, CF_f, 72, 72, 300_000, 500_000, 2_000_000, 1, 23);

        // ══════════════════════════════════════════════════════════
        // CE SABADELL FC (TeamId=39)
        // ══════════════════════════════════════════════════════════
        int SAB = 39;
        AddSimple(SAB, "Diego", "Fuoli", "Fuoli", 27, "ESP", 186, 78, RIGHT, GK, GK_f, 68, 76, 400_000, 250_000, 2_000_000, 3, 1);
        AddSimple(SAB, "Nil", "Ruiz", "Nil Ruiz", 22, "ESP", 184, 76, RIGHT, GK, GK_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 13);
        AddSimple(SAB, "José", "Ortega", "José Ortega", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 4);
        AddSimple(SAB, "Arthur", "Bonaldo", "Bonaldo", 22, "BRA", 184, 76, RIGHT, CB, CB_f, 66, 74, 300_000, 150_000, 2_000_000, 4, 5);
        AddSimple(SAB, "Carlos", "García", "Carlos García", 22, "ESP", 178, 72, RIGHT, CB, CB_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 14);
        AddSimple(SAB, "Kaiser", "", "Kaiser", 22, "BRA", 186, 78, RIGHT, CB, CB_f, 66, 74, 300_000, 150_000, 2_000_000, 4, 15);
        AddSimple(SAB, "Genar", "Fornés", "Fornés", 22, "ESP", 178, 72, LEFT, LB, LM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 3);
        AddSimple(SAB, "Sergi", "Segura", "Segura", 22, "ESP", 176, 68, LEFT, LB, LM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 16);
        AddSimple(SAB, "David", "Astals", "Astals", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 2);
        AddSimple(SAB, "Carlos", "Alemán", "Alemán", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 17);
        AddSimple(SAB, "Quadri", "Liameed", "Liameed", 22, "NGA", 182, 74, RIGHT, CDM, CM_f, 66, 74, 300_000, 150_000, 2_000_000, 4, 6);
        AddSimple(SAB, "Albert", "Orriols", "Orriols", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 8);
        AddSimple(SAB, "Eneko", "Aguilar", "E. Aguilar", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 10);
        AddSimple(SAB, "Javi", "López", "Javi López", 22, "ESP", 174, 66, RIGHT, CM, CAM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 18);
        AddSimple(SAB, "Joel", "Priego", "Priego", 22, "ESP", 176, 68, RIGHT, CAM, LW_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 20);
        AddSimple(SAB, "Xavi", "Moreno", "Xavi Moreno", 22, "ESP", 174, 66, RIGHT, CAM, RW_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 21);
        AddSimple(SAB, "Ton", "Ripoll", "Ripoll", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 11);
        AddSimple(SAB, "Rubén", "Martínez", "Rubén M.", 22, "ESP", 178, 72, RIGHT, LW, RW_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 7);
        AddSimple(SAB, "Miguelete", "", "Miguelete", 22, "ESP", 180, 72, RIGHT, RW, LW_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 19);
        AddSimple(SAB, "Alan", "Godoy", "Godoy", 22, "ESP", 182, 74, RIGHT, ST, CF_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 9);
        AddSimple(SAB, "Agustín", "Coscia", "Coscia", 22, "ARG", 178, 72, RIGHT, ST, CF_f, 66, 74, 300_000, 150_000, 2_000_000, 4, 12);
        AddSimple(SAB, "Rodrigo", "Escudero", "Escudero", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 22);

        // ══════════════════════════════════════════════════════════
        // AD CEUTA FC (TeamId=40)
        // ══════════════════════════════════════════════════════════
        int CEU = 40;
        AddSimple(CEU, "Guillermo", "Vallejo", "Vallejo", 22, "ESP", 184, 76, RIGHT, GK, GK_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 1);
        AddSimple(CEU, "Pedro", "López", "Pedro López", 22, "ESP", 186, 78, RIGHT, GK, GK_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 13);
        AddSimple(CEU, "Diego", "González", "Diego G.", 22, "ESP", 182, 74, RIGHT, CB, CB_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 4);
        AddSimple(CEU, "Yago", "Cantero", "Y. Cantero", 22, "ESP", 184, 76, RIGHT, CB, CB_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 5);
        AddSimple(CEU, "Albert", "Caparrós", "Caparrós", 22, "ESP", 178, 72, RIGHT, CB, CDM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 14);
        AddSimple(CEU, "Carlos", "Hernández", "C. Hernández", 34, "ESP", 186, 80, RIGHT, CB, CB_f, 72, 72, 250_000, 400_000, 2_000_000, 1, 15);
        AddSimple(CEU, "José", "Matos", "Matos", 22, "ESP", 176, 68, LEFT, LB, LM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 3);
        AddSimple(CEU, "Carlos", "Redru", "Redru", 22, "ESP", 178, 72, LEFT, LB, LM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 16);
        AddSimple(CEU, "Gonzalo", "Almenara", "Almenara", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 2);
        AddSimple(CEU, "Manu", "Sánchez", "Manu Sánchez", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 17);
        AddSimple(CEU, "Youness", "Lachhab", "Lachhab", 22, "MAR", 180, 72, RIGHT, CDM, CM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 6);
        AddSimple(CEU, "Yann", "Bodiger", "Bodiger", 29, "FRA", 186, 78, RIGHT, CDM, CM_f, 70, 74, 500_000, 300_000, 3_000_000, 2, 8);
        AddSimple(CEU, "Cristian", "Rodríguez", "Cristian R.", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 10);
        AddSimple(CEU, "José", "Campaña", "Campaña", 32, "ESP", 180, 72, RIGHT, CM, CAM_f, 74, 76, 800_000, 600_000, 5_000_000, 2, 11);
        AddSimple(CEU, "Josema", "López", "Josema", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 18);
        AddSimple(CEU, "Arick", "Betancourt", "Betancourt", 22, "ESP", 180, 72, RIGHT, CAM, LW_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 20);
        AddSimple(CEU, "Kuki", "Zalazar", "Zalazar", 26, "URY", 174, 68, RIGHT, CAM, LW_f, 70, 76, 500_000, 300_000, 3_000_000, 3, 7);
        AddSimple(CEU, "Rubén", "Díez", "Rubén Díez", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 21);
        AddSimple(CEU, "Kialy Abdoul", "Kone", "Kone", 22, "CIV", 182, 74, RIGHT, LW, RW_f, 66, 74, 300_000, 150_000, 2_000_000, 4, 19);
        AddSimple(CEU, "Anuar", "", "Anuar", 29, "ESP", 176, 70, RIGHT, RW, LW_f, 72, 76, 600_000, 400_000, 4_000_000, 2, 12);
        AddSimple(CEU, "Ignacio", "Schor", "Schor", 22, "ARG", 178, 72, RIGHT, RW, LW_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 22);
        AddSimple(CEU, "Salvi", "Sánchez", "Salvi", 33, "ESP", 176, 68, RIGHT, RW, LW_f, 72, 72, 300_000, 400_000, 2_000_000, 1, 23);
        AddSimple(CEU, "Marcos", "Fernández", "Marcos F.", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 66, 74, 250_000, 150_000, 2_000_000, 5, 9);
        AddSimple(CEU, "Marc", "Domènech", "Domènech", 22, "ESP", 182, 74, RIGHT, ST, CF_f, 64, 72, 200_000, 100_000, 1_500_000, 5, 24);

        // ══════════════════════════════════════════════════════════
        // REAL SOCIEDAD B (TeamId=41)
        // ══════════════════════════════════════════════════════════
        int RSB = 41;
        AddSimple(RSB, "Aitor", "Fraga", "Fraga", 22, "ESP", 184, 76, RIGHT, GK, GK_f, 66, 76, 300_000, 150_000, 2_000_000, 5, 1);
        AddSimple(RSB, "Egoitz", "Arana", "Arana", 22, "ESP", 186, 78, RIGHT, GK, GK_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 13);
        AddSimple(RSB, "Kazunari", "Kita", "Kita", 22, "JPN", 180, 72, RIGHT, GK, GK_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 25);
        AddSimple(RSB, "Luken", "Beitia", "Beitia", 22, "ESP", 184, 76, RIGHT, CB, CB_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 4);
        AddSimple(RSB, "Unax", "Ayo", "Ayo", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 5);
        AddSimple(RSB, "Eneko", "Astigarraga", "Astigarraga", 22, "ESP", 178, 72, RIGHT, CB, CB_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 14);
        AddSimple(RSB, "Iker", "Calderon", "Calderon", 22, "ESP", 176, 68, LEFT, LB, LM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 3);
        AddSimple(RSB, "Jon", "Balda", "Balda", 22, "ESP", 178, 72, LEFT, LB, LM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 15);
        AddSimple(RSB, "Unax", "Agote", "Agote", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 2);
        AddSimple(RSB, "Jon", "Garro", "Garro", 22, "ESP", 178, 72, RIGHT, RB, RM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 16);
        AddSimple(RSB, "Alberto", "Dadie", "Dadie", 22, "ESP", 180, 72, RIGHT, CDM, CM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 6);
        AddSimple(RSB, "Iñaki", "Rupérez", "Rupérez", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 8);
        AddSimple(RSB, "Tomy", "Carbonell", "Carbonell", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 10);
        AddSimple(RSB, "Gorka", "Gorosabel", "Gorosabel", 22, "ESP", 178, 72, RIGHT, CM, CDM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 18);
        AddSimple(RSB, "Ibai", "Aguirre", "I. Aguirre", 22, "ESP", 174, 66, RIGHT, CAM, LW_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 20);
        AddSimple(RSB, "Mikel", "Rodriguez", "M. Rodriguez", 22, "ESP", 176, 68, RIGHT, CAM, RW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 21);
        AddSimple(RSB, "Alex", "Lebarbier", "Lebarbier", 22, "FRA", 178, 72, RIGHT, LW, RW_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 11);
        AddSimple(RSB, "Alex", "Marchal", "Marchal", 22, "FRA", 180, 72, RIGHT, LW, RW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 7);
        AddSimple(RSB, "Arkaitz", "Mariezkurrena", "Mariezkurrena", 22, "ESP", 176, 68, RIGHT, RW, LW_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 19);
        AddSimple(RSB, "Darío", "Ramírez", "Darío R.", 22, "ESP", 174, 66, RIGHT, RW, LW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 22);
        AddSimple(RSB, "Lander", "Astiazaran", "Astiazaran", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 9);
        AddSimple(RSB, "Dani", "Díaz", "Dani Díaz", 22, "ESP", 178, 72, RIGHT, ST, CF_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 12);
        AddSimple(RSB, "Job", "Ochieng", "Ochieng", 22, "KEN", 182, 74, RIGHT, ST, CF_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 23);
        AddSimple(RSB, "Gorka", "Carrera", "Carrera", 22, "ESP", 176, 68, RIGHT, ST, LW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 24);
        AddSimple(RSB, "Ekain", "Orobengoa", "Orobengoa", 22, "ESP", 178, 72, RIGHT, ST, CF_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 26);

        // ══════════════════════════════════════════════════════════
        // RC CELTA FORTUNA (TeamId=42)
        // ══════════════════════════════════════════════════════════
        int FOR = 42;
        AddSimple(FOR, "Coke", "Carrillo", "Carrillo", 22, "ESP", 184, 76, RIGHT, GK, GK_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 1);
        AddSimple(FOR, "Caio", "Barone", "Barone", 22, "BRA", 186, 78, RIGHT, GK, GK_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 13);
        AddSimple(FOR, "Marcos", "González", "Marcos G.", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 4);
        AddSimple(FOR, "Anxo", "Rodríguez", "Anxo", 22, "ESP", 178, 72, RIGHT, CB, CB_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 5);
        AddSimple(FOR, "Pablo", "Meixús", "Meixús", 22, "ESP", 180, 72, RIGHT, CB, CB_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 14);
        AddSimple(FOR, "Kike", "Ribes", "Ribes", 22, "ESP", 176, 68, LEFT, LB, LM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 3);
        AddSimple(FOR, "Seyni Mbaye", "Ndiaye", "Ndiaye", 22, "SEN", 182, 74, LEFT, LB, LM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 15);
        AddSimple(FOR, "Anthony", "Khayat", "Khayat", 22, "FRA", 178, 72, RIGHT, RB, RM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 2);
        AddSimple(FOR, "Joel", "López", "Joel López", 22, "ESP", 176, 68, RIGHT, RB, RM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 16);
        AddSimple(FOR, "Jan", "Oliveras", "Oliveras", 22, "ESP", 180, 72, RIGHT, RB, RM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 17);
        AddSimple(FOR, "Pablo", "Gavián", "Gavián", 22, "ESP", 178, 72, RIGHT, CDM, CM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 6);
        AddSimple(FOR, "Hugo", "Burcio", "Burcio", 22, "ESP", 176, 68, RIGHT, CM, CAM_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 8);
        AddSimple(FOR, "Andrés", "Antañón", "Antañón", 22, "ESP", 180, 72, RIGHT, CM, CDM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 10);
        AddSimple(FOR, "David", "de la Iglesia", "De la Iglesia", 22, "ESP", 178, 72, RIGHT, CM, CAM_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 18);
        AddSimple(FOR, "Adrià", "Capdevila", "Capdevila", 22, "ESP", 174, 66, RIGHT, CAM, LW_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 20);
        AddSimple(FOR, "Óscar", "Marcos", "Ó. Marcos", 22, "ESP", 176, 68, RIGHT, CAM, RW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 21);
        AddSimple(FOR, "Aldrine", "Kibet", "Kibet", 22, "KEN", 180, 72, RIGHT, LW, RW_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 11);
        AddSimple(FOR, "Ángel", "Arcos", "Arcos", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 7);
        AddSimple(FOR, "Luis", "Bilbao", "Bilbao", 22, "ESP", 178, 72, RIGHT, RW, LW_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 19);
        AddSimple(FOR, "Hugo", "González", "Hugo G.", 22, "ESP", 174, 66, RIGHT, RW, LW_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 22);
        AddSimple(FOR, "Germain", "Milla", "Milla", 22, "ESP", 180, 72, RIGHT, ST, CF_f, 64, 74, 200_000, 100_000, 1_500_000, 5, 9);
        AddSimple(FOR, "Álvaro", "Marín", "Álvaro Marín", 22, "ESP", 178, 72, RIGHT, ST, CF_f, 62, 72, 150_000, 80_000, 1_000_000, 5, 12);

        // ══════════════════════════════════════════════════════════
        // CD Mirandés (TeamId=43)
        // ══════════════════════════════════════════════════════════
        int MIR = 43;
        AddSimple(MIR, "Igor", "Nikic", "Nikic", 25, "MNE", 195, 89, RIGHT, GK, 0, 62, 68, 500000, 142, 5000000, 4, 1);
        AddSimple(MIR, "Juanpa", "Palomares", "Palomares", 25, "ESP", 187, 86, RIGHT, GK, 0, 62, 68, 500000, 142, 5000000, 4, 13);
        AddSimple(MIR, "Jorge", "Cabello", "Cabello", 22, "ESP", 183, 73, RIGHT, LB, CB_f, 60, 68, 900000, 257, 9000000, 4, 3);
        AddSimple(MIR, "Juan", "Gutiérrez", "J. Gutiérrez", 26, "ESP", 188, 78, RIGHT, CB, 0, 63, 68, 800000, 228, 8000000, 4, 5);
        AddSimple(MIR, "Nikola", "Maras", "Maras", 30, "SRB", 189, 79, RIGHT, CB, 0, 62, 68, 600000, 171, 6000000, 5, 15);
        AddSimple(MIR, "Adrián", "Pica", "Pica", 24, "ESP", 190, 79, RIGHT, CB, RB_f, 62, 68, 500000, 142, 5000000, 4, 4);
        AddSimple(MIR, "Iker", "Córdoba", "Córdoba", 20, "ESP", 190, 79, RIGHT, CB, 0, 60, 70, 500000, 142, 5000000, 3, 14);
        AddSimple(MIR, "Martín", "Pascual", "Pascual", 26, "ESP", 190, 79, RIGHT, CB, 0, 61, 66, 400000, 100, 4000000, 4, 6);
        AddSimple(MIR, "Sergio", "Postigo", "Postigo", 37, "ESP", 184, 77, RIGHT, CB, 0, 58, 58, 100000, 20, 1000000, 5, 16);
        AddSimple(MIR, "Pablo", "Pérez", "P. Pérez", 24, "ESP", 174, 69, RIGHT, LB, LW_f, 60, 68, 300000, 75, 3000000, 4, 2);
        AddSimple(MIR, "Hugo", "Novoa", "Novoa", 23, "ESP", 182, 72, RIGHT, RB, RW_f, 62, 70, 800000, 228, 8000000, 4, 17);
        AddSimple(MIR, "Mickaël", "Malsa", "Malsa", 30, "FRA", 179, 71, RIGHT, CDM, CM_f, 62, 66, 300000, 75, 3000000, 5, 8);
        AddSimple(MIR, "Selvi", "Clua", "Clua", 21, "ESP", 189, 75, RIGHT, CDM, CM_f, 60, 70, 300000, 75, 3000000, 4, 18);
        AddSimple(MIR, "Aarón", "Martin", "A. Martin", 19, "ESP", 175, 70, RIGHT, CM, 0, 60, 72, 800000, 228, 8000000, 3, 10);
        AddSimple(MIR, "Javi", "Hernández", "J. Hernández", 22, "ESP", 175, 68, RIGHT, CAM, 0, 62, 72, 1500000, 500, 15000000, 4, 11);
        AddSimple(MIR, "Diego", "Sia", "Sia", 20, "ITA", 170, 66, RIGHT, LW, RW_f, 60, 70, 400000, 100, 4000000, 3, 19);
        AddSimple(MIR, "Salim", "El Jebari", "El Jebari", 22, "MAR", 178, 69, RIGHT, LW, CAM_f, 61, 70, 400000, 100, 4000000, 4, 20);
        AddSimple(MIR, "Pablo", "López", "P. López", 20, "ESP", 175, 68, RIGHT, LW, RW_f, 60, 70, 300000, 75, 3000000, 3, 21);
        AddSimple(MIR, "Iker", "Varela", "Varela", 23, "ESP", 188, 73, RIGHT, LW, RW_f, 60, 68, 300000, 75, 3000000, 4, 22);
        AddSimple(MIR, "Ali", "Houary", "Houary", 20, "MAR", 178, 74, RIGHT, ST, LW_f, 60, 70, 300000, 75, 3000000, 3, 23);
        AddSimple(MIR, "Unax", "del Cura", "Del Cura", 21, "ESP", 182, 76, RIGHT, CF, 0, 62, 72, 2000000, 666, 20000000, 4, 9);
        AddSimple(MIR, "Carlos", "Fernández", "C. Fernández", 30, "ESP", 185, 77, RIGHT, CF, RW_f, 64, 66, 1200000, 400, 12000000, 5, 12);
        AddSimple(MIR, "Siren", "Diao", "Diao", 21, "ESP", 189, 79, RIGHT, CF, LW_f, 62, 72, 1000000, 333, 10000000, 4, 24);
        AddSimple(MIR, "Alberto", "Marí", "Marí", 24, "ESP", 183, 76, RIGHT, ST, 0, 62, 68, 600000, 171, 6000000, 4, 7);

        // ══════════════════════════════════════════════════════════
        // Cultural Leonesa (TeamId=44)
        // ══════════════════════════════════════════════════════════
        int CUL = 44;
        AddSimple(CUL, "Edgar", "Badia", "Badia", 34, "ESP", 180, 82, RIGHT, GK, 0, 64, 66, 400000, 100, 4000000, 5, 1);
        AddSimple(CUL, "Miguel", "Bañuz", "Bañuz", 33, "ESP", 188, 86, RIGHT, GK, 0, 62, 64, 200000, 50, 2000000, 5, 13);
        AddSimple(CUL, "Arnau", "Rafús", "Rafús", 23, "ESP", 190, 87, RIGHT, GK, 0, 62, 70, 200000, 50, 2000000, 4, 25);
        AddSimple(CUL, "Matia", "Barzic", "Barzic", 22, "CRO", 190, 79, RIGHT, CB, 0, 64, 74, 1000000, 333, 10000000, 4, 4);
        AddSimple(CUL, "Rodri", "Suárez", "Rodri S.", 23, "ESP", 186, 78, RIGHT, CB, 0, 63, 72, 500000, 142, 5000000, 4, 5);
        AddSimple(CUL, "Peru", "Rodríguez", "Peru R.", 24, "ESP", 180, 75, RIGHT, CB, CM_f, 62, 70, 400000, 100, 4000000, 4, 14);
        AddSimple(CUL, "Tomás", "Ribeiro", "T. Ribeiro", 27, "PRT", 185, 77, RIGHT, CB, LB_f, 63, 68, 400000, 100, 4000000, 4, 15);
        AddSimple(CUL, "Quique", "Fornos", "Fornos", 29, "ESP", 182, 76, RIGHT, CB, RB_f, 62, 66, 300000, 75, 3000000, 5, 3);
        AddSimple(CUL, "Eneko", "Satrústegui", "Satrústegui", 35, "ESP", 187, 78, RIGHT, CB, LB_f, 60, 60, 100000, 20, 1000000, 5, 16);
        AddSimple(CUL, "Roger", "Hinojo", "Hinojo", 21, "ESP", 180, 72, RIGHT, LB, CB_f, 62, 74, 1000000, 333, 10000000, 4, 2);
        AddSimple(CUL, "Homam", "Al-Amin", "Al-Amin", 26, "QAT", 188, 75, RIGHT, LB, LM_f, 62, 68, 500000, 142, 5000000, 4, 17);
        AddSimple(CUL, "Iván", "Calero", "Calero", 31, "ESP", 174, 69, RIGHT, RB, RW_f, 64, 66, 700000, 200, 7000000, 5, 6);
        AddSimple(CUL, "Víctor", "García", "V. García", 32, "VEN", 178, 71, RIGHT, RB, 0, 60, 62, 200000, 50, 2000000, 5, 18);
        AddSimple(CUL, "Nemanja", "Radoja", "Radoja", 33, "SRB", 186, 74, RIGHT, CDM, CM_f, 64, 64, 300000, 75, 3000000, 5, 8);
        AddSimple(CUL, "Thiago", "Ojeda", "Ojeda", 23, "ARG", 187, 74, RIGHT, CM, CAM_f, 64, 72, 600000, 171, 6000000, 4, 10);
        AddSimple(CUL, "Selu", "Diallo", "Diallo", 22, "GUI", 187, 74, RIGHT, CM, 0, 62, 70, 400000, 100, 4000000, 4, 19);
        AddSimple(CUL, "Yayo", "González", "Yayo", 21, "ESP", 175, 70, RIGHT, CM, 0, 60, 70, 300000, 75, 3000000, 4, 20);
        AddSimple(CUL, "Sergi", "Maestre", "Maestre", 35, "ESP", 187, 74, RIGHT, CM, CB_f, 60, 60, 100000, 20, 1000000, 5, 21);
        AddSimple(CUL, "Bicho", "", "Bicho", 30, "ESP", 175, 68, RIGHT, CAM, CM_f, 62, 66, 300000, 75, 3000000, 5, 11);
        AddSimple(CUL, "Luis", "Chacón", "Chacón", 26, "ESP", 174, 67, RIGHT, LW, RW_f, 64, 68, 1000000, 333, 10000000, 4, 7);
        AddSimple(CUL, "Diego", "Collado", "Collado", 25, "ESP", 183, 71, RIGHT, LW, RW_f, 62, 68, 400000, 100, 4000000, 4, 22);
        AddSimple(CUL, "Rafa", "Tresaco", "Tresaco", 25, "ESP", 180, 70, RIGHT, LW, CF_f, 60, 66, 300000, 75, 3000000, 4, 23);
        AddSimple(CUL, "Lucas", "Ribeiro", "L. Ribeiro", 27, "BRA", 181, 70, RIGHT, RW, CAM_f, 64, 68, 1500000, 500, 15000000, 4, 9);
        AddSimple(CUL, "Manu", "Justo", "Justo", 30, "ESP", 174, 73, RIGHT, ST, CF_f, 64, 66, 500000, 142, 5000000, 5, 12);
        AddSimple(CUL, "Rubén", "Sobrino", "Sobrino", 33, "ESP", 185, 77, RIGHT, ST, LW_f, 62, 64, 300000, 75, 3000000, 5, 24);

        // ══════════════════════════════════════════════════════════
        // SD Ponferradina (TeamId=45)
        // ══════════════════════════════════════════════════════════
        int PNF = 45;
        AddSimple(PNF, "Andrés", "Prieto", "Prieto", 32, "ESP", 194, 89, RIGHT, GK, 0, 63, 66, 150000, 30, 1500000, 5, 1);
        AddSimple(PNF, "Ángel", "Jiménez", "Á. Jiménez", 24, "ESP", 194, 89, RIGHT, GK, 0, 62, 70, 150000, 30, 1500000, 4, 13);
        AddSimple(PNF, "Pablo", "Barredo", "Barredo", 22, "ESP", 180, 82, RIGHT, GK, 0, 60, 70, 50000, 10, 500000, 4, 25);
        AddSimple(PNF, "Germán", "Nóvoa", "Nóvoa", 29, "ESP", 179, 75, RIGHT, CB, 0, 62, 66, 200000, 50, 2000000, 5, 4);
        AddSimple(PNF, "Boris", "Moltenis", "Moltenis", 27, "FRA", 184, 77, RIGHT, CB, RB_f, 62, 66, 250000, 62, 2500000, 4, 5);
        AddSimple(PNF, "David", "Andújar", "Andújar", 34, "ESP", 189, 79, RIGHT, CB, 0, 60, 60, 75000, 15, 750000, 5, 14);
        AddSimple(PNF, "Vladyslav", "Kysil", "Kysil", 21, "UKR", 193, 81, RIGHT, CB, CDM_f, 60, 72, 50000, 10, 500000, 4, 15);
        AddSimple(PNF, "Andoni", "López", "A. López", 30, "ESP", 187, 74, RIGHT, LB, 0, 62, 66, 250000, 62, 2500000, 5, 3);
        AddSimple(PNF, "Vasco", "Sousa", "V. Sousa", 22, "PRT", 184, 73, RIGHT, LB, CB_f, 60, 70, 100000, 20, 1000000, 4, 16);
        AddSimple(PNF, "Koke", "Iglesias", "Koke", 21, "ESP", 185, 74, RIGHT, RB, 0, 62, 72, 150000, 30, 1500000, 4, 2);
        AddSimple(PNF, "Mario", "Jorrín", "Jorrín", 23, "ESP", 180, 72, RIGHT, RB, CB_f, 60, 68, 100000, 20, 1000000, 4, 17);
        AddSimple(PNF, "Fede", "San Emeterio", "San Emeterio", 29, "ESP", 176, 70, RIGHT, CM, 0, 64, 68, 250000, 62, 2500000, 5, 6);
        AddSimple(PNF, "Eugene", "Frimpong", "Frimpong", 23, "GHA", 174, 69, RIGHT, CM, CAM_f, 62, 70, 150000, 30, 1500000, 4, 18);
        AddSimple(PNF, "Nuke", "Mfulu", "Mfulu", 32, "COD", 183, 73, RIGHT, CM, 0, 62, 64, 150000, 30, 1500000, 5, 8);
        AddSimple(PNF, "Vicente", "Esquerdo", "Esquerdo", 27, "ESP", 180, 72, RIGHT, CM, 0, 60, 66, 150000, 30, 1500000, 4, 19);
        AddSimple(PNF, "Eneko", "Undabarrena", "Undabarrena", 33, "ESP", 184, 73, RIGHT, CM, CB_f, 60, 62, 100000, 20, 1000000, 5, 20);
        AddSimple(PNF, "Erik", "Morán", "Morán", 35, "ESP", 187, 74, RIGHT, CM, 0, 60, 60, 50000, 10, 500000, 5, 21);
        AddSimple(PNF, "Xemi", "Fernández", "Xemi", 31, "ESP", 184, 71, RIGHT, CAM, 0, 62, 66, 200000, 50, 2000000, 5, 10);
        AddSimple(PNF, "Abdoulaye", "Keita", "Keita", 23, "ESP", 186, 72, RIGHT, LW, RW_f, 62, 70, 200000, 50, 2000000, 4, 11);
        AddSimple(PNF, "Carlos", "Calderón", "Calderón", 31, "ESP", 183, 71, RIGHT, RW, 0, 62, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(PNF, "Slavy", "", "Slavy", 24, "BUL", 182, 76, RIGHT, ST, 0, 64, 70, 400000, 100, 4000000, 4, 9);
        AddSimple(PNF, "José Luis", "Cortés", "J. L. Cortés", 24, "ESP", 176, 73, RIGHT, ST, 0, 62, 68, 200000, 50, 2000000, 4, 22);
        AddSimple(PNF, "Borja", "Vázquez", "B. Vázquez", 21, "ESP", 186, 78, RIGHT, ST, 0, 62, 72, 200000, 50, 2000000, 4, 23);
        AddSimple(PNF, "Borja", "Valle", "B. Valle", 33, "ESP", 177, 74, RIGHT, ST, 0, 62, 64, 150000, 30, 1500000, 5, 12);
        AddSimple(PNF, "Pau", "Ferrer", "Ferrer", 22, "ESP", 180, 75, RIGHT, ST, 0, 60, 70, 150000, 30, 1500000, 4, 24);

        // ══════════════════════════════════════════════════════════
        // CD Lugo (TeamId=46)
        // ══════════════════════════════════════════════════════════
        int LUG = 46;
        AddSimple(LUG, "Marc", "Martínez", "M. Martínez", 36, "ESP", 186, 85, RIGHT, GK, 0, 62, 64, 50000, 10, 500000, 5, 1);
        AddSimple(LUG, "Iker", "Piedra", "Piedra", 24, "ESP", 186, 85, RIGHT, GK, 0, 62, 70, 50000, 10, 500000, 4, 13);
        AddSimple(LUG, "Ibaider", "Garriz", "Garriz", 25, "ESP", 189, 79, RIGHT, CB, 0, 62, 68, 200000, 50, 2000000, 4, 4);
        AddSimple(LUG, "José", "Amo", "Amo", 28, "ESP", 182, 76, RIGHT, CB, 0, 62, 68, 200000, 50, 2000000, 5, 5);
        AddSimple(LUG, "Josep", "Gayá", "Gayá", 25, "ESP", 185, 77, RIGHT, CB, 0, 62, 68, 200000, 50, 2000000, 4, 14);
        AddSimple(LUG, "Pablo", "Trigueros", "Trigueros", 33, "ESP", 188, 78, RIGHT, CB, 0, 60, 62, 100000, 20, 1000000, 5, 15);
        AddSimple(LUG, "Diego", "Caballo", "Caballo", 32, "ESP", 171, 68, RIGHT, LB, 0, 60, 64, 100000, 20, 1000000, 5, 3);
        AddSimple(LUG, "Pere", "Haro", "Haro", 22, "ESP", 181, 72, RIGHT, LB, 0, 60, 70, 50000, 10, 500000, 4, 16);
        AddSimple(LUG, "Iago", "López", "I. López", 27, "ESP", 177, 70, RIGHT, RB, 0, 62, 66, 150000, 30, 1500000, 4, 2);
        AddSimple(LUG, "Yago", "Rodríguez", "Y. Rodríguez", 20, "ESP", 175, 70, RIGHT, RB, 0, 60, 72, 100000, 20, 1000000, 3, 17);
        AddSimple(LUG, "Gabriel", "Costa", "Costa", 19, "EQG", 176, 70, RIGHT, CDM, 0, 62, 74, 300000, 75, 3000000, 3, 6);
        AddSimple(LUG, "Álex", "Balboa", "Balboa", 25, "EQG", 181, 72, RIGHT, CM, 0, 62, 68, 250000, 62, 2500000, 4, 8);
        AddSimple(LUG, "Txus", "Alba", "Alba", 23, "ESP", 173, 69, RIGHT, CM, 0, 62, 70, 200000, 50, 2000000, 4, 10);
        AddSimple(LUG, "Antonio", "Perera", "Perera", 29, "ESP", 188, 75, RIGHT, CM, 0, 62, 66, 200000, 50, 2000000, 5, 18);
        AddSimple(LUG, "Kevin", "Presa", "Presa", 32, "ESP", 178, 71, RIGHT, CM, 0, 60, 62, 100000, 20, 1000000, 5, 19);
        AddSimple(LUG, "Neco", "Celorio", "Celorio", 22, "ESP", 172, 67, RIGHT, CAM, 0, 62, 72, 300000, 75, 3000000, 4, 11);
        AddSimple(LUG, "Santi", "Samanes", "Samanes", 30, "ESP", 176, 68, RIGHT, LW, 0, 62, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(LUG, "Borja", "Sánchez", "B. Sánchez", 30, "ESP", 186, 72, RIGHT, LW, 0, 62, 66, 200000, 50, 2000000, 5, 20);
        AddSimple(LUG, "Álex", "Gallar", "Gallar", 34, "ESP", 178, 69, RIGHT, LW, 0, 60, 62, 75000, 15, 750000, 5, 21);
        AddSimple(LUG, "Lago", "Júnior", "Lago Jr.", 35, "CIV", 180, 70, RIGHT, LW, 0, 58, 60, 75000, 15, 750000, 5, 22);
        AddSimple(LUG, "Víctor", "Pastrana", "Pastrana", 29, "ESP", 180, 70, RIGHT, RW, 0, 62, 66, 200000, 50, 2000000, 5, 12);
        AddSimple(LUG, "Jorge", "González", "J. González", 22, "ESP", 174, 67, RIGHT, RW, 0, 60, 70, 50000, 10, 500000, 4, 23);
        AddSimple(LUG, "Iker", "Unzueta", "Unzueta", 27, "ESP", 184, 77, RIGHT, ST, 0, 64, 68, 500000, 142, 5000000, 4, 9);
        AddSimple(LUG, "Nicolás", "Reniero", "Reniero", 31, "ARG", 183, 76, RIGHT, ST, 0, 62, 66, 200000, 50, 2000000, 5, 24);
        AddSimple(LUG, "Ognjen", "Teofilovic", "Teofilovic", 19, "SRB", 192, 80, RIGHT, ST, 0, 60, 74, 50000, 10, 500000, 3, 25);

        // ══════════════════════════════════════════════════════════
        // Racing Club de Ferrol (TeamId=47)
        // ══════════════════════════════════════════════════════════
        int FER = 47;
        AddSimple(FER, "Miquel", "Parera", "Parera", 30, "ESP", 182, 83, RIGHT, GK, 0, 62, 68, 500000, 142, 5000000, 5, 1);
        AddSimple(FER, "César", "Fernández", "C. Fernández", 22, "ESP", 181, 83, RIGHT, GK, 0, 60, 70, 150000, 30, 1500000, 4, 13);
        AddSimple(FER, "Lucas", "Díaz", "L. Díaz", 30, "ESP", 188, 86, RIGHT, GK, 0, 60, 64, 150000, 30, 1500000, 5, 25);
        AddSimple(FER, "Edgar", "Pujol", "Pujol", 21, "ESP", 177, 74, RIGHT, CB, 0, 60, 72, 250000, 62, 2500000, 4, 4);
        AddSimple(FER, "Álex", "Zalaya", "Zalaya", 28, "ESP", 184, 77, RIGHT, CB, 0, 62, 66, 250000, 62, 2500000, 5, 5);
        AddSimple(FER, "Markel", "Artetxe", "Artetxe", 27, "ESP", 183, 76, RIGHT, CB, 0, 62, 66, 200000, 50, 2000000, 4, 14);
        AddSimple(FER, "Chema", "Rodríguez", "Chema", 34, "ESP", 189, 79, RIGHT, CB, 0, 60, 62, 100000, 20, 1000000, 5, 15);
        AddSimple(FER, "Álvaro", "Ramón", "Ramón", 26, "ESP", 171, 68, RIGHT, LB, 0, 62, 68, 400000, 100, 4000000, 4, 3);
        AddSimple(FER, "Saúl", "García", "S. García", 31, "ESP", 182, 72, RIGHT, LB, 0, 62, 66, 200000, 50, 2000000, 5, 16);
        AddSimple(FER, "Miguel", "Leal", "Leal", 29, "ESP", 170, 68, RIGHT, RB, 0, 62, 68, 300000, 75, 3000000, 5, 2);
        AddSimple(FER, "Álvaro", "Mardones", "Mardones", 20, "ESP", 168, 67, RIGHT, RB, 0, 60, 72, 50000, 10, 500000, 3, 17);
        AddSimple(FER, "Fabio", "González", "F. González", 29, "ESP", 176, 70, RIGHT, CM, 0, 62, 68, 200000, 50, 2000000, 5, 6);
        AddSimple(FER, "Ander", "Gorostidi", "Gorostidi", 30, "ESP", 185, 74, RIGHT, CM, 0, 62, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(FER, "Sergio", "Tejera", "Tejera", 36, "ESP", 180, 72, RIGHT, CM, 0, 60, 60, 75000, 15, 750000, 5, 18);
        AddSimple(FER, "Pascu", "", "Pascu", 26, "ESP", 184, 71, RIGHT, CAM, 0, 62, 68, 350000, 87, 3500000, 4, 10);
        AddSimple(FER, "Jairo", "Noriega", "Noriega", 22, "ESP", 182, 70, RIGHT, CAM, 0, 62, 72, 300000, 75, 3000000, 4, 20);
        AddSimple(FER, "Raúl", "Dacosta", "Dacosta", 24, "ESP", 175, 68, RIGHT, LW, 0, 62, 70, 200000, 50, 2000000, 4, 11);
        AddSimple(FER, "Álvaro", "Juan", "Á. Juan", 26, "ESP", 178, 69, RIGHT, LW, 0, 60, 66, 150000, 30, 1500000, 4, 21);
        AddSimple(FER, "Azael", "García", "A. García", 22, "ESP", 178, 69, RIGHT, LW, 0, 60, 70, 50000, 10, 500000, 4, 22);
        AddSimple(FER, "David", "Concha", "Concha", 29, "ESP", 179, 69, RIGHT, RW, 0, 62, 66, 150000, 30, 1500000, 5, 7);
        AddSimple(FER, "Antón", "Escobar", "Escobar", 28, "ESP", 179, 75, RIGHT, ST, 0, 62, 68, 400000, 100, 4000000, 5, 9);
        AddSimple(FER, "Martim", "Tavares", "Tavares", 22, "PRT", 183, 76, RIGHT, ST, 0, 62, 72, 300000, 75, 3000000, 4, 12);
        AddSimple(FER, "Ekain", "Azkune", "Azkune", 25, "ESP", 180, 75, RIGHT, ST, 0, 60, 66, 150000, 30, 1500000, 4, 23);
        AddSimple(FER, "Álvaro", "Giménez", "Á. Giménez", 35, "ESP", 183, 76, RIGHT, ST, 0, 60, 60, 100000, 20, 1000000, 5, 24);

        // ══════════════════════════════════════════════════════════
        // Mérida AD (TeamId=48)
        // ══════════════════════════════════════════════════════════
        int MER = 48;
        AddSimple(MER, "Adrián", "Csenterics", "Csenterics", 21, "HUN", 196, 90, RIGHT, GK, 0, 62, 72, 300000, 75, 3000000, 4, 1);
        AddSimple(MER, "Pablo", "Cacharrón", "Cacharrón", 29, "ESP", 186, 85, RIGHT, GK, 0, 60, 66, 100000, 20, 1000000, 5, 13);
        AddSimple(MER, "Javi", "Lancho", "Lancho", 24, "ESP", 185, 77, RIGHT, CB, LB_f, 62, 68, 250000, 62, 2500000, 4, 4);
        AddSimple(MER, "Javi", "Domínguez", "J. Domínguez", 25, "ESP", 195, 81, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 4, 5);
        AddSimple(MER, "Luis", "Pareja", "Pareja", 27, "ESP", 187, 78, RIGHT, CB, 0, 60, 64, 150000, 30, 1500000, 4, 14);
        AddSimple(MER, "Gaizka", "Martínez", "G. Martínez", 29, "ESP", 193, 81, RIGHT, CB, CM_f, 60, 64, 150000, 30, 1500000, 5, 15);
        AddSimple(MER, "Eudald", "Vergés", "Vergés", 29, "ESP", 177, 70, RIGHT, LB, 0, 62, 66, 250000, 62, 2500000, 5, 3);
        AddSimple(MER, "Manu", "Rivas", "Rivas", 23, "ESP", 180, 72, RIGHT, LB, 0, 60, 68, 150000, 30, 1500000, 4, 16);
        AddSimple(MER, "Jacobo", "Martí", "Martí", 21, "ESP", 182, 72, RIGHT, RB, 0, 60, 72, 100000, 20, 1000000, 4, 2);
        AddSimple(MER, "Pipe", "", "Pipe", 33, "ESP", 175, 70, RIGHT, RB, 0, 58, 60, RIGHT, GK, 0, 5, 17);
        AddSimple(MER, "Martín", "Solar", "Solar", 26, "ESP", 175, 70, RIGHT, CM, 0, 62, 66, 300000, 75, 3000000, 4, 6);
        AddSimple(MER, "Geo", "Almeida", "Almeida", 21, "PRT", 185, 74, RIGHT, CM, 0, 62, 72, 250000, 62, 2500000, 4, 18);
        AddSimple(MER, "Raúl", "Beneit", "Beneit", 31, "ESP", 172, 68, RIGHT, CM, 0, 62, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(MER, "Víctor", "Corral", "Corral", 24, "ESP", 178, 71, RIGHT, CM, 0, 60, 68, 100000, 20, 1000000, 4, 19);
        AddSimple(MER, "Miguel", "Capitas", "Capitas", 25, "ESP", 175, 68, RIGHT, CAM, 0, 60, 66, 150000, 30, 1500000, 4, 10);
        AddSimple(MER, "Benny", "Dibrani", "Dibrani", 22, "FRA", 183, 71, RIGHT, CAM, 0, 60, 70, 150000, 30, 1500000, 4, 20);
        AddSimple(MER, "Chiqui", "", "Chiqui", 28, "ESP", 180, 70, RIGHT, LW, 0, 60, 64, 250000, 62, 2500000, 5, 11);
        AddSimple(MER, "Carlos", "Doncel", "Doncel", 29, "ESP", 167, 65, RIGHT, LW, 0, 60, 64, 250000, 62, 2500000, 5, 21);
        AddSimple(MER, "Rui", "Gomes", "R. Gomes", 28, "PRT", 175, 68, RIGHT, LW, 0, 60, 64, 150000, 30, 1500000, 5, 22);
        AddSimple(MER, "Javi", "Areso", "Areso", 28, "ESP", 187, 72, RIGHT, RW, 0, 60, 64, 150000, 30, 1500000, 5, 7);
        AddSimple(MER, "Sigurdur", "Hallsson", "Hallsson", 26, "ISL", 178, 74, RIGHT, CF, 0, 60, 66, 200000, 50, 2000000, 4, 9);
        AddSimple(MER, "Sofiane", "El Ftouhi", "El Ftouhi", 25, "FRA", 185, 77, RIGHT, CF, 0, 60, 68, 200000, 50, 2000000, 4, 12);
        AddSimple(MER, "Rodrigo", "Pereira", "R. Pereira", 23, "PRT", 188, 78, RIGHT, CF, 0, 60, 70, 50000, 10, 500000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Pontevedra CF (TeamId=49)
        // ══════════════════════════════════════════════════════════
        int PON = 49;
        AddSimple(PON, "Raúl", "Marqueta", "Marqueta", 31, "ESP", 185, 85, RIGHT, GK, 0, 62, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(PON, "Edu", "Sousa", "E. Sousa", 34, "ESP", 181, 83, RIGHT, GK, 0, 60, 62, 50000, 10, 500000, 5, 13);
        AddSimple(PON, "Antonio", "Montoro", "Montoro", 29, "ESP", 182, 76, RIGHT, CB, CM_f, 62, 66, 300000, 75, 3000000, 5, 4);
        AddSimple(PON, "Miki", "Bosch", "Bosch", 25, "ESP", 190, 79, RIGHT, CB, 0, 62, 68, 250000, 62, 2500000, 4, 5);
        AddSimple(PON, "Benjamín", "Garay", "Garay", 26, "ARG", 183, 76, RIGHT, CB, RB_f, 60, 66, 200000, 50, 2000000, 4, 14);
        AddSimple(PON, "Álvaro", "Pérez", "Á. Pérez", 30, "ESP", 183, 76, RIGHT, CB, RB_f, 60, 64, 100000, 20, 1000000, 5, 15);
        AddSimple(PON, "Juanra", "", "Juanra", 31, "ESP", 185, 77, RIGHT, CB, RB_f, 60, 64, 100000, 20, 1000000, 5, 16);
        AddSimple(PON, "Adrián", "Expósito", "Expósito", 24, "ESP", 175, 70, RIGHT, LB, 0, 62, 68, 150000, 30, 1500000, 4, 3);
        AddSimple(PON, "Víctor", "Eimil", "Eimil", 27, "ESP", 173, 69, RIGHT, RB, 0, 62, 66, 150000, 30, 1500000, 4, 2);
        AddSimple(PON, "Yelko", "Pino", "Pino", 29, "ESP", 167, 66, RIGHT, CM, CAM_f, 62, 66, 300000, 75, 3000000, 5, 8);
        AddSimple(PON, "Ander", "Vidorreta", "Vidorreta", 28, "ESP", 188, 75, RIGHT, CM, CB_f, 62, 66, 250000, 62, 2500000, 5, 6);
        AddSimple(PON, "Tiago", "Rodríguez", "T. Rodríguez", 25, "ESP", 180, 72, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 4, 10);
        AddSimple(PON, "Facundo", "Ballardo", "Ballardo", 30, "ARG", 182, 72, RIGHT, CM, CAM_f, 60, 64, 50000, 10, 500000, 5, 18);
        AddSimple(PON, "Alain", "Ribeiro", "Ribeiro", 28, "ESP", 189, 73, RIGHT, CAM, CM_f, 62, 66, 300000, 75, 3000000, 5, 20);
        AddSimple(PON, "Luisao", "Macías", "Macías", 21, "ECU", 175, 68, RIGHT, CAM, LW_f, 60, 72, 100000, 20, 1000000, 4, 21);
        AddSimple(PON, "Rubén", "López", "R. López", 21, "ESP", 175, 68, RIGHT, LW, CM_f, 60, 72, 500000, 142, 5000000, 4, 11);
        AddSimple(PON, "Miguel", "Cuesta", "Cuesta", 25, "ESP", 178, 69, RIGHT, LW, LB_f, 62, 68, 250000, 62, 2500000, 4, 7);
        AddSimple(PON, "Álex", "González", "Á. González", 36, "ESP", 178, 69, RIGHT, LW, RW_f, 58, 60, 50000, 10, 500000, 5, 22);
        AddSimple(PON, "Brais", "Abelenda", "Abelenda", 30, "ESP", 179, 69, RIGHT, RW, LW_f, 62, 66, 250000, 62, 2500000, 5, 12);
        AddSimple(PON, "Alberto", "Gil", "A. Gil", 29, "ESP", 178, 69, RIGHT, RW, LW_f, 60, 64, 200000, 50, 2000000, 5, 19);
        AddSimple(PON, "Diego", "Gómez", "D. Gómez", 21, "ESP", 177, 69, RIGHT, RW, LW_f, 60, 72, 200000, 50, 2000000, 4, 23);
        AddSimple(PON, "Alex", "Comparada", "Comparada", 22, "ESP", 183, 76, RIGHT, CF, 0, 62, 72, 300000, 75, 3000000, 4, 9);
        AddSimple(PON, "João", "Resende", "Resende", 23, "PRT", 178, 74, RIGHT, CF, RW_f, 60, 68, 250000, 62, 2500000, 4, 24);

        // ══════════════════════════════════════════════════════════
        // UD Logroñés (TeamId=50)
        // ══════════════════════════════════════════════════════════
        int LOG = 50;
        AddSimple(LOG, "Taliby", "Konate", "Konate", 28, "ESP", 190, 87, RIGHT, GK, 0, 60, 64, 200000, 50, 2000000, 5, 1);
        AddSimple(LOG, "Álex", "Daza", "Daza", 24, "ESP", 183, 84, RIGHT, GK, 0, 60, 68, 100000, 20, 1000000, 4, 13);
        AddSimple(LOG, "Eduardo", "Cabetas", "Cabetas", 30, "ESP", 190, 79, RIGHT, CB, 0, 62, 66, 150000, 30, 1500000, 5, 4);
        AddSimple(LOG, "Aritz", "Muguruza", "Muguruza", 24, "ESP", 180, 75, RIGHT, CB, 0, 60, 68, 100000, 20, 1000000, 4, 5);
        AddSimple(LOG, "Eder", "Larrea", "Larrea", 20, "ESP", 185, 77, RIGHT, CB, CM_f, 60, 72, 100000, 20, 1000000, 3, 14);
        AddSimple(LOG, "Andoni", "Ugarte", "Ugarte", 31, "ESP", 190, 79, RIGHT, CB, 0, 60, 64, 50000, 10, 500000, 5, 15);
        AddSimple(LOG, "Sergio", "Camacho", "Camacho", 24, "ESP", 173, 69, RIGHT, LB, 0, 62, 68, 200000, 50, 2000000, 4, 3);
        AddSimple(LOG, "Iñaki", "Sáenz", "Sáenz", 38, "ESP", 177, 70, RIGHT, LB, LW_f, 58, 60, 25000, 5, 250000, 5, 16);
        AddSimple(LOG, "José", "Val", "Val", 26, "ESP", 178, 71, RIGHT, RB, 0, 62, 66, 200000, 50, 2000000, 4, 2);
        AddSimple(LOG, "Aitor", "Pascual", "Pascual", 27, "ESP", 179, 71, RIGHT, RB, RW_f, 62, 66, 150000, 30, 1500000, 4, 17);
        AddSimple(LOG, "Carlos", "Benítez", "Benítez", 23, "ESP", 170, 68, RIGHT, CM, CAM_f, 62, 70, 250000, 62, 2500000, 4, 6);
        AddSimple(LOG, "Miguel", "Marí", "M. Marí", 28, "ESP", 185, 74, RIGHT, CM, CAM_f, 60, 64, 200000, 50, 2000000, 5, 8);
        AddSimple(LOG, "Iker", "Otadui", "Otadui", 21, "ESP", 180, 72, RIGHT, CM, CAM_f, 62, 72, 200000, 50, 2000000, 4, 18);
        AddSimple(LOG, "Quique", "Rivero", "Rivero", 34, "ESP", 176, 70, RIGHT, CM, CAM_f, 60, 62, 100000, 20, 1000000, 5, 10);
        AddSimple(LOG, "Latif", "Issman", "Issman", 19, "CIV", 178, 71, RIGHT, CM, 0, 58, 72, 25000, 5, 250000, 3, 19);
        AddSimple(LOG, "Anai", "Morales", "Morales", 20, "ESP", 175, 68, RIGHT, CAM, LW_f, 62, 74, 400000, 100, 4000000, 3, 11);
        AddSimple(LOG, "Urki", "Txoperena", "Txoperena", 27, "ESP", 172, 67, RIGHT, CAM, LW_f, 60, 64, 150000, 30, 1500000, 4, 20);
        AddSimple(LOG, "Alex", "Cerdá", "Cerdá", 23, "ESP", 184, 71, RIGHT, CAM, CM_f, 60, 68, 100000, 20, 1000000, 4, 21);
        AddSimple(LOG, "Ismael", "Santana", "Santana", 22, "ESP", 175, 68, RIGHT, RW, LW_f, 62, 72, 300000, 75, 3000000, 4, 7);
        AddSimple(LOG, "Joel", "Febas", "Febas", 24, "ESP", 171, 66, RIGHT, RW, CF_f, 60, 68, 100000, 20, 1000000, 4, 22);
        AddSimple(LOG, "Berto", "Rosas", "Rosas", 23, "AND", 179, 75, RIGHT, ST, RW_f, 62, 70, 300000, 75, 3000000, 4, 9);
        AddSimple(LOG, "Andrei", "Lupu", "Lupu", 28, "ROU", 177, 74, RIGHT, ST, 0, 62, 66, 150000, 30, 1500000, 5, 12);

        // ══════════════════════════════════════════════════════════
        // Unionistas de Salamanca (TeamId=51)
        // ══════════════════════════════════════════════════════════
        int UNI = 51;
        AddSimple(UNI, "Salvi", "Carrasco", "Carrasco", 26, "ESP", 192, 88, RIGHT, GK, 0, 62, 68, 400000, 100, 4000000, 4, 1);
        AddSimple(UNI, "Marco", "Suárez", "M. Suárez", 22, "ESP", 185, 85, RIGHT, GK, 0, 60, 70, 100000, 20, 1000000, 4, 13);
        AddSimple(UNI, "Vadik", "Murria", "Murria", 25, "ESP", 188, 78, RIGHT, CB, 0, 62, 68, 250000, 62, 2500000, 4, 4);
        AddSimple(UNI, "Aleix", "Gorjón", "Gorjón", 23, "ESP", 182, 76, RIGHT, CB, 0, 60, 68, 200000, 50, 2000000, 4, 5);
        AddSimple(UNI, "Farru", "", "Farru", 25, "ESP", 183, 76, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 4, 14);
        AddSimple(UNI, "Ramiro", "Mayor", "Mayor", 35, "ESP", 185, 77, RIGHT, CB, 0, 60, 62, 50000, 10, 500000, 5, 15);
        AddSimple(UNI, "Mounir", "Errahaly", "Errahaly", 26, "MAR", 180, 72, RIGHT, LB, 0, 62, 66, 200000, 50, 2000000, 4, 3);
        AddSimple(UNI, "Raúl", "Prada", "Prada", 25, "ESP", 180, 72, RIGHT, LB, 0, 62, 68, 200000, 50, 2000000, 4, 16);
        AddSimple(UNI, "Víctor", "Olmedo", "Olmedo", 27, "ESP", 170, 68, RIGHT, RB, 0, 62, 66, 400000, 100, 4000000, 4, 2);
        AddSimple(UNI, "Jan", "Encuentra", "Encuentra", 20, "ESP", 178, 71, RIGHT, RB, 0, 60, 72, 200000, 50, 2000000, 3, 17);
        AddSimple(UNI, "Abderrezzek", "Saidi", "Saidi", 20, "ALG", 178, 71, RIGHT, RB, 0, 60, 72, 50000, 10, 500000, 3, 18);
        AddSimple(UNI, "José", "Masllorens", "Masllorens", 25, "ESP", 183, 73, RIGHT, CM, 0, 62, 68, 300000, 75, 3000000, 4, 6);
        AddSimple(UNI, "Juanje", "Rodríguez", "Juanje", 26, "ESP", 178, 71, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 4, 8);
        AddSimple(UNI, "Jota", "López", "Jota", 23, "ESP", 178, 71, RIGHT, CM, 0, 62, 70, 200000, 50, 2000000, 4, 10);
        AddSimple(UNI, "Juanma", "Lendínez", "Lendínez", 22, "ESP", 175, 70, RIGHT, CM, 0, 60, 70, 150000, 30, 1500000, 4, 19);
        AddSimple(UNI, "Carlos", "de la Nava", "De la Nava", 33, "ESP", 188, 73, RIGHT, CAM, 0, 60, 64, 150000, 30, 1500000, 5, 20);
        AddSimple(UNI, "Hugo", "de Bustos", "De Bustos", 21, "ESP", 181, 70, RIGHT, LW, 0, 60, 72, 100000, 20, 1000000, 4, 11);
        AddSimple(UNI, "Álvaro", "Gómez", "Á. Gómez", 29, "ESP", 178, 69, RIGHT, RW, 0, 62, 66, 300000, 75, 3000000, 5, 7);
        AddSimple(UNI, "Serpeta", "", "Serpeta", 26, "ESP", 179, 69, RIGHT, RW, LW_f, 62, 68, 200000, 50, 2000000, 4, 12);
        AddSimple(UNI, "Adam", "Arvelo", "Arvelo", 21, "ESP", 176, 68, RIGHT, RW, 0, 60, 72, 50000, 10, 500000, 4, 21);
        AddSimple(UNI, "Juan", "Artola", "Artola", 26, "ESP", 178, 74, RIGHT, CF, RW_f, 62, 68, 200000, 50, 2000000, 4, 9);
        AddSimple(UNI, "Josué", "Chibozo", "Chibozo", 22, "BEN", 175, 73, RIGHT, CF, 0, 60, 70, 150000, 30, 1500000, 4, 22);
        AddSimple(UNI, "Aarón", "Piñán", "Piñán", 28, "ESP", 180, 75, RIGHT, CF, LW_f, 60, 64, 150000, 30, 1500000, 5, 23);
        AddSimple(UNI, "Pere", "Marco", "Marco", 22, "ESP", 178, 74, RIGHT, ST, 0, 62, 72, 200000, 50, 2000000, 4, 24);

        // ══════════════════════════════════════════════════════════
        // Barakaldo CF (TeamId=52)
        // ══════════════════════════════════════════════════════════
        int BRK = 52;
        AddSimple(BRK, "Gaizka", "Campos", "Campos", 29, "ESP", 190, 87, RIGHT, GK, 0, 62, 66, 150000, 30, 1500000, 5, 1);
        AddSimple(BRK, "Ibon", "Ispizua", "Ispizua", 23, "ESP", 185, 85, RIGHT, GK, 0, 60, 70, 100000, 20, 1000000, 4, 13);
        AddSimple(BRK, "Markel", "Arana", "Arana", 24, "ESP", 186, 78, RIGHT, CB, 0, 62, 68, 300000, 75, 3000000, 4, 4);
        AddSimple(BRK, "Unai", "Dufur", "Dufur", 27, "ESP", 187, 78, RIGHT, CB, 0, 62, 66, 200000, 50, 2000000, 4, 5);
        AddSimple(BRK, "Iker", "Ropero", "Ropero", 20, "ESP", 186, 78, RIGHT, CB, 0, 60, 72, 100000, 20, 1000000, 3, 14);
        AddSimple(BRK, "Javier", "Vicario", "Vicario", 24, "ESP", 188, 78, RIGHT, CB, 0, 58, 64, 50000, 10, 500000, 4, 15);
        AddSimple(BRK, "Borja", "García", "B. García", 36, "ESP", 185, 77, RIGHT, CB, 0, 58, 60, 50000, 10, 500000, 5, 16);
        AddSimple(BRK, "Oier", "López", "O. López", 24, "ESP", 178, 71, RIGHT, LB, LM_f, 62, 68, 300000, 75, 3000000, 4, 3);
        AddSimple(BRK, "Urtzi", "Albizua", "Albizua", 20, "ESP", 176, 70, RIGHT, LB, LM_f, 60, 72, 150000, 30, 1500000, 3, 17);
        AddSimple(BRK, "Beñat", "de Jesús", "De Jesús", 24, "ESP", 178, 71, RIGHT, RB, 0, 62, 68, 200000, 50, 2000000, 4, 2);
        AddSimple(BRK, "Iker", "Pedernales", "Pedernales", 27, "ESP", 175, 70, RIGHT, RB, 0, 62, 66, 200000, 50, 2000000, 4, 18);
        AddSimple(BRK, "Julen", "Huidobro", "Huidobro", 31, "ESP", 177, 70, RIGHT, CM, 0, 62, 66, 200000, 50, 2000000, 5, 6);
        AddSimple(BRK, "Unai", "Naveira", "Naveira", 25, "ESP", 183, 73, RIGHT, CM, 0, 60, 66, 150000, 30, 1500000, 4, 8);
        AddSimple(BRK, "Iván", "Castillo", "Castillo", 26, "ESP", 177, 70, RIGHT, CM, 0, 60, 66, 100000, 20, 1000000, 4, 19);
        AddSimple(BRK, "Álvaro", "Peña", "Á. Peña", 34, "ESP", 179, 71, RIGHT, CM, CAM_f, 60, 62, 75000, 15, 750000, 5, 10);
        AddSimple(BRK, "Ekaitz", "Molina", "Molina", 35, "ESP", 182, 72, RIGHT, CM, 0, 58, 60, 50000, 10, 500000, 5, 20);
        AddSimple(BRK, "Eric", "Pérez", "E. Pérez", 25, "ESP", 180, 70, RIGHT, LW, RW_f, 62, 68, 200000, 50, 2000000, 4, 11);
        AddSimple(BRK, "José", "de León", "De León", 22, "ESP", 187, 72, RIGHT, LW, RW_f, 60, 70, 150000, 30, 1500000, 4, 21);
        AddSimple(BRK, "Jon", "Cabo", "Cabo", 26, "ESP", 184, 71, RIGHT, LW, ST_f, 60, 66, 50000, 10, 500000, 4, 22);
        AddSimple(BRK, "Marc", "Torra", "Torra", 21, "ESP", 178, 69, RIGHT, LW, RW_f, 60, 72, 50000, 10, 500000, 4, 23);
        AddSimple(BRK, "Saliou", "Mandiang", "Mandiang", 23, "SEN", 183, 71, RIGHT, RW, LW_f, 62, 70, 200000, 50, 2000000, 4, 7);
        AddSimple(BRK, "Iñigo", "Muñoz", "I. Muñoz", 29, "ESP", 165, 64, RIGHT, RW, LW_f, 60, 64, 150000, 30, 1500000, 5, 12);
        AddSimple(BRK, "Aitor", "Galarza", "Galarza", 23, "ESP", 182, 76, RIGHT, ST, CF_f, 62, 72, 250000, 62, 2500000, 4, 9);
        AddSimple(BRK, "Alex", "Valiño", "Valiño", 28, "ESP", 185, 77, RIGHT, ST, CF_f, 62, 66, 200000, 50, 2000000, 5, 24);
        AddSimple(BRK, "Sabin", "Merino", "Merino", 34, "ESP", 187, 78, RIGHT, ST, CF_f, 60, 62, 75000, 15, 750000, 5, 25);

        // ══════════════════════════════════════════════════════════
        // Zamora CF (TeamId=53)
        // ══════════════════════════════════════════════════════════
        int ZAM = 53;
        AddSimple(ZAM, "Fermín", "Sobrón", "Sobrón", 30, "ESP", 177, 81, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(ZAM, "Adrián", "Pereda", "Pereda", 23, "ESP", 194, 89, RIGHT, GK, 0, 60, 70, RIGHT, GK, 0, 4, 13);
        AddSimple(ZAM, "Luismi", "Luengo", "Luengo", 25, "ESP", 185, 77, RIGHT, CB, 0, 62, 68, 250000, 62, 2500000, 4, 4);
        AddSimple(ZAM, "Miki", "Codina", "Codina", 24, "ESP", 185, 77, RIGHT, CB, 0, 62, 68, 250000, 62, 2500000, 4, 5);
        AddSimple(ZAM, "Erik", "Ruiz", "Ruiz", 31, "ESP", 184, 77, RIGHT, CB, 0, 60, 64, 150000, 30, 1500000, 5, 14);
        AddSimple(ZAM, "Ismael", "Athuman", "Athuman", 31, "KEN", 185, 77, RIGHT, CB, 0, 60, 64, 100000, 20, 1000000, 5, 15);
        AddSimple(ZAM, "Dani", "Merchán", "Merchán", 24, "ESP", 180, 72, RIGHT, LB, 0, 62, 68, 200000, 50, 2000000, 4, 3);
        AddSimple(ZAM, "Mani", "López", "M. López", 19, "MAR", 178, 71, RIGHT, LB, 0, 60, 74, RIGHT, GK, 0, 3, 16);
        AddSimple(ZAM, "Diego", "Moreno", "D. Moreno", 25, "ESP", 177, 70, RIGHT, RB, 0, 62, 68, 200000, 50, 2000000, 4, 2);
        AddSimple(ZAM, "Sergi", "López", "S. López", 22, "ESP", 180, 72, RIGHT, RB, 0, 62, 72, 200000, 50, 2000000, 4, 17);
        AddSimple(ZAM, "Markel", "Lozano", "Lozano", 30, "ESP", 189, 75, RIGHT, CM, 0, 62, 66, 300000, 75, 3000000, 5, 6);
        AddSimple(ZAM, "Mario", "García", "M. García", 28, "ESP", 174, 69, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(ZAM, "Carlos", "Ramos", "C. Ramos", 31, "ESP", 182, 72, RIGHT, CM, 0, 60, 64, 200000, 50, 2000000, 5, 10);
        AddSimple(ZAM, "Alejandro", "Marcelo", "Marcelo", 27, "ESP", 178, 71, RIGHT, CM, 0, 58, 64, 100000, 20, 1000000, 4, 18);
        AddSimple(ZAM, "Abde", "Damar", "Damar", 26, "MAR", 172, 67, RIGHT, LW, 0, 62, 68, 300000, 75, 3000000, 4, 11);
        AddSimple(ZAM, "Álvaro", "Romero", "Á. Romero", 30, "ESP", 167, 65, RIGHT, LW, 0, 60, 64, 150000, 30, 1500000, 5, 19);
        AddSimple(ZAM, "Loren", "Burón", "Burón", 31, "ESP", 170, 66, RIGHT, LW, 0, 60, 64, 150000, 30, 1500000, 5, 20);
        AddSimple(ZAM, "Jaime", "Sancho", "Sancho", 25, "ESP", 172, 67, RIGHT, RW, 0, 62, 68, 250000, 62, 2500000, 4, 7);
        AddSimple(ZAM, "Kike", "Márquez", "Márquez", 36, "ESP", 176, 73, RIGHT, CF, CAM_f, 60, 62, 100000, 20, 1000000, 5, 9);
        AddSimple(ZAM, "Javier", "Carbonell", "Carbonell", 29, "ESP", 183, 76, RIGHT, CF, 0, 62, 66, 250000, 62, 2500000, 5, 12);
        AddSimple(ZAM, "Mario", "Losada", "Losada", 29, "ESP", 175, 73, RIGHT, CF, 0, 60, 64, 200000, 50, 2000000, 5, 21);
        AddSimple(ZAM, "Joshua", "Farrell", "Farrell", 23, "WAL", 182, 76, RIGHT, CF, 0, 60, 68, 200000, 50, 2000000, 4, 22);

        // ══════════════════════════════════════════════════════════
        // Real Avilés (TeamId=54)
        // ══════════════════════════════════════════════════════════
        int AVI = 54;
        AddSimple(AVI, "Álvaro", "Fernández", "Á. Fernández", 26, "ESP", 185, 85, RIGHT, GK, 0, 60, 68, 200000, 50, 2000000, 4, 1);
        AddSimple(AVI, "Nando", "Almodóvar", "Almodóvar", 22, "ESP", 192, 88, RIGHT, GK, 0, 60, 70, 50000, 10, 500000, 4, 13);
        AddSimple(AVI, "Ricardo", "Grigore", "Grigore", 27, "ROU", 192, 80, RIGHT, CB, 0, 60, 66, 100000, 20, 1000000, 4, 4);
        AddSimple(AVI, "Julio", "Rodríguez", "J. Rodríguez", 30, "ESP", 194, 81, RIGHT, CB, 0, 60, 64, 50000, 10, 500000, 5, 5);
        AddSimple(AVI, "Jean-Sylvain", "Babin", "Babin", 39, "FRA", 181, 76, RIGHT, CB, 0, 60, 60, 50000, 10, 500000, 5, 14);
        AddSimple(AVI, "Chukwuma", "Eze", "Eze", 22, "NGA", 185, 77, RIGHT, CB, 0, 58, 68, 50000, 10, 500000, 4, 15);
        AddSimple(AVI, "Andrés", "López", "A. López", 23, "ESP", 190, 79, RIGHT, CB, 0, 60, 68, 50000, 10, 500000, 4, 16);
        AddSimple(AVI, "Osky", "Menéndez", "Menéndez", 22, "ESP", 176, 70, RIGHT, LB, 0, 60, 70, 150000, 30, 1500000, 4, 3);
        AddSimple(AVI, "Viti", "Díaz", "Viti", 35, "ESP", 180, 72, RIGHT, LB, 0, 58, 60, 50000, 10, 500000, 5, 17);
        AddSimple(AVI, "Guzmán", "Ortega", "Ortega", 22, "ESP", 178, 71, RIGHT, RB, 0, 60, 72, 150000, 30, 1500000, 4, 2);
        AddSimple(AVI, "Edu", "Campabadal", "Campabadal", 33, "ESP", 178, 71, RIGHT, RB, 0, 60, 64, 100000, 20, 1000000, 5, 18);
        AddSimple(AVI, "Kevin", "Bautista", "Bautista", 27, "ESP", 187, 74, RIGHT, CM, 0, 62, 68, 300000, 75, 3000000, 4, 6);
        AddSimple(AVI, "Álvaro", "Gete", "Gete", 25, "ESP", 182, 72, RIGHT, CM, 0, 60, 68, 200000, 50, 2000000, 4, 8);
        AddSimple(AVI, "Christian", "Rivera", "Rivera", 28, "ESP", 191, 76, RIGHT, CM, 0, 62, 68, 200000, 50, 2000000, 5, 10);
        AddSimple(AVI, "Adrián", "Gómez", "A. Gómez", 32, "ESP", 182, 72, RIGHT, CM, 0, 58, 62, 100000, 20, 1000000, 5, 19);
        AddSimple(AVI, "Edu", "Cortina", "Cortina", 29, "ESP", 174, 69, RIGHT, CM, 0, 58, 62, 50000, 10, 500000, 5, 20);
        AddSimple(AVI, "Luis", "Alcalde", "Alcalde", 31, "ESP", 184, 71, RIGHT, CAM, CM_f, 60, 64, 150000, 30, 1500000, 5, 11);
        AddSimple(AVI, "Isi", "Ros", "Isi Ros", 30, "ESP", 168, 65, RIGHT, LW, 0, 60, 64, 150000, 30, 1500000, 5, 7);
        AddSimple(AVI, "Berto", "Cayarga", "Cayarga", 29, "ESP", 170, 66, RIGHT, RW, 0, 62, 66, 200000, 50, 2000000, 5, 12);
        AddSimple(AVI, "Quicala", "Bari", "Bari", 24, "GNB", 177, 69, RIGHT, RW, 0, 60, 68, 200000, 50, 2000000, 4, 21);
        AddSimple(AVI, "Álvaro", "Santamaría", "Santamaría", 24, "ESP", 184, 77, RIGHT, ST, 0, 62, 70, 250000, 62, 2500000, 4, 9);
        AddSimple(AVI, "Javi", "Cueto", "Cueto", 25, "ESP", 182, 76, RIGHT, ST, 0, 62, 68, 250000, 62, 2500000, 4, 22);
        AddSimple(AVI, "Raúl", "Rubio", "R. Rubio", 26, "ESP", 182, 76, RIGHT, ST, 0, 60, 66, 200000, 50, 2000000, 4, 23);
        AddSimple(AVI, "Aitor", "Uzkudun", "Uzkudun", 25, "ESP", 183, 76, RIGHT, ST, 0, 60, 68, 200000, 50, 2000000, 4, 24);
        AddSimple(AVI, "Natalio", "", "Natalio", 41, "ESP", 176, 73, RIGHT, ST, 0, 56, 56, 25000, 5, 250000, 5, 25);

        // ══════════════════════════════════════════════════════════
        // Real Unión (TeamId=55)
        // ══════════════════════════════════════════════════════════
        int RUN = 55;
        AddSimple(RUN, "Jon", "Tena", "Tena", 33, "ESP", 189, 86, RIGHT, GK, 0, 60, 64, 100000, 20, 1000000, 5, 1);
        AddSimple(RUN, "Lander", "Emery", "Emery", 23, "ESP", 190, 87, RIGHT, GK, 0, 60, 70, 25000, 5, 250000, 4, 13);
        AddSimple(RUN, "Hugo", "Carrillo", "Carrillo", 24, "ESP", 185, 77, RIGHT, CB, 0, 62, 68, 200000, 50, 2000000, 4, 4);
        AddSimple(RUN, "David", "Fernández", "D. Fernández", 25, "ESP", 180, 75, RIGHT, CB, 0, 60, 66, 150000, 30, 1500000, 4, 5);
        AddSimple(RUN, "Manolo", "Molina", "Molina", 30, "ESP", 189, 79, RIGHT, CB, 0, 60, 64, 100000, 20, 1000000, 5, 14);
        AddSimple(RUN, "Thierry", "Katsukunya", "Katsukunya", 20, "WAL", 185, 77, RIGHT, CB, 0, 58, 72, 50000, 10, 500000, 3, 15);
        AddSimple(RUN, "Álvaro", "Mateo", "Mateo", 29, "ESP", 174, 69, RIGHT, LB, 0, 62, 66, 200000, 50, 2000000, 5, 3);
        AddSimple(RUN, "Aimar", "Labaka", "Labaka", 22, "ESP", 178, 71, RIGHT, LB, 0, 58, 68, 25000, 5, 250000, 4, 16);
        AddSimple(RUN, "Javi", "Fontán", "Fontán", 26, "ESP", 182, 72, RIGHT, RB, 0, 62, 68, 200000, 50, 2000000, 4, 2);
        AddSimple(RUN, "Corentin", "Louakima", "Louakima", 23, "FRA", 183, 73, RIGHT, RB, 0, 60, 68, 100000, 20, 1000000, 4, 17);
        AddSimple(RUN, "Martín", "Arruti", "Arruti", 21, "ESP", 175, 70, RIGHT, CM, 0, 62, 74, 200000, 50, 2000000, 4, 6);
        AddSimple(RUN, "Juan", "de la Mata", "De la Mata", 28, "ESP", 187, 74, RIGHT, CM, 0, 62, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(RUN, "Javi", "Mecerreyes", "Mecerreyes", 25, "ESP", 183, 73, RIGHT, CM, 0, 60, 66, 150000, 30, 1500000, 4, 10);
        AddSimple(RUN, "Luca", "Sangalli", "Sangalli", 31, "ESP", 167, 66, RIGHT, CM, 0, 60, 64, 100000, 20, 1000000, 5, 18);
        AddSimple(RUN, "Beñat", "Gaubeka", "Gaubeka", 25, "ESP", 180, 72, RIGHT, CM, 0, 58, 66, 50000, 10, 500000, 4, 19);
        AddSimple(RUN, "Ignacio", "Tellechea", "Tellechea", 29, "ESP", 175, 68, RIGHT, LW, 0, 60, 64, 200000, 50, 2000000, 5, 11);
        AddSimple(RUN, "Manex", "Iradi", "Iradi", 28, "ESP", 175, 68, RIGHT, LW, 0, 60, 64, 100000, 20, 1000000, 5, 20);
        AddSimple(RUN, "Mustapha Hussaini", "Muhammad", "Muhammad", 20, "NGA", 178, 69, RIGHT, LW, 0, 58, 72, 50000, 10, 500000, 3, 21);
        AddSimple(RUN, "Unai", "García", "U. García", 24, "ESP", 176, 68, RIGHT, RW, 0, 62, 70, 250000, 62, 2500000, 4, 7);
        AddSimple(RUN, "Javier", "Soroeta", "Soroeta", 21, "ESP", 180, 75, RIGHT, ST, 0, 62, 74, 400000, 100, 4000000, 4, 9);
        AddSimple(RUN, "Santi", "Miguélez", "Miguélez", 24, "ESP", 180, 75, RIGHT, ST, 0, 60, 68, 150000, 30, 1500000, 4, 12);
        AddSimple(RUN, "Peru", "Ruiz", "P. Ruiz", 25, "ESP", 182, 76, RIGHT, ST, 0, 60, 66, 100000, 20, 1000000, 4, 22);
        AddSimple(RUN, "Laken", "Torres", "Torres", 21, "ESP", 185, 77, RIGHT, ST, 0, 60, 72, 100000, 20, 1000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Cacereño (TeamId=56)
        // ══════════════════════════════════════════════════════════
        int CAC = 56;
        AddSimple(CAC, "Diego", "Nieves", "Nieves", 27, "ESP", 185, 85, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 4, 1);
        AddSimple(CAC, "Alex", "Quevedo", "Quevedo", 22, "ESP", 191, 87, RIGHT, GK, 0, 60, 70, 50000, 10, 500000, 4, 13);
        AddSimple(CAC, "Iván", "Martínez", "I. Martínez", 24, "ESP", 181, 76, RIGHT, CB, 0, 60, 68, 200000, 50, 2000000, 4, 4);
        AddSimple(CAC, "José Manuel", "Alonso", "Alonso", 30, "ESP", 185, 77, RIGHT, CB, 0, 60, 64, 150000, 30, 1500000, 5, 5);
        AddSimple(CAC, "Adrián", "Crespo", "Crespo", 33, "ESP", 185, 77, RIGHT, CB, 0, 58, 62, 100000, 20, 1000000, 5, 14);
        AddSimple(CAC, "Javi", "Barrio", "Barrio", 35, "ESP", 186, 78, RIGHT, CB, 0, 58, 60, 50000, 10, 500000, 5, 15);
        AddSimple(CAC, "Joserra", "de Diego", "Joserra", 27, "ESP", 178, 71, RIGHT, LB, 0, 62, 66, 250000, 62, 2500000, 4, 3);
        AddSimple(CAC, "Oussama", "El Goumiri", "El Goumiri", 23, "MAR", 183, 73, RIGHT, LB, 0, 60, 68, 50000, 10, 500000, 4, 16);
        AddSimple(CAC, "Emi", "Hernández", "E. Hernández", 25, "ESP", 175, 70, RIGHT, RB, 0, 60, 66, 200000, 50, 2000000, 4, 2);
        AddSimple(CAC, "Nico", "Conesa", "Conesa", 24, "ESP", 183, 73, RIGHT, CM, 0, 60, 66, 150000, 30, 1500000, 4, 6);
        AddSimple(CAC, "Deco", "", "Deco", 30, "ESP", 168, 67, RIGHT, CM, 0, 58, 62, 100000, 20, 1000000, 5, 8);
        AddSimple(CAC, "Wilfrid", "Kaptoum", "Kaptoum", 29, "CMR", 174, 69, RIGHT, CM, 0, 60, 64, 100000, 20, 1000000, 5, 10);
        AddSimple(CAC, "Diego", "Gutiérrez", "D. Gutiérrez", 25, "ESP", 171, 68, RIGHT, CM, 0, 58, 66, 100000, 20, 1000000, 4, 18);
        AddSimple(CAC, "Rubén", "Valdera", "Valdera", 23, "ESP", 185, 74, RIGHT, CM, 0, 60, 68, 100000, 20, 1000000, 4, 19);
        AddSimple(CAC, "Antonio", "Marchena", "Marchena", 23, "ESP", 182, 72, RIGHT, CM, 0, 58, 66, 50000, 10, 500000, 4, 20);
        AddSimple(CAC, "Raúl", "Sanchís", "Sanchís", 23, "ESP", 180, 72, RIGHT, CM, 0, 58, 66, 50000, 10, 500000, 4, 21);
        AddSimple(CAC, "Javier", "Ajenjo", "Ajenjo", 25, "ESP", 178, 69, RIGHT, CAM, 0, 60, 66, 200000, 50, 2000000, 4, 11);
        AddSimple(CAC, "Álex", "Monerris", "Monerris", 25, "ESP", 181, 70, RIGHT, LW, 0, 60, 66, 100000, 20, 1000000, 4, 7);
        AddSimple(CAC, "Miguel", "Berlanga", "Berlanga", 25, "ESP", 187, 72, RIGHT, RW, 0, 60, 66, 200000, 50, 2000000, 4, 22);
        AddSimple(CAC, "Iván", "Fernández", "I. Fernández", 26, "ESP", 179, 69, RIGHT, RW, 0, 60, 64, 150000, 30, 1500000, 4, 12);
        AddSimple(CAC, "Rubén", "Sanchidrián", "Sanchidrián", 26, "ESP", 177, 69, RIGHT, RW, 0, 60, 64, 150000, 30, 1500000, 4, 23);
        AddSimple(CAC, "Diego", "Gómez", "D. Gómez", 27, "ESP", 187, 78, RIGHT, CF, 0, 60, 64, 200000, 50, 2000000, 4, 9);
        AddSimple(CAC, "Pau", "Palacín", "Palacín", 26, "ESP", 189, 79, RIGHT, CF, 0, 60, 66, 200000, 50, 2000000, 4, 24);
        AddSimple(CAC, "Kensly", "Vázquez", "Vázquez", 23, "VEN", 177, 74, RIGHT, CF, 0, 58, 68, 150000, 30, 1500000, 4, 25);
        AddSimple(CAC, "César", "Gómez", "C. Gómez", 26, "ESP", 180, 75, RIGHT, CF, 0, 60, 66, 100000, 20, 1000000, 4, 26);

        // ══════════════════════════════════════════════════════════
        // UD Ourense (TeamId=57)
        // ══════════════════════════════════════════════════════════
        int OUR = 57;
        AddSimple(OUR, "Álvaro", "Ratón", "Ratón", 30, "ESP", 186, 85, RIGHT, GK, 0, 60, 66, 75000, 15, 750000, 5, 1);
        AddSimple(OUR, "Alberto", "Sánchez", "A. Sánchez", 24, "ESP", 183, 84, RIGHT, GK, 0, 58, 66, 50000, 10, 500000, 4, 13);
        AddSimple(OUR, "Miguel", "Prado", "Prado", 22, "ESP", 185, 85, RIGHT, GK, 0, 58, 68, 25000, 5, 250000, 4, 25);
        AddSimple(OUR, "Fran", "Carmona", "Carmona", 27, "ESP", 184, 77, RIGHT, CB, 0, 60, 66, 100000, 20, 1000000, 4, 4);
        AddSimple(OUR, "Aymane", "Jelbat", "Jelbat", 24, "MAR", 188, 78, RIGHT, CB, 0, 58, 66, 50000, 10, 500000, 4, 5);
        AddSimple(OUR, "Adrián", "Pérez", "A. Pérez", 26, "ESP", 180, 75, RIGHT, CB, LB_f, 58, 64, 75000, 15, 750000, 4, 14);
        AddSimple(OUR, "Hugo", "Sanz", "H. Sanz", 23, "ESP", 186, 78, RIGHT, CB, 0, 58, 66, 50000, 10, 500000, 4, 15);
        AddSimple(OUR, "Enol", "Coto", "Coto", 22, "ESP", 175, 70, RIGHT, RB, 0, 60, 70, 75000, 15, 750000, 4, 2);
        AddSimple(OUR, "Jerin", "Ramos", "Ramos", 25, "ESP", 183, 76, RIGHT, CB, CDM_f, 58, 64, 50000, 10, 500000, 4, 16);
        AddSimple(OUR, "Álvaro", "Yuste", "Yuste", 26, "ESP", 174, 67, RIGHT, LM, 0, 60, 66, 50000, 10, 500000, 4, 3);
        AddSimple(OUR, "David", "Muñoz", "D. Muñoz", 27, "ESP", 178, 71, RIGHT, CM, 0, 60, 66, 75000, 15, 750000, 4, 6);
        AddSimple(OUR, "David", "Rabadán", "Rabadán", 28, "ESP", 180, 72, RIGHT, CM, CDM_f, 60, 66, 75000, 15, 750000, 5, 8);
        AddSimple(OUR, "César", "Moreno", "C. Moreno", 29, "ESP", 176, 70, RIGHT, CM, 0, 58, 64, 50000, 10, 500000, 5, 10);
        AddSimple(OUR, "Lamine", "Gueye", "Gueye", 27, "SEN", 180, 70, RIGHT, LW, RW_f, 60, 66, 100000, 20, 1000000, 4, 11);
        AddSimple(OUR, "Nacho", "Castillo", "Castillo", 25, "ESP", 172, 68, RIGHT, CM, CAM_f, 58, 64, 50000, 10, 500000, 4, 18);
        AddSimple(OUR, "Iker", "Punzano", "Punzano", 23, "ESP", 178, 69, RIGHT, CAM, CF_f, 58, 66, 50000, 10, 500000, 4, 20);
        AddSimple(OUR, "Aitor", "Aranzabe", "Aranzabe", 26, "ESP", 170, 66, RIGHT, LW, 0, 60, 66, 75000, 15, 750000, 4, 7);
        AddSimple(OUR, "Luis", "Rivas", "Rivas", 27, "ESP", 178, 71, RIGHT, RB, 0, 58, 64, 50000, 10, 500000, 4, 17);
        AddSimple(OUR, "Adrián", "Guerrero", "Guerrero", 28, "ESP", 182, 76, RIGHT, ST, 0, 58, 64, 50000, 10, 500000, 5, 9);
        AddSimple(OUR, "Raúl", "Hernández", "R. Hernández", 26, "ESP", 185, 77, RIGHT, ST, 0, 60, 66, 75000, 15, 750000, 4, 12);
        AddSimple(OUR, "Amin", "Bouzaig", "Bouzaig", 24, "MAR", 180, 75, RIGHT, CF, 0, 60, 68, 50000, 10, 500000, 4, 21);
        AddSimple(OUR, "Martín", "Ochoa", "Ochoa", 23, "ESP", 179, 74, RIGHT, CF, 0, 58, 66, 50000, 10, 500000, 4, 22);
        AddSimple(OUR, "Sergio", "Benito", "Benito", 28, "ESP", 184, 77, RIGHT, ST, 0, 60, 66, 75000, 15, 750000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // CD Extremadura 1924 (TeamId=58)
        // ══════════════════════════════════════════════════════════
        int EXT = 58;
        AddSimple(EXT, "David", "Pinto", "Pinto", 29, "ESP", 195, 89, RIGHT, GK, 0, 60, 66, 100000, 20, 1000000, 5, 1);
        AddSimple(EXT, "Cristian", "Rodríguez", "C. Rodríguez", 22, "ESP", 190, 87, RIGHT, GK, 0, 58, 68, 50000, 10, 500000, 4, 13);
        AddSimple(EXT, "Carlos", "Cordero", "Cordero", 29, "ESP", 181, 76, RIGHT, CB, 0, 60, 66, 100000, 20, 1000000, 5, 4);
        AddSimple(EXT, "Ángel", "Cano", "Cano", 31, "ESP", 194, 81, RIGHT, CB, 0, 60, 64, 100000, 20, 1000000, 5, 5);
        AddSimple(EXT, "Alfonso", "Peral", "Peral", 21, "ESP", 190, 79, RIGHT, CB, 0, 58, 70, 25000, 5, 250000, 4, 14);
        AddSimple(EXT, "Tala", "", "Tala", 25, "ESP", 175, 70, RIGHT, LB, 0, 58, 64, 50000, 10, 500000, 4, 3);
        AddSimple(EXT, "Miguel", "Cera", "Cera", 29, "ESP", 172, 68, RIGHT, RB, 0, 60, 66, 100000, 20, 1000000, 5, 2);
        AddSimple(EXT, "Fran", "Rosales", "Rosales", 26, "ESP", 178, 71, RIGHT, RB, 0, 58, 64, 50000, 10, 500000, 4, 15);
        AddSimple(EXT, "Rober", "Correa", "Correa", 33, "ESP", 180, 72, RIGHT, RB, 0, 58, 62, 50000, 10, 500000, 5, 16);
        AddSimple(EXT, "Luis", "Morcillo", "Morcillo", 22, "ESP", 178, 71, RIGHT, CM, 0, 60, 72, 200000, 50, 2000000, 4, 6);
        AddSimple(EXT, "Giovanni", "Zarfino", "Zarfino", 34, "ESP", 185, 74, RIGHT, CM, 0, 60, 64, 150000, 30, 1500000, 5, 8);
        AddSimple(EXT, "Robe", "Moreno", "Moreno", 28, "ESP", 187, 74, RIGHT, CM, 0, 58, 64, 25000, 5, 250000, 5, 10);
        AddSimple(EXT, "Miguel", "Núñez", "Núñez", 39, "ESP", 180, 72, RIGHT, CM, 0, 56, 58, 25000, 5, 250000, 5, 18);
        AddSimple(EXT, "Manu", "Ramírez", "Ramírez", 30, "ESP", 178, 69, RIGHT, CAM, 0, 60, 64, 150000, 30, 1500000, 5, 11);
        AddSimple(EXT, "Óscar", "Pinchi", "Pinchi", 30, "ESP", 171, 66, RIGHT, LW, 0, 62, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(EXT, "Juanmi", "Callejón", "Callejón", 39, "ESP", 180, 70, RIGHT, LW, 0, 56, 58, 25000, 5, 250000, 5, 19);
        AddSimple(EXT, "Marco", "Manchón", "Manchón", 22, "ESP", 178, 69, RIGHT, RW, 0, 60, 72, 200000, 50, 2000000, 4, 12);
        AddSimple(EXT, "Imanol", "Barace", "Barace", 28, "ESP", 178, 69, RIGHT, RW, 0, 60, 66, 150000, 30, 1500000, 4, 20);
        AddSimple(EXT, "Diego", "Díaz", "D. Díaz", 31, "ESP", 174, 67, RIGHT, RW, 0, 58, 62, 100000, 20, 1000000, 5, 21);
        AddSimple(EXT, "Usama", "Arhoun", "Arhoun", 22, "MAR", 173, 67, RIGHT, RW, 0, 58, 68, 50000, 10, 500000, 4, 22);
        AddSimple(EXT, "Alfredo", "Sualdea", "Sualdea", 27, "ESP", 183, 76, RIGHT, CF, 0, 60, 66, 150000, 30, 1500000, 4, 9);
        AddSimple(EXT, "Maikel", "Villajos", "Villajos", 27, "ESP", 178, 69, RIGHT, CF, 0, 58, 64, 50000, 10, 500000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Bilbao Athletic (TeamId=59)
        // ══════════════════════════════════════════════════════════
        int BAT = 59;
        AddSimple(BAT, "Mikel", "Santos", "Santos", 21, "ESP", 188, 86, RIGHT, GK, 0, 58, 72, 75000, 15, 750000, 4, 1);
        AddSimple(BAT, "Eñaut", "Lete", "Lete", 19, "ESP", 191, 87, RIGHT, GK, 0, 56, 74, 50000, 10, 500000, 3, 13);
        AddSimple(BAT, "Iker", "Galindo", "Galindo", 20, "ESP", 186, 78, RIGHT, CB, 0, 58, 74, 100000, 20, 1000000, 3, 4);
        AddSimple(BAT, "Eric", "Gamen", "Gamen", 20, "ESP", 185, 77, RIGHT, CB, 0, 58, 74, 75000, 15, 750000, 3, 5);
        AddSimple(BAT, "Jon", "de Luis", "De Luis", 22, "ESP", 189, 79, RIGHT, CB, 0, 60, 72, 125000, 25, 1250000, 4, 14);
        AddSimple(BAT, "Aritz", "Conde", "Conde", 21, "ESP", 184, 77, RIGHT, CB, 0, 58, 72, 75000, 15, 750000, 3, 15);
        AddSimple(BAT, "Asier", "Hierro", "Hierro", 23, "ESP", 188, 78, RIGHT, CB, 0, 60, 70, 100000, 20, 1000000, 4, 16);
        AddSimple(BAT, "Iker", "Monreal", "Monreal", 19, "ESP", 175, 70, RIGHT, LB, 0, 58, 76, 100000, 20, 1000000, 3, 3);
        AddSimple(BAT, "Peio", "Huestamendia", "Huestamendia", 20, "ESP", 176, 70, RIGHT, LB, 0, 56, 72, 50000, 10, 500000, 3, 17);
        AddSimple(BAT, "Ander", "Izagirre", "Izagirre", 22, "ESP", 178, 71, RIGHT, RB, 0, 58, 70, 75000, 15, 750000, 4, 2);
        AddSimple(BAT, "Johaneko", "Louis-Jean", "Louis-Jean", 22, "FRA", 179, 71, RIGHT, RB, 0, 60, 72, 200000, 50, 2000000, 4, 18);
        AddSimple(BAT, "Adrián", "Pérez", "A. Pérez", 20, "ESP", 177, 70, RIGHT, RB, 0, 56, 72, 50000, 10, 500000, 3, 19);
        AddSimple(BAT, "Aimar", "Duñabeitia", "Duñabeitia", 21, "ESP", 181, 72, RIGHT, CDM, CM_f, 60, 72, 100000, 20, 1000000, 4, 6);
        AddSimple(BAT, "Manex", "Gibelalde", "Gibelalde", 20, "ESP", 180, 72, RIGHT, CDM, 0, 56, 72, 75000, 15, 750000, 3, 20);
        AddSimple(BAT, "Miguel", "Barandalla", "Barandalla", 21, "ESP", 180, 72, RIGHT, CM, 0, 58, 72, 100000, 20, 1000000, 4, 8);
        AddSimple(BAT, "Beñat", "García", "B. García", 23, "ESP", 183, 73, RIGHT, CM, 0, 60, 70, 150000, 30, 1500000, 4, 10);
        AddSimple(BAT, "Efe", "Korkut", "Korkut", 20, "ESP", 178, 71, RIGHT, CM, 0, 56, 72, 50000, 10, 500000, 3, 21);
        AddSimple(BAT, "Xabi", "Irurita", "Irurita", 22, "ESP", 174, 67, RIGHT, LM, 0, 56, 70, 50000, 10, 500000, 4, 11);
        AddSimple(BAT, "Iker", "Alday", "Alday", 20, "ESP", 176, 68, RIGHT, CAM, 0, 60, 76, 125000, 25, 1250000, 3, 22);
        AddSimple(BAT, "Ibai", "Sanz", "Sanz", 22, "ESP", 174, 67, RIGHT, CAM, 0, 58, 70, 75000, 15, 750000, 4, 23);
        AddSimple(BAT, "Selton", "Sánchez", "Sánchez", 22, "ESP", 173, 67, RIGHT, LW, RW_f, 60, 72, 175000, 43, 1750000, 4, 7);
        AddSimple(BAT, "Ibon", "Sánchez", "I. Sánchez", 21, "ESP", 171, 66, RIGHT, LW, 0, 60, 74, 125000, 25, 1250000, 4, 24);
        AddSimple(BAT, "Eder", "García", "E. García", 21, "ESP", 177, 69, RIGHT, RW, 0, 58, 72, 100000, 20, 1000000, 4, 12);
        AddSimple(BAT, "Elijah", "Gift", "Gift", 20, "ESP", 174, 67, RIGHT, RW, LW_f, 60, 76, 250000, 62, 2500000, 3, 25);
        AddSimple(BAT, "Javi", "Sola", "Sola", 22, "ESP", 185, 77, RIGHT, ST, 0, 60, 72, 100000, 20, 1000000, 4, 9);
        AddSimple(BAT, "Endika", "Buján", "Buján", 21, "ESP", 182, 76, RIGHT, ST, 0, 58, 72, 75000, 15, 750000, 4, 26);

        // ══════════════════════════════════════════════════════════
        // Arenas Club (TeamId=60)
        // ══════════════════════════════════════════════════════════
        int ARE = 60;
        AddSimple(ARE, "Oier", "Gastesi", "Gastesi", 23, "ESP", 187, 86, RIGHT, GK, 0, 58, 68, 50000, 10, 500000, 4, 1);
        AddSimple(ARE, "Óscar", "Bazaga", "Bazaga", 24, "ESP", 189, 86, RIGHT, GK, 0, 58, 68, 50000, 10, 500000, 4, 13);
        AddSimple(ARE, "Anartz", "Peña", "Peña", 24, "ESP", 185, 77, RIGHT, CB, 0, 58, 66, 75000, 15, 750000, 4, 4);
        AddSimple(ARE, "Sergio", "Aragoneses", "Aragoneses", 25, "ESP", 186, 78, RIGHT, CB, 0, 60, 68, 100000, 20, 1000000, 4, 5);
        AddSimple(ARE, "Jon", "Sillero", "Sillero", 24, "ESP", 184, 77, RIGHT, CB, 0, 60, 68, 100000, 20, 1000000, 4, 14);
        AddSimple(ARE, "Carlos", "Mattheus", "Mattheus", 25, "ESP", 187, 78, RIGHT, CB, 0, 60, 68, 100000, 20, 1000000, 4, 15);
        AddSimple(ARE, "Paul", "Álvarez", "Álvarez", 22, "ESP", 176, 70, RIGHT, LB, 0, 58, 68, 75000, 15, 750000, 4, 3);
        AddSimple(ARE, "Julen", "Lartitegi", "Lartitegi", 22, "ESP", 175, 70, RIGHT, LB, 0, 58, 70, 75000, 15, 750000, 4, 16);
        AddSimple(ARE, "Christian", "Mutilva", "Mutilva", 23, "ESP", 178, 71, RIGHT, RB, 0, 58, 68, 50000, 10, 500000, 4, 2);
        AddSimple(ARE, "Urko", "Collado", "Collado", 23, "ESP", 176, 70, RIGHT, RB, 0, 58, 68, 50000, 10, 500000, 4, 17);
        AddSimple(ARE, "Santi", "Borikó", "Borikó", 23, "ESP", 180, 72, RIGHT, CDM, CM_f, 58, 68, 75000, 15, 750000, 4, 6);
        AddSimple(ARE, "Sergio", "Navarro", "Navarro", 24, "ESP", 182, 72, RIGHT, CDM, 0, 58, 66, 75000, 15, 750000, 4, 18);
        AddSimple(ARE, "Pablo", "García", "P. García", 22, "ESP", 179, 71, RIGHT, CM, 0, 56, 68, 50000, 10, 500000, 4, 8);
        AddSimple(ARE, "Jon", "Merino", "Merino", 23, "ESP", 181, 72, RIGHT, CM, 0, 58, 68, 75000, 15, 750000, 4, 10);
        AddSimple(ARE, "Alex", "Hidalgo", "Hidalgo", 22, "ESP", 178, 71, RIGHT, CM, 0, 56, 68, 50000, 10, 500000, 4, 19);
        AddSimple(ARE, "Ander", "Sustatxa", "Sustatxa", 23, "ESP", 175, 68, RIGHT, CAM, 0, 58, 68, 75000, 15, 750000, 4, 20);
        AddSimple(ARE, "Txus", "Vizcay", "Vizcay", 25, "ESP", 173, 67, RIGHT, CAM, 0, 56, 64, 50000, 10, 500000, 5, 21);
        AddSimple(ARE, "Eneko", "Zabaleta", "Zabaleta", 24, "ESP", 173, 67, RIGHT, LM, 0, 56, 64, 50000, 10, 500000, 4, 11);
        AddSimple(ARE, "Adrián", "Verde", "Verde", 22, "ESP", 172, 67, RIGHT, LW, 0, 58, 70, 75000, 15, 750000, 4, 7);
        AddSimple(ARE, "Mathieu", "Seyram", "Seyram", 23, "FRA", 174, 67, RIGHT, LW, 0, 56, 66, 50000, 10, 500000, 4, 22);
        AddSimple(ARE, "Marcos", "Fernández", "M. Fernández", 25, "ESP", 177, 69, RIGHT, RW, 0, 60, 68, 100000, 20, 1000000, 4, 12);
        AddSimple(ARE, "Ángel", "Troncho", "Troncho", 24, "ESP", 180, 75, RIGHT, CF, ST_f, 58, 68, 75000, 15, 750000, 4, 23);
        AddSimple(ARE, "Mikel", "Zabala", "Zabala", 24, "ESP", 183, 76, RIGHT, ST, 0, 60, 68, 100000, 20, 1000000, 4, 9);
        AddSimple(ARE, "Tamir", "Glazer", "Glazer", 23, "ISR", 186, 78, RIGHT, ST, 0, 56, 66, 50000, 10, 500000, 4, 24);
        AddSimple(ARE, "Álvaro", "Vázquez", "Á. Vázquez", 25, "ESP", 184, 77, RIGHT, ST, 0, 60, 68, 100000, 20, 1000000, 5, 25);

        // ══════════════════════════════════════════════════════════
        // CD Coria (TeamId=61)
        // ══════════════════════════════════════════════════════════
        int CRI = 61;
        AddSimple(CRI, "Aarón", "Alonso", "Alonso", 25, "ESP", 186, 85, RIGHT, GK, 0, 58, 66, 50000, 10, 500000, 4, 1);
        AddSimple(CRI, "Eduardo", "Loscos", "Loscos", 24, "ESP", 188, 86, RIGHT, GK, 0, 58, 66, 50000, 10, 500000, 4, 13);
        AddSimple(CRI, "Iñaki", "León", "León", 26, "ESP", 185, 77, RIGHT, CB, 0, 58, 64, 75000, 15, 750000, 4, 4);
        AddSimple(CRI, "Álvaro", "Domínguez", "Á. Domínguez", 25, "ESP", 184, 77, RIGHT, CB, 0, 56, 64, 50000, 10, 500000, 4, 5);
        AddSimple(CRI, "Gonzalo", "Llerena", "Llerena", 24, "ESP", 186, 78, RIGHT, CB, 0, 58, 66, 75000, 15, 750000, 4, 14);
        AddSimple(CRI, "Antonio", "Hernández", "A. Hernández", 25, "ESP", 188, 78, RIGHT, CB, 0, 58, 66, 75000, 15, 750000, 4, 15);
        AddSimple(CRI, "Hugo", "López", "H. López", 23, "ESP", 174, 69, RIGHT, LB, 0, 58, 68, 50000, 10, 500000, 4, 3);
        AddSimple(CRI, "Marcos", "Bautista", "Bautista", 24, "ESP", 177, 70, RIGHT, RB, 0, 58, 66, 50000, 10, 500000, 4, 2);
        AddSimple(CRI, "Juanjo", "Chavalés", "Chavalés", 25, "ESP", 176, 70, RIGHT, RB, 0, 58, 66, 50000, 10, 500000, 4, 16);
        AddSimple(CRI, "Jacobo", "Guzmán", "Guzmán", 25, "ESP", 180, 72, RIGHT, CDM, 0, 58, 66, 50000, 10, 500000, 4, 6);
        AddSimple(CRI, "Javi", "Tapia", "Tapia", 25, "ESP", 181, 72, RIGHT, CM, 0, 58, 64, 50000, 10, 500000, 4, 8);
        AddSimple(CRI, "Eric", "Iglesias", "Iglesias", 23, "ESP", 180, 72, RIGHT, CM, 0, 58, 68, 50000, 10, 500000, 4, 10);
        AddSimple(CRI, "Álex", "Toper", "Toper", 24, "ESP", 179, 71, RIGHT, CM, 0, 56, 64, 50000, 10, 500000, 4, 18);
        AddSimple(CRI, "José", "Currás", "Currás", 24, "ESP", 178, 71, RIGHT, CM, 0, 56, 64, 50000, 10, 500000, 4, 19);
        AddSimple(CRI, "Benji", "Núñez", "Núñez", 23, "ESP", 176, 68, RIGHT, CAM, 0, 58, 68, 75000, 15, 750000, 4, 20);
        AddSimple(CRI, "Sergio", "Gómez", "S. Gómez", 23, "ESP", 172, 67, RIGHT, LW, 0, 58, 68, 75000, 15, 750000, 4, 11);
        AddSimple(CRI, "Dawda", "Dambelleh", "Dambelleh", 23, "GAM", 171, 66, RIGHT, LW, 0, 56, 66, 50000, 10, 500000, 4, 7);
        AddSimple(CRI, "Adri", "Pérez", "Adri P.", 24, "ESP", 175, 68, RIGHT, RW, 0, 58, 66, 50000, 10, 500000, 4, 12);
        AddSimple(CRI, "Adrià", "Mercadal", "Mercadal", 24, "ESP", 174, 67, RIGHT, CAM, 0, 58, 66, 50000, 10, 500000, 4, 21);
        AddSimple(CRI, "José Antonio", "Martín", "J. A. Martín", 26, "ESP", 184, 77, RIGHT, ST, 0, 60, 66, 100000, 20, 1000000, 4, 9);
        AddSimple(CRI, "Domingo", "Mba", "Mba", 24, "EQG", 183, 76, RIGHT, ST, 0, 58, 66, 50000, 10, 500000, 4, 22);
        AddSimple(CRI, "Tommy", "Fernández", "T. Fernández", 23, "ESP", 182, 76, RIGHT, ST, 0, 58, 68, 75000, 15, 750000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Fabril Deportivo (TeamId=62)
        // ══════════════════════════════════════════════════════════
        int FAB = 62;
        AddSimple(FAB, "Hugo", "Rios", "Rios", 20, "ESP", 187, 86, RIGHT, GK, 0, 56, 72, 50000, 10, 500000, 3, 1);
        AddSimple(FAB, "Brais", "Suárez", "B. Suárez", 21, "ESP", 189, 86, RIGHT, GK, 0, 56, 70, 50000, 10, 500000, 4, 13);
        AddSimple(FAB, "Álex", "Marqués", "Marqués", 20, "ESP", 185, 77, RIGHT, CB, 0, 58, 74, 75000, 15, 750000, 3, 4);
        AddSimple(FAB, "Samu", "Fernández", "S. Fernández", 21, "ESP", 186, 78, RIGHT, CB, 0, 58, 72, 75000, 15, 750000, 4, 5);
        AddSimple(FAB, "Damián", "Canedo", "Canedo", 22, "ESP", 184, 77, RIGHT, CB, 0, 56, 68, 50000, 10, 500000, 4, 14);
        AddSimple(FAB, "Jaime", "Garrido", "Garrido", 22, "ESP", 186, 78, RIGHT, CB, 0, 58, 70, 75000, 15, 750000, 4, 15);
        AddSimple(FAB, "David", "Domínguez", "D. Domínguez", 23, "ESP", 185, 77, RIGHT, CB, 0, 58, 68, 75000, 15, 750000, 4, 16);
        AddSimple(FAB, "Manu", "Ferreiro", "Ferreiro", 21, "ESP", 176, 70, RIGHT, LB, 0, 56, 70, 50000, 10, 500000, 4, 3);
        AddSimple(FAB, "Rubén", "Vilela", "Vilela", 21, "ESP", 177, 70, RIGHT, RB, 0, 56, 70, 50000, 10, 500000, 4, 2);
        AddSimple(FAB, "Iker", "Vidal", "Vidal", 20, "ESP", 175, 70, RIGHT, RB, 0, 56, 72, 50000, 10, 500000, 3, 17);
        AddSimple(FAB, "Aarón", "Sánchez", "A. Sánchez", 21, "ESP", 180, 72, RIGHT, CDM, 0, 56, 70, 50000, 10, 500000, 4, 6);
        AddSimple(FAB, "Malick", "Ndiaye", "Ndiaye", 22, "SEN", 182, 72, RIGHT, CDM, CM_f, 58, 70, 75000, 15, 750000, 4, 18);
        AddSimple(FAB, "Fabián", "Urzain", "Urzain", 23, "ESP", 181, 72, RIGHT, CDM, 0, 56, 66, 50000, 10, 500000, 4, 19);
        AddSimple(FAB, "Hugo", "Torres", "H. Torres", 21, "ESP", 179, 71, RIGHT, CM, 0, 56, 70, 50000, 10, 500000, 4, 8);
        AddSimple(FAB, "Pablo", "García", "P. García", 20, "ESP", 178, 71, RIGHT, CM, 0, 56, 72, 50000, 10, 500000, 3, 10);
        AddSimple(FAB, "Dani", "Estévez", "Estévez", 20, "ESP", 180, 72, RIGHT, CM, 0, 56, 72, 50000, 10, 500000, 3, 20);
        AddSimple(FAB, "Quique", "Teijo", "Teijo", 22, "ESP", 176, 68, RIGHT, CAM, 0, 58, 70, 75000, 15, 750000, 4, 21);
        AddSimple(FAB, "Mario", "Nájera", "Nájera", 22, "ESP", 175, 68, RIGHT, CAM, 0, 56, 68, 50000, 10, 500000, 4, 22);
        AddSimple(FAB, "Quique", "Fernández", "Q. Fernández", 21, "ESP", 174, 67, RIGHT, RW, 0, 56, 70, 50000, 10, 500000, 4, 7);
        AddSimple(FAB, "Mario", "Hermo", "Hermo", 21, "ESP", 172, 67, RIGHT, LW, 0, 56, 72, 50000, 10, 500000, 4, 11);
        AddSimple(FAB, "Alioune", "Mané", "Mané", 22, "SEN", 173, 67, RIGHT, LW, RW_f, 58, 70, 75000, 15, 750000, 4, 23);
        AddSimple(FAB, "Noé", "Carrillo", "Carrillo", 22, "ESP", 183, 76, RIGHT, ST, 0, 58, 70, 75000, 15, 750000, 4, 9);
        AddSimple(FAB, "Papa Samsou", "Niang", "Niang", 23, "SEN", 185, 77, RIGHT, ST, 0, 58, 68, 75000, 15, 750000, 4, 12);
        AddSimple(FAB, "Pablo", "Cortés", "Cortés", 21, "ESP", 184, 77, RIGHT, ST, 0, 56, 72, 50000, 10, 500000, 3, 24);
        AddSimple(FAB, "Héctor", "Areosa", "Areosa", 22, "ESP", 182, 76, RIGHT, ST, 0, 56, 68, 50000, 10, 500000, 4, 25);
        AddSimple(FAB, "Rodrigue", "Dipanda", "Dipanda", 24, "CMR", 188, 78, RIGHT, ST, 0, 58, 68, 75000, 15, 750000, 4, 26);

        // ══════════════════════════════════════════════════════════
        // PRIMERA REF - GRUPO II (LeagueId=4) — 10 equipos
        // ══════════════════════════════════════════════════════════

        // Real Zaragoza (TeamId=63)
        // ══════════════════════════════════════════════════════════
        int ZAR = 63;
        AddSimple(ZAR, "Pablo", "Cuesta", "Cuesta", 27, "ESP", 188, 86, RIGHT, GK, 0, 68, 72, 2500000, 834, 25000000, 5, 1);
        AddSimple(ZAR, "Álvaro", "Ratón", "Ratón", 31, "ESP", 186, 85, RIGHT, GK, 0, 65, 68, 1200000, 400, 12000000, 5, 13);
        AddSimple(ZAR, "Andrés", "Prieto", "Prieto", 33, "ESP", 194, 89, RIGHT, GK, 0, 62, 64, 300000, 75, 3000000, 5, 25);
        AddSimple(ZAR, "Jair", "Amador", "Amador", 35, "ESP", 192, 80, RIGHT, CB, 0, 69, 70, 1500000, 500, 15000000, 5, 4);
        AddSimple(ZAR, "Lluís", "López", "L. López", 29, "ESP", 185, 77, RIGHT, CB, 0, 67, 70, 1400000, 466, 14000000, 5, 5);
        AddSimple(ZAR, "Alejandro", "Francés", "Francés", 25, "ESP", 181, 76, RIGHT, CB, LB_f, 64, 70, 1000000, 333, 10000000, 4, 14);
        AddSimple(ZAR, "Sergio", "Postigo", "Postigo", 38, "ESP", 184, 77, RIGHT, CB, 0, 60, 60, 75000, 15, 750000, 5, 15);
        AddSimple(ZAR, "Andrés", "Borge", "Borge", 28, "ESP", 175, 70, RIGHT, LB, CB_f, 66, 70, 1500000, 500, 15000000, 5, 3);
        AddSimple(ZAR, "Marcos", "Luna", "Luna", 26, "ESP", 177, 70, RIGHT, RB, 0, 64, 68, 800000, 228, 8000000, 4, 2);
        AddSimple(ZAR, "Marc", "Aguado", "Aguado", 30, "ESP", 178, 71, RIGHT, CDM, CM_f, 68, 70, 1800000, 600, 18000000, 5, 6);
        AddSimple(ZAR, "Francho", "Gracia", "Francho", 25, "ESP", 180, 72, RIGHT, CDM, CM_f, 64, 70, 800000, 228, 8000000, 4, 18);
        AddSimple(ZAR, "Alberto", "Zapater", "Zapater", 41, "ESP", 185, 74, RIGHT, CM, CDM_f, 66, 66, 500000, 142, 5000000, 5, 8);
        AddSimple(ZAR, "Manu", "Molina", "Molina", 33, "ESP", 180, 72, RIGHT, CM, CAM_f, 66, 68, 600000, 171, 6000000, 5, 10);
        AddSimple(ZAR, "Jaume", "Grau", "Grau", 28, "ESP", 178, 71, RIGHT, CM, 0, 64, 68, 600000, 171, 6000000, 5, 19);
        AddSimple(ZAR, "Adrián", "Liso", "Liso", 25, "ESP", 177, 70, RIGHT, CM, RW_f, 63, 68, 500000, 142, 5000000, 4, 20);
        AddSimple(ZAR, "Ager", "Aketxe", "Aketxe", 32, "ESP", 174, 69, RIGHT, CAM, CM_f, 70, 72, 3000000, 1000, 30000000, 5, 11);
        AddSimple(ZAR, "Malcom", "Bokele", "Bokele", 25, "CMR", 183, 73, RIGHT, CAM, LW_f, 65, 70, 1000000, 333, 10000000, 4, 21);
        AddSimple(ZAR, "Iván", "Azón", "Azón", 23, "ESP", 176, 68, RIGHT, LW, ST_f, 68, 74, 3500000, 1166, 35000000, 4, 7);
        AddSimple(ZAR, "Mario", "Soberón", "Soberón", 29, "ESP", 180, 70, RIGHT, RW, LW_f, 67, 70, 1500000, 500, 15000000, 5, 12);
        AddSimple(ZAR, "Víctor", "Mollejo", "Mollejo", 27, "ESP", 176, 68, RIGHT, LW, RW_f, 65, 68, 1000000, 333, 10000000, 4, 22);
        AddSimple(ZAR, "Maikel", "Mesa", "Mesa", 35, "ESP", 175, 73, RIGHT, ST, 0, 70, 70, 2000000, 666, 20000000, 5, 9);
        AddSimple(ZAR, "Sergio", "Enrich", "Enrich", 35, "ESP", 182, 76, RIGHT, ST, LW_f, 68, 68, 1500000, 500, 15000000, 5, 23);
        AddSimple(ZAR, "Álex", "Varela", "Varela", 22, "ESP", 184, 77, RIGHT, CF, ST_f, 67, 74, 2500000, 834, 25000000, 4, 24);
        AddSimple(ZAR, "Mario", "Riquelme", "Riquelme", 28, "ESP", 178, 69, RIGHT, CF, LW_f, 66, 70, 1200000, 400, 12000000, 5, 16);

        // ══════════════════════════════════════════════════════════
        // SD Huesca (TeamId=64)
        // ══════════════════════════════════════════════════════════
        int HUE = 64;
        AddSimple(HUE, "Álvaro", "Fernández", "Á. Fernández", 27, "ESP", 185, 85, RIGHT, GK, 0, 68, 72, 2200000, 734, 22000000, 5, 1);
        AddSimple(HUE, "Juan", "Pérez", "J. Pérez", 25, "ESP", 188, 86, RIGHT, GK, 0, 64, 68, 800000, 228, 8000000, 4, 13);
        AddSimple(HUE, "Ignacio", "Álvarez", "Álvarez", 22, "ESP", 190, 87, RIGHT, GK, 0, 62, 72, 500000, 142, 5000000, 4, 25);
        AddSimple(HUE, "Rubén", "Blasco", "Blasco", 27, "ESP", 183, 76, RIGHT, CB, 0, 68, 72, 2000000, 666, 20000000, 5, 4);
        AddSimple(HUE, "Mikel", "Villanueva", "Villanueva", 31, "ESP", 186, 78, RIGHT, CB, 0, 66, 68, 1000000, 333, 10000000, 5, 5);
        AddSimple(HUE, "Jeremy", "Blasco", "J. Blasco", 26, "FRA", 184, 77, RIGHT, CB, LB_f, 64, 68, 800000, 228, 8000000, 4, 14);
        AddSimple(HUE, "Rubén", "Pulido", "Pulido", 25, "ESP", 189, 79, RIGHT, CB, 0, 63, 68, 600000, 171, 6000000, 4, 15);
        AddSimple(HUE, "Miguel", "Cuesta", "M. Cuesta", 26, "ESP", 178, 71, RIGHT, LB, 0, 65, 68, 800000, 228, 8000000, 4, 3);
        AddSimple(HUE, "Jorge", "Pulido", "J. Pulido", 34, "ESP", 174, 69, RIGHT, LB, CB_f, 62, 64, 300000, 75, 3000000, 5, 16);
        AddSimple(HUE, "Gerard", "Valentín", "Valentín", 31, "ESP", 178, 71, RIGHT, RB, 0, 66, 68, 1200000, 400, 12000000, 5, 2);
        AddSimple(HUE, "Paco", "Sanz", "Sanz", 27, "ESP", 176, 70, RIGHT, RB, 0, 62, 66, 400000, 100, 4000000, 4, 17);
        AddSimple(HUE, "Cristian", "Gutiérrez", "C. Gutiérrez", 32, "ESP", 184, 73, RIGHT, CDM, CB_f, 67, 68, 1200000, 400, 12000000, 5, 6);
        AddSimple(HUE, "Marc", "Roca", "M. Roca", 22, "ESP", 180, 72, RIGHT, CDM, CM_f, 63, 72, 800000, 228, 8000000, 4, 18);
        AddSimple(HUE, "Javi", "Galán", "Galán", 30, "ESP", 178, 71, RIGHT, CM, CDM_f, 68, 70, 1800000, 600, 18000000, 5, 8);
        AddSimple(HUE, "Pablo", "Martínez", "P. Martínez", 26, "ESP", 182, 72, RIGHT, CM, 0, 65, 70, 1000000, 333, 10000000, 5, 10);
        AddSimple(HUE, "Álex", "García", "Á. García", 28, "ESP", 179, 71, RIGHT, CM, CAM_f, 64, 68, 800000, 228, 8000000, 5, 19);
        AddSimple(HUE, "David", "Vila", "Vila", 23, "ESP", 175, 70, RIGHT, CM, 0, 60, 70, 400000, 100, 4000000, 4, 20);
        AddSimple(HUE, "Joaquín", "Muñoz", "Muñoz", 27, "ESP", 176, 68, RIGHT, CAM, CM_f, 66, 70, 1200000, 400, 12000000, 5, 11);
        AddSimple(HUE, "Samuel", "Obeng", "Obeng", 28, "GHA", 185, 74, RIGHT, LW, ST_f, 67, 70, 1500000, 500, 15000000, 5, 7);
        AddSimple(HUE, "Juan", "Pinto", "Pinto", 27, "CHI", 178, 69, RIGHT, RW, LW_f, 66, 70, 1200000, 400, 12000000, 5, 12);
        AddSimple(HUE, "Eladio", "Zorrilla", "Zorrilla", 24, "ESP", 176, 68, RIGHT, LW, RW_f, 63, 68, 500000, 142, 5000000, 4, 21);
        AddSimple(HUE, "Patrick", "Soko", "Soko", 28, "NGA", 181, 70, RIGHT, ST, LW_f, 68, 70, 2000000, 666, 20000000, 5, 9);
        AddSimple(HUE, "Diego", "Gómez", "D. Gómez", 24, "ESP", 183, 76, RIGHT, ST, CF_f, 65, 72, 1200000, 400, 12000000, 4, 22);
        AddSimple(HUE, "Iker", "Unzueta", "Unzueta", 26, "ESP", 184, 77, RIGHT, ST, 0, 64, 68, 800000, 228, 8000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // FC Cartagena (TeamId=65)
        // ══════════════════════════════════════════════════════════
        int CAR = 65;
        AddSimple(CAR, "Toni", "Fuidias", "Fuidias", 24, "ESP", 192, 88, RIGHT, GK, 0, 66, 72, 1500000, 500, 15000000, 4, 1);
        AddSimple(CAR, "Juan", "Soberón", "J. Soberón", 25, "ESP", 185, 85, RIGHT, GK, 0, 63, 68, 500000, 142, 5000000, 4, 13);
        AddSimple(CAR, "Pablo", "Cuñat", "Cuñat", 22, "ESP", 190, 87, RIGHT, GK, 0, 61, 72, 300000, 75, 3000000, 4, 25);
        AddSimple(CAR, "Arnau", "Solà", "Solà", 24, "ESP", 186, 78, RIGHT, CB, 0, 66, 72, 1500000, 500, 15000000, 4, 4);
        AddSimple(CAR, "Kiko", "Mar", "Kiko Mar", 30, "ESP", 183, 76, RIGHT, CB, 0, 65, 68, 800000, 228, 8000000, 5, 5);
        AddSimple(CAR, "Nikola", "Šipčić", "Šipčić", 30, "SRB", 189, 79, RIGHT, CB, 0, 63, 66, 500000, 142, 5000000, 5, 14);
        AddSimple(CAR, "Lautaro", "Leone", "Leone", 27, "ARG", 186, 78, RIGHT, CB, 0, 62, 66, 400000, 100, 4000000, 4, 15);
        AddSimple(CAR, "Martín", "Aguirregabiria", "Aguirregabiria", 28, "ESP", 178, 71, RIGHT, LB, RB_f, 65, 68, 1000000, 333, 10000000, 5, 3);
        AddSimple(CAR, "Daniel", "Vázquez", "D. Vázquez", 23, "ESP", 177, 70, RIGHT, LB, 0, 62, 70, 500000, 142, 5000000, 4, 16);
        AddSimple(CAR, "Iván", "Mourelo", "Mourelo", 27, "ESP", 176, 70, RIGHT, RB, 0, 64, 68, 800000, 228, 8000000, 4, 2);
        AddSimple(CAR, "Tomás", "Alarcón", "Alarcón", 26, "CHI", 182, 72, RIGHT, CDM, CM_f, 65, 68, 800000, 228, 8000000, 5, 6);
        AddSimple(CAR, "Damián", "Petroff", "Petroff", 23, "ARG", 184, 73, RIGHT, CDM, 0, 62, 70, 500000, 142, 5000000, 4, 18);
        AddSimple(CAR, "Mikel", "Rico", "Rico", 39, "ESP", 179, 71, RIGHT, CM, 0, 63, 64, 300000, 75, 3000000, 5, 8);
        AddSimple(CAR, "Andy", "Rodríguez", "Rodríguez", 24, "ESP", 178, 71, RIGHT, CM, 0, 64, 70, 600000, 171, 6000000, 4, 10);
        AddSimple(CAR, "Javier", "Mecerreyes", "Mecerreyes", 25, "ESP", 183, 73, RIGHT, CM, CAM_f, 62, 68, 400000, 100, 4000000, 4, 19);
        AddSimple(CAR, "Marcos", "Corredera", "Corredera", 23, "ESP", 175, 70, RIGHT, CAM, CM_f, 63, 72, 800000, 228, 8000000, 4, 20);
        AddSimple(CAR, "Manolo", "Gómez", "M. Gómez", 22, "ESP", 176, 68, RIGHT, CAM, 0, 60, 70, 300000, 75, 3000000, 4, 21);
        AddSimple(CAR, "Álex", "Martínez", "Á. Martínez", 32, "ESP", 180, 70, RIGHT, LW, RW_f, 67, 68, 1500000, 500, 15000000, 5, 11);
        AddSimple(CAR, "Miki", "Muñoz", "M. Muñoz", 25, "ESP", 175, 68, RIGHT, RW, 0, 64, 68, 1000000, 333, 10000000, 4, 7);
        AddSimple(CAR, "Francisco", "Arencibia", "Arencibia", 23, "ESP", 174, 67, RIGHT, LW, RW_f, 62, 70, 500000, 142, 5000000, 4, 22);
        AddSimple(CAR, "Alfredo", "Ortuño", "Ortuño", 34, "ESP", 182, 76, RIGHT, ST, 0, 66, 68, 1200000, 400, 12000000, 5, 9);
        AddSimple(CAR, "Diego", "Rodríguez", "D. Rodríguez", 23, "ESP", 185, 77, RIGHT, ST, 0, 64, 72, 800000, 228, 8000000, 4, 12);
        AddSimple(CAR, "Lucho", "Vega", "Vega", 24, "ESP", 180, 75, RIGHT, ST, LW_f, 63, 70, 600000, 171, 6000000, 4, 23);
        AddSimple(CAR, "Juan", "Pedro", "Juan Pe.", 29, "ESP", 184, 77, RIGHT, CF, 0, 65, 68, 1000000, 333, 10000000, 5, 24);

        // ══════════════════════════════════════════════════════════
        // Real Murcia CF (TeamId=66)
        // ══════════════════════════════════════════════════════════
        int MUR = 66;
        AddSimple(MUR, "Pablo", "Cuñat", "P. Cuñat", 25, "ESP", 186, 85, RIGHT, GK, 0, 66, 70, 1200000, 400, 12000000, 4, 1);
        AddSimple(MUR, "Manuel", "García", "M. García", 24, "ESP", 188, 86, RIGHT, GK, 0, 63, 68, 400000, 100, 4000000, 4, 13);
        AddSimple(MUR, "Juan", "Carlos", "J. Carlos", 36, "ESP", 185, 85, RIGHT, GK, 0, 60, 62, 100000, 20, 1000000, 5, 25);
        AddSimple(MUR, "José", "Ruiz", "J. Ruiz", 27, "ESP", 185, 77, RIGHT, CB, 0, 66, 70, 1200000, 400, 12000000, 5, 4);
        AddSimple(MUR, "Alberto", "González", "A. González", 28, "ESP", 184, 77, RIGHT, CB, 0, 64, 68, 800000, 228, 8000000, 5, 5);
        AddSimple(MUR, "Pablo", "Larrea", "Larrea", 31, "ESP", 190, 79, RIGHT, CB, CDM_f, 63, 66, 400000, 100, 4000000, 5, 14);
        AddSimple(MUR, "Marc", "Forner", "Forner", 23, "ESP", 186, 78, RIGHT, CB, 0, 62, 72, 500000, 142, 5000000, 4, 15);
        AddSimple(MUR, "Alex", "Martínez", "A. Martínez", 26, "ESP", 175, 70, RIGHT, LB, LW_f, 64, 68, 800000, 228, 8000000, 4, 3);
        AddSimple(MUR, "Antonio", "Crespo", "Crespo", 22, "ESP", 178, 71, RIGHT, LB, 0, 60, 70, 200000, 50, 2000000, 4, 16);
        AddSimple(MUR, "Juan", "Bravo", "Bravo", 27, "ESP", 179, 71, RIGHT, RB, 0, 65, 68, 1000000, 333, 10000000, 4, 2);
        AddSimple(MUR, "Iván", "Pérez", "I. Pérez", 24, "ESP", 177, 70, RIGHT, CB, RB_f, 62, 68, 300000, 75, 3000000, 4, 17);
        AddSimple(MUR, "Sergio", "Tejera", "Tejera", 36, "ESP", 180, 72, RIGHT, CDM, CM_f, 64, 66, 400000, 100, 4000000, 5, 6);
        AddSimple(MUR, "Moha", "Slimane", "Slimane", 25, "ESP", 184, 73, RIGHT, CDM, 0, 62, 68, 400000, 100, 4000000, 4, 18);
        AddSimple(MUR, "Daniel", "Giménez", "Giménez", 28, "ESP", 180, 72, RIGHT, CM, 0, 64, 68, 600000, 171, 6000000, 5, 8);
        AddSimple(MUR, "Álvaro", "Sanmartín", "Sanmartín", 24, "ESP", 178, 71, RIGHT, CM, CAM_f, 63, 70, 500000, 142, 5000000, 4, 10);
        AddSimple(MUR, "Imanol", "Sánchez", "I. Sánchez", 26, "ESP", 182, 72, RIGHT, CM, CDM_f, 62, 66, 300000, 75, 3000000, 4, 19);
        AddSimple(MUR, "Jorge", "Mier", "Mier", 23, "ESP", 180, 72, RIGHT, CAM, CM_f, 64, 72, 800000, 228, 8000000, 4, 11);
        AddSimple(MUR, "Dani", "Vega", "D. Vega", 23, "ESP", 175, 68, RIGHT, CAM, 0, 62, 70, 500000, 142, 5000000, 4, 20);
        AddSimple(MUR, "Pablo", "García", "P. García", 26, "ESP", 176, 68, RIGHT, LW, RW_f, 66, 70, 1200000, 400, 12000000, 5, 7);
        AddSimple(MUR, "José", "Ángel", "J. Ángel", 21, "ESP", 174, 67, RIGHT, RW, LW_f, 60, 72, 300000, 75, 3000000, 4, 21);
        AddSimple(MUR, "David", "Tosic", "Tosic", 25, "SRB", 180, 70, RIGHT, RW, 0, 63, 68, 600000, 171, 6000000, 4, 12);
        AddSimple(MUR, "Jordi", "Marchena", "Marchena", 31, "ESP", 182, 76, RIGHT, ST, 0, 66, 68, 1000000, 333, 10000000, 5, 9);
        AddSimple(MUR, "Kennedy", "Segundo", "Segundo", 24, "NGA", 183, 76, RIGHT, ST, 0, 64, 70, 800000, 228, 8000000, 4, 22);
        AddSimple(MUR, "Cristian", "López", "C. López", 27, "ESP", 180, 75, RIGHT, ST, 0, 62, 66, 400000, 100, 4000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // AD Alcorcón (TeamId=67)
        // ══════════════════════════════════════════════════════════
        int ALC = 67;
        AddSimple(ALC, "Jesús", "Ruiz", "Jesús R.", 29, "ESP", 191, 87, RIGHT, GK, 0, 65, 68, 800000, 228, 8000000, 5, 1);
        AddSimple(ALC, "Lucas", "Anacker", "Anacker", 22, "GER", 194, 89, RIGHT, GK, 0, 62, 72, 400000, 100, 4000000, 4, 13);
        AddSimple(ALC, "Víctor", "López", "V. López", 24, "ESP", 186, 85, RIGHT, GK, 0, 60, 68, 200000, 50, 2000000, 4, 25);
        AddSimple(ALC, "Imanol", "García", "I. García", 26, "ESP", 185, 77, RIGHT, CB, 0, 65, 68, 800000, 228, 8000000, 5, 4);
        AddSimple(ALC, "Javi", "Castro", "Castro", 25, "ESP", 184, 77, RIGHT, CB, 0, 63, 68, 500000, 142, 5000000, 4, 5);
        AddSimple(ALC, "Esteban", "Burgos", "Burgos", 34, "ARG", 183, 76, RIGHT, CB, 0, 62, 64, 300000, 75, 3000000, 5, 14);
        AddSimple(ALC, "Pablo", "Tripodi", "Tripodi", 23, "ARG", 186, 78, RIGHT, CB, 0, 60, 70, 200000, 50, 2000000, 4, 15);
        AddSimple(ALC, "José", "León", "León", 29, "ESP", 176, 70, RIGHT, LB, CB_f, 64, 68, 800000, 228, 8000000, 5, 3);
        AddSimple(ALC, "Pablo", "Clemente", "Clemente", 25, "ESP", 175, 70, RIGHT, LB, 0, 60, 66, 200000, 50, 2000000, 4, 16);
        AddSimple(ALC, "Carlos", "Giménez", "C. Giménez", 28, "ESP", 178, 71, RIGHT, RB, 0, 64, 68, 600000, 171, 6000000, 5, 2);
        AddSimple(ALC, "Kike", "Ríos", "Ríos", 23, "ESP", 177, 70, RIGHT, RB, 0, 60, 68, 200000, 50, 2000000, 4, 17);
        AddSimple(ALC, "Juan", "Poveda", "Poveda", 30, "ESP", 182, 72, RIGHT, CDM, CM_f, 65, 68, 1000000, 333, 10000000, 5, 6);
        AddSimple(ALC, "Álex", "Mula", "Mula", 28, "ESP", 177, 70, RIGHT, CDM, 0, 62, 66, 300000, 75, 3000000, 5, 18);
        AddSimple(ALC, "Iker", "Bilbao", "Bilbao", 23, "ESP", 180, 72, RIGHT, CM, CAM_f, 63, 72, 600000, 171, 6000000, 4, 8);
        AddSimple(ALC, "Dani", "Tasende", "Tasende", 25, "ESP", 179, 71, RIGHT, CM, 0, 62, 68, 400000, 100, 4000000, 4, 10);
        AddSimple(ALC, "Antonio", "García", "A. García", 24, "ESP", 178, 71, RIGHT, CM, 0, 60, 68, 200000, 50, 2000000, 4, 19);
        AddSimple(ALC, "Andrés", "Martín", "A. Martín", 26, "ESP", 175, 68, RIGHT, CAM, CM_f, 64, 68, 800000, 228, 8000000, 5, 11);
        AddSimple(ALC, "Javi", "Díaz", "J. Díaz", 22, "ESP", 176, 68, RIGHT, CAM, 0, 60, 72, 300000, 75, 3000000, 4, 20);
        AddSimple(ALC, "Joel", "García", "Joel G.", 28, "ESP", 180, 70, RIGHT, LW, RW_f, 64, 68, 800000, 228, 8000000, 5, 7);
        AddSimple(ALC, "Alberto", "Quiles", "Quiles", 29, "ESP", 183, 71, RIGHT, RW, LW_f, 64, 68, 800000, 228, 8000000, 5, 12);
        AddSimple(ALC, "Marc", "Llinares", "Llinares", 24, "ESP", 176, 68, RIGHT, LW, 0, 60, 66, 200000, 50, 2000000, 4, 21);
        AddSimple(ALC, "José", "López", "J. López", 27, "ESP", 181, 76, RIGHT, ST, 0, 65, 68, 1000000, 333, 10000000, 5, 9);
        AddSimple(ALC, "Diego", "Suárez", "D. Suárez", 24, "ESP", 183, 76, RIGHT, ST, CF_f, 63, 70, 600000, 171, 6000000, 4, 22);
        AddSimple(ALC, "Carlos", "López", "C. López", 26, "ESP", 180, 75, RIGHT, ST, 0, 62, 66, 400000, 100, 4000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Real Madrid Castilla (TeamId=68)
        // ══════════════════════════════════════════════════════════
        int RMC = 68;
        AddSimple(RMC, "Mario", "de Luis", "De Luis", 22, "ESP", 188, 86, RIGHT, GK, 0, 62, 74, 800000, 228, 8000000, 4, 1);
        AddSimple(RMC, "Álex", "González", "Á. González", 21, "ESP", 190, 87, RIGHT, GK, 0, 60, 74, 500000, 142, 5000000, 3, 13);
        AddSimple(RMC, "Sergio", "Mestre", "Mestre", 20, "ESP", 185, 85, RIGHT, GK, 0, 58, 74, 300000, 75, 3000000, 3, 25);
        AddSimple(RMC, "Raúl", "Asencio", "Asencio", 22, "ESP", 185, 77, RIGHT, CB, 0, 64, 78, 2000000, 666, 20000000, 4, 4);
        AddSimple(RMC, "Jacobo", "Ramón", "Ramón", 23, "ESP", 186, 78, RIGHT, CB, 0, 62, 74, 1000000, 333, 10000000, 4, 5);
        AddSimple(RMC, "David", "de la Gala", "De la Gala", 21, "ESP", 184, 77, RIGHT, CB, 0, 60, 76, 800000, 228, 8000000, 3, 14);
        AddSimple(RMC, "Adrián", "Lombardo", "Lombardo", 22, "ESP", 188, 78, RIGHT, CB, 0, 60, 74, 600000, 171, 6000000, 4, 15);
        AddSimple(RMC, "Daniel", "Yáñez", "Yáñez", 22, "ESP", 176, 70, RIGHT, LB, 0, 62, 76, 1500000, 500, 15000000, 4, 3);
        AddSimple(RMC, "Iker", "Sánchez", "I. Sánchez", 21, "ESP", 175, 70, RIGHT, LB, 0, 58, 74, 500000, 142, 5000000, 3, 16);
        AddSimple(RMC, "Lorenzo", "Aguado", "Aguado", 22, "ESP", 178, 71, RIGHT, RB, 0, 62, 76, 1200000, 400, 12000000, 4, 2);
        AddSimple(RMC, "Álex", "Gómez", "Á. Gómez", 21, "ESP", 177, 70, RIGHT, RB, 0, 58, 74, 500000, 142, 5000000, 3, 17);
        AddSimple(RMC, "Chema", "Andrés", "Andrés", 23, "ESP", 181, 72, RIGHT, CDM, CM_f, 62, 74, 1000000, 333, 10000000, 4, 6);
        AddSimple(RMC, "Hugo", "de Llanos", "De Llanos", 22, "ESP", 180, 72, RIGHT, CDM, 0, 58, 72, 400000, 100, 4000000, 3, 18);
        AddSimple(RMC, "Jairo", "Cabrera", "J. Cabrera", 23, "ESP", 180, 72, RIGHT, CM, 0, 62, 74, 1000000, 333, 10000000, 4, 8);
        AddSimple(RMC, "Manuel", "Ángel", "M. Ángel", 22, "ESP", 179, 71, RIGHT, CM, 0, 60, 74, 800000, 228, 8000000, 4, 10);
        AddSimple(RMC, "Pol", "Fortuny", "Fortuny", 22, "ESP", 178, 71, RIGHT, CM, CAM_f, 60, 76, 1000000, 333, 10000000, 3, 19);
        AddSimple(RMC, "Ian", "Bryan", "Bryan", 21, "ESP", 175, 68, RIGHT, CAM, 0, 62, 78, 2000000, 666, 20000000, 3, 11);
        AddSimple(RMC, "Víctor", "Valverde", "V. Valverde", 22, "ESP", 176, 68, RIGHT, CAM, CM_f, 60, 76, 1200000, 400, 12000000, 4, 20);
        AddSimple(RMC, "Pablo", "Rodríguez", "P. Rodríguez", 22, "ESP", 176, 68, RIGHT, LW, RW_f, 64, 78, 2500000, 834, 25000000, 4, 7);
        AddSimple(RMC, "Mario", "Martín", "M. Martín", 21, "ESP", 174, 67, RIGHT, RW, LW_f, 62, 78, 2000000, 666, 20000000, 3, 12);
        AddSimple(RMC, "Daniel", "Muñoz", "D. Muñoz", 21, "ESP", 173, 67, RIGHT, LW, 0, 58, 74, 600000, 171, 6000000, 3, 21);
        AddSimple(RMC, "Álvaro", "Rodríguez", "Á. Rodríguez", 20, "ESP", 190, 79, RIGHT, ST, 0, 64, 80, 3000000, 1000, 30000000, 3, 9);
        AddSimple(RMC, "Daniel", "García", "D. García", 22, "ESP", 183, 76, RIGHT, ST, 0, 62, 76, 1500000, 500, 15000000, 4, 22);
        AddSimple(RMC, "Noah", "Díez", "Díez", 21, "ESP", 182, 76, RIGHT, ST, RW_f, 60, 76, 1000000, 333, 10000000, 3, 23);

        // ══════════════════════════════════════════════════════════
        // Nàstic de Tarragona (TeamId=69)
        // ══════════════════════════════════════════════════════════
        int NAS = 69;
        AddSimple(NAS, "Manuel", "García", "Manu G.", 31, "ESP", 189, 86, RIGHT, GK, 0, 65, 68, 600000, 171, 6000000, 5, 1);
        AddSimple(NAS, "Alberto", "Varo", "Varo", 29, "ESP", 185, 85, RIGHT, GK, 0, 63, 66, 400000, 100, 4000000, 5, 13);
        AddSimple(NAS, "Eric", "Puig", "Puig", 24, "ESP", 187, 86, RIGHT, GK, 0, 60, 68, 200000, 50, 2000000, 4, 25);
        AddSimple(NAS, "Pol", "Domingo", "Domingo", 26, "ESP", 186, 78, RIGHT, CB, 0, 65, 68, 800000, 228, 8000000, 5, 4);
        AddSimple(NAS, "Miquel", "Robuste", "Robuste", 31, "ESP", 184, 77, RIGHT, CB, 0, 63, 66, 500000, 142, 5000000, 5, 5);
        AddSimple(NAS, "Joan", "Oriol", "Oriol", 35, "ESP", 182, 76, RIGHT, CB, LB_f, 62, 64, 200000, 50, 2000000, 5, 14);
        AddSimple(NAS, "Pau", "Martínez", "P. Martínez", 23, "ESP", 186, 78, RIGHT, CB, 0, 60, 70, 300000, 75, 3000000, 4, 15);
        AddSimple(NAS, "Iago", "Indias", "Indias", 26, "ESP", 178, 71, RIGHT, LB, 0, 64, 68, 600000, 171, 6000000, 4, 3);
        AddSimple(NAS, "Arnau", "Soldevila", "Soldevila", 22, "ESP", 177, 70, RIGHT, LB, 0, 60, 70, 200000, 50, 2000000, 4, 16);
        AddSimple(NAS, "Unai", "Dufur", "Dufur", 27, "ESP", 178, 71, RIGHT, RB, 0, 63, 66, 500000, 142, 5000000, 5, 2);
        AddSimple(NAS, "David", "Vidal", "Vidal", 24, "ESP", 177, 70, RIGHT, RB, 0, 60, 68, 200000, 50, 2000000, 4, 17);
        AddSimple(NAS, "Marc", "Montes", "Montes", 27, "ESP", 182, 72, RIGHT, CDM, CM_f, 64, 68, 600000, 171, 6000000, 5, 6);
        AddSimple(NAS, "Ander", "Gorostidi", "Gorostidi", 30, "ESP", 185, 74, RIGHT, CDM, 0, 62, 66, 400000, 100, 4000000, 5, 18);
        AddSimple(NAS, "Dani", "Cortijos", "Cortijos", 25, "ESP", 180, 72, RIGHT, CM, 0, 64, 68, 600000, 171, 6000000, 4, 8);
        AddSimple(NAS, "David", "Ballesteros", "Ballesteros", 30, "ESP", 178, 71, RIGHT, CM, 0, 62, 66, 400000, 100, 4000000, 5, 10);
        AddSimple(NAS, "Jordi", "Ventura", "Ventura", 24, "ESP", 179, 71, RIGHT, CM, CAM_f, 60, 68, 200000, 50, 2000000, 4, 19);
        AddSimple(NAS, "Alex", "López", "A. López", 22, "ESP", 175, 68, RIGHT, CAM, CM_f, 63, 72, 600000, 171, 6000000, 4, 11);
        AddSimple(NAS, "Marc", "Álvarez", "Álvarez", 28, "ESP", 180, 70, RIGHT, LW, RW_f, 65, 68, 1000000, 333, 10000000, 5, 7);
        AddSimple(NAS, "Jaume", "Jornet", "Jornet", 24, "ESP", 175, 68, RIGHT, RW, LW_f, 62, 68, 400000, 100, 4000000, 4, 12);
        AddSimple(NAS, "Adrián", "Gómez", "A. Gómez", 26, "ESP", 178, 69, RIGHT, LW, 0, 60, 64, 200000, 50, 2000000, 4, 21);
        AddSimple(NAS, "Pablo", "Fernández", "P. Fernández", 27, "ESP", 182, 76, RIGHT, ST, 0, 65, 68, 1000000, 333, 10000000, 5, 9);
        AddSimple(NAS, "Marc", "Fernández", "M. Fernández", 23, "ESP", 184, 77, RIGHT, ST, 0, 62, 70, 400000, 100, 4000000, 4, 22);
        AddSimple(NAS, "Xavi", "Ferrer", "Ferrer", 22, "ESP", 183, 76, RIGHT, ST, LW_f, 60, 70, 300000, 75, 3000000, 4, 23);
        AddSimple(NAS, "Antonio", "Hidalgo", "Hidalgo", 25, "ESP", 181, 75, RIGHT, CF, ST_f, 64, 68, 800000, 228, 8000000, 4, 24);

        // ══════════════════════════════════════════════════════════
        // Hércules CF (TeamId=70)
        // ══════════════════════════════════════════════════════════
        int HER = 70;
        AddSimple(HER, "Carlos", "Abad", "Abad", 31, "ESP", 190, 87, RIGHT, GK, 0, 64, 68, 600000, 171, 6000000, 5, 1);
        AddSimple(HER, "Pablo", "Espina", "Espina", 24, "ESP", 185, 85, RIGHT, GK, 0, 62, 68, 200000, 50, 2000000, 4, 13);
        AddSimple(HER, "David", "González", "D. González", 22, "ESP", 186, 85, RIGHT, GK, 0, 58, 68, 100000, 20, 1000000, 4, 25);
        AddSimple(HER, "Pablo", "Íñiguez", "Íñiguez", 32, "ESP", 186, 78, RIGHT, CB, 0, 64, 68, 600000, 171, 6000000, 5, 4);
        AddSimple(HER, "Adrián", "Crusat", "Crusat", 28, "ESP", 185, 77, RIGHT, CB, 0, 62, 66, 400000, 100, 4000000, 5, 5);
        AddSimple(HER, "José", "Carlos", "J. Carlos", 27, "ESP", 183, 76, RIGHT, CB, 0, 60, 64, 200000, 50, 2000000, 4, 14);
        AddSimple(HER, "Miguel", "López", "M. López", 23, "ESP", 186, 78, RIGHT, CB, 0, 58, 68, 100000, 20, 1000000, 4, 15);
        AddSimple(HER, "David", "Fernández", "D. Fernández", 26, "ESP", 176, 70, RIGHT, LB, CB_f, 63, 66, 600000, 171, 6000000, 5, 3);
        AddSimple(HER, "Óscar", "García", "Ó. García", 24, "ESP", 175, 70, RIGHT, LB, 0, 58, 66, 100000, 20, 1000000, 4, 16);
        AddSimple(HER, "José", "Luis", "J. Luis", 27, "ESP", 178, 71, RIGHT, RB, 0, 62, 66, 400000, 100, 4000000, 5, 2);
        AddSimple(HER, "José", "María", "J. María", 24, "ESP", 177, 70, RIGHT, RB, 0, 58, 66, 100000, 20, 1000000, 4, 17);
        AddSimple(HER, "Ángel", "Sánchez", "Á. Sánchez", 30, "ESP", 182, 72, RIGHT, CDM, CM_f, 63, 66, 600000, 171, 6000000, 5, 6);
        AddSimple(HER, "Pablo", "Alarcón", "Alarcón", 26, "ESP", 180, 72, RIGHT, CDM, 0, 60, 66, 200000, 50, 2000000, 4, 18);
        AddSimple(HER, "Marc", "Serramitja", "Serramitja", 28, "ESP", 180, 72, RIGHT, CM, 0, 63, 66, 500000, 142, 5000000, 5, 8);
        AddSimple(HER, "José", "Gutiérrez", "J. Gutiérrez", 25, "ESP", 178, 71, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 4, 10);
        AddSimple(HER, "Iván", "Pérez", "I. Pérez", 22, "ESP", 179, 71, RIGHT, CM, 0, 58, 70, 100000, 20, 1000000, 4, 19);
        AddSimple(HER, "Pablo", "Ganet", "Ganet", 29, "ESP", 175, 68, RIGHT, CAM, CM_f, 62, 66, 400000, 100, 4000000, 5, 11);
        AddSimple(HER, "Héctor", "Villajos", "Villajos", 24, "ESP", 176, 68, RIGHT, CAM, 0, 60, 68, 200000, 50, 2000000, 4, 20);
        AddSimple(HER, "Rubén", "Torres", "R. Torres", 27, "ESP", 180, 70, RIGHT, LW, RW_f, 64, 68, 800000, 228, 8000000, 5, 7);
        AddSimple(HER, "David", "Córcoles", "Córcoles", 26, "ESP", 174, 67, RIGHT, RW, LW_f, 62, 66, 400000, 100, 4000000, 5, 12);
        AddSimple(HER, "Sergio", "García", "S. García", 23, "ESP", 176, 68, RIGHT, LW, 0, 58, 68, 100000, 20, 1000000, 4, 21);
        AddSimple(HER, "Alberto", "Fernández", "A. Fernández", 26, "ESP", 182, 76, RIGHT, ST, 0, 64, 68, 800000, 228, 8000000, 5, 9);
        AddSimple(HER, "Manuel", "López", "Manu L.", 24, "ESP", 183, 76, RIGHT, ST, 0, 60, 68, 200000, 50, 2000000, 4, 22);
        AddSimple(HER, "Pablo", "Martínez", "P. Martínez", 25, "ESP", 180, 75, RIGHT, ST, CF_f, 62, 68, 400000, 100, 4000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // UD Ibiza (TeamId=71)
        // ══════════════════════════════════════════════════════════
        int IBZ = 71;
        AddSimple(IBZ, "Juan", "Soriano", "Soriano", 28, "ESP", 185, 85, RIGHT, GK, 0, 64, 68, 800000, 228, 8000000, 5, 1);
        AddSimple(IBZ, "Albert", "Martínez", "A. Martínez", 24, "ESP", 189, 86, RIGHT, GK, 0, 62, 68, 300000, 75, 3000000, 4, 13);
        AddSimple(IBZ, "José", "Santos", "Santos", 22, "ESP", 186, 85, RIGHT, GK, 0, 58, 68, 100000, 20, 1000000, 4, 25);
        AddSimple(IBZ, "Jordi", "Martín", "Martín", 27, "ESP", 185, 77, RIGHT, CB, 0, 64, 68, 600000, 171, 6000000, 5, 4);
        AddSimple(IBZ, "José", "Escobar", "Escobar", 26, "ESP", 184, 77, RIGHT, CB, 0, 62, 66, 400000, 100, 4000000, 5, 5);
        AddSimple(IBZ, "Miki", "López", "M. López", 24, "ESP", 186, 78, RIGHT, CB, 0, 60, 68, 200000, 50, 2000000, 4, 14);
        AddSimple(IBZ, "Marcos", "Peña", "Peña", 23, "ESP", 183, 76, RIGHT, CB, 0, 58, 66, 100000, 20, 1000000, 4, 15);
        AddSimple(IBZ, "Marc", "Mateu", "Mateu", 28, "ESP", 176, 70, RIGHT, LB, CB_f, 63, 66, 600000, 171, 6000000, 5, 3);
        AddSimple(IBZ, "Álex", "Rivas", "Rivas", 24, "ESP", 175, 70, RIGHT, LB, 0, 60, 68, 200000, 50, 2000000, 4, 16);
        AddSimple(IBZ, "David", "Güerri", "Güerri", 27, "ESP", 178, 71, RIGHT, RB, 0, 63, 66, 500000, 142, 5000000, 5, 2);
        AddSimple(IBZ, "Carlos", "Díaz", "C. Díaz", 24, "ESP", 177, 70, RIGHT, RB, 0, 60, 68, 200000, 50, 2000000, 4, 17);
        AddSimple(IBZ, "Álex", "Gálvez", "Gálvez", 28, "ESP", 182, 72, RIGHT, CDM, CM_f, 64, 68, 800000, 228, 8000000, 5, 6);
        AddSimple(IBZ, "Pablo", "Morillas", "Morillas", 25, "ESP", 180, 72, RIGHT, CDM, 0, 60, 66, 200000, 50, 2000000, 4, 18);
        AddSimple(IBZ, "Javier", "Molina", "Molina", 27, "ESP", 180, 72, RIGHT, CM, 0, 63, 66, 500000, 142, 5000000, 5, 8);
        AddSimple(IBZ, "Marc", "Vila", "Vila", 25, "ESP", 178, 71, RIGHT, CM, CAM_f, 60, 66, 200000, 50, 2000000, 4, 10);
        AddSimple(IBZ, "Iván", "Ramos", "Ramos", 23, "ESP", 179, 71, RIGHT, CM, 0, 58, 68, 100000, 20, 1000000, 4, 19);
        AddSimple(IBZ, "David", "Álvarez", "Álvarez", 29, "ESP", 175, 68, RIGHT, CAM, CM_f, 62, 66, 400000, 100, 4000000, 5, 11);
        AddSimple(IBZ, "Xesc", "Riquelme", "Riquelme", 24, "ESP", 176, 68, RIGHT, CAM, 0, 60, 68, 200000, 50, 2000000, 4, 20);
        AddSimple(IBZ, "Ismael", "García", "I. García", 27, "ESP", 180, 70, RIGHT, LW, RW_f, 64, 68, 800000, 228, 8000000, 5, 7);
        AddSimple(IBZ, "Óscar", "Sierra", "Sierra", 26, "ESP", 174, 67, RIGHT, RW, LW_f, 62, 66, 400000, 100, 4000000, 5, 12);
        AddSimple(IBZ, "Manuel", "Ruiz", "Ruiz", 23, "ESP", 176, 68, RIGHT, LW, 0, 58, 68, 100000, 20, 1000000, 4, 21);
        AddSimple(IBZ, "Álex", "Pérez", "Á. Pérez", 26, "ESP", 182, 76, RIGHT, ST, 0, 64, 68, 800000, 228, 8000000, 5, 9);
        AddSimple(IBZ, "Adrián", "Sánchez", "A. Sánchez", 24, "ESP", 183, 76, RIGHT, ST, 0, 60, 68, 200000, 50, 2000000, 4, 22);
        AddSimple(IBZ, "Hugo", "García", "H. García", 22, "ESP", 180, 75, RIGHT, ST, 0, 58, 70, 100000, 20, 1000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Algeciras CF (TeamId=72)
        // ══════════════════════════════════════════════════════════
        int ALG = 72;
        AddSimple(ALG, "David", "Munuera", "Munuera", 28, "ESP", 185, 85, RIGHT, GK, 0, 63, 68, 500000, 142, 5000000, 5, 1);
        AddSimple(ALG, "Rubén", "Gálvez", "Gálvez", 24, "ESP", 188, 86, RIGHT, GK, 0, 60, 68, 200000, 50, 2000000, 4, 13);
        AddSimple(ALG, "Jesús", "Álvarez", "J. Álvarez", 22, "ESP", 186, 85, RIGHT, GK, 0, 58, 68, 100000, 20, 1000000, 4, 25);
        AddSimple(ALG, "Carlos", "Gutiérrez", "C. Gutiérrez", 28, "ESP", 185, 77, RIGHT, CB, 0, 63, 66, 500000, 142, 5000000, 5, 4);
        AddSimple(ALG, "José", "Márquez", "Márquez", 26, "ESP", 184, 77, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 5, 5);
        AddSimple(ALG, "Sergio", "Sánchez", "S. Sánchez", 24, "ESP", 186, 78, RIGHT, CB, 0, 58, 66, 100000, 20, 1000000, 4, 14);
        AddSimple(ALG, "Marc", "Gracia", "Gracia", 23, "ESP", 183, 76, RIGHT, CB, 0, 56, 66, 100000, 20, 1000000, 4, 15);
        AddSimple(ALG, "Jorge", "Canelo", "Canelo", 27, "ESP", 176, 70, RIGHT, LB, CB_f, 62, 66, 400000, 100, 4000000, 5, 3);
        AddSimple(ALG, "Álvaro", "Leiva", "Leiva", 24, "ESP", 175, 70, RIGHT, LB, 0, 58, 66, 100000, 20, 1000000, 4, 16);
        AddSimple(ALG, "Manuel", "Serrano", "Serrano", 27, "ESP", 178, 71, RIGHT, RB, 0, 62, 66, 400000, 100, 4000000, 5, 2);
        AddSimple(ALG, "Dani", "Postigo", "Postigo", 24, "ESP", 177, 70, RIGHT, RB, 0, 58, 66, 100000, 20, 1000000, 4, 17);
        AddSimple(ALG, "Alberto", "López", "A. López", 29, "ESP", 182, 72, RIGHT, CDM, CM_f, 62, 66, 400000, 100, 4000000, 5, 6);
        AddSimple(ALG, "Pablo", "Sánchez", "P. Sánchez", 25, "ESP", 180, 72, RIGHT, CDM, 0, 58, 66, 100000, 20, 1000000, 4, 18);
        AddSimple(ALG, "Eder", "Barbosa", "Barbosa", 28, "ESP", 180, 72, RIGHT, CM, 0, 62, 66, 400000, 100, 4000000, 5, 8);
        AddSimple(ALG, "Álex", "Meléndez", "Meléndez", 25, "ESP", 178, 71, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 4, 10);
        AddSimple(ALG, "Javi", "Hernández", "J. Hernández", 23, "ESP", 179, 71, RIGHT, CM, 0, 58, 68, 100000, 20, 1000000, 4, 19);
        AddSimple(ALG, "Miguel", "Ángel", "M. Ángel", 29, "ESP", 175, 68, RIGHT, CAM, CM_f, 61, 64, 300000, 75, 3000000, 5, 11);
        AddSimple(ALG, "Fran", "Carbià", "Carbià", 24, "ESP", 176, 68, RIGHT, CAM, 0, 58, 68, 100000, 20, 1000000, 4, 20);
        AddSimple(ALG, "Rafa", "Sánchez", "R. Sánchez", 27, "ESP", 180, 70, RIGHT, LW, RW_f, 63, 66, 600000, 171, 6000000, 5, 7);
        AddSimple(ALG, "Luis", "Sánchez", "L. Sánchez", 26, "ESP", 174, 67, RIGHT, RW, LW_f, 62, 66, 400000, 100, 4000000, 5, 12);
        AddSimple(ALG, "Adrián", "Díaz", "A. Díaz", 23, "ESP", 176, 68, RIGHT, LW, 0, 58, 68, 100000, 20, 1000000, 4, 21);
        AddSimple(ALG, "Diego", "Molina", "Molina", 27, "ESP", 182, 76, RIGHT, ST, 0, 63, 66, 600000, 171, 6000000, 5, 9);
        AddSimple(ALG, "Álvaro", "Romero", "Á. Romero", 24, "ESP", 183, 76, RIGHT, ST, 0, 60, 68, 200000, 50, 2000000, 4, 22);
        AddSimple(ALG, "José", "Rodríguez", "J. Rodríguez", 26, "ESP", 180, 75, RIGHT, ST, CF_f, 60, 64, 200000, 50, 2000000, 4, 23);

        // ══════════════════════════════════════════════════════════
        // Antequera CF (TeamId=73)
        // ══════════════════════════════════════════════════════════
        int ANT = 73;
        AddSimple(ANT, "Samu", "Pérez", "Samu Pérez", 29, "ESP", 185, 85, RIGHT, GK, 0, 63, 66, 400000, 100, 4000000, 5, 1);
        AddSimple(ANT, "Dani", "Alcover", "Alcover", 22, "ESP", 188, 86, RIGHT, GK, 0, 60, 70, 200000, 50, 2000000, 4, 13);
        AddSimple(ANT, "Félix", "Garreta", "Garreta", 26, "ESP", 184, 77, RIGHT, CB, 0, 62, 66, 300000, 75, 3000000, 4, 4);
        AddSimple(ANT, "Raúl", "Albentosa", "Albentosa", 37, "ESP", 188, 78, RIGHT, CB, 0, 60, 62, 150000, 30, 1500000, 5, 3);
        AddSimple(ANT, "Raúl", "Giménez", "R. Giménez", 22, "ESP", 186, 78, RIGHT, CB, 0, 60, 72, 250000, 62, 2500000, 4, 15);
        AddSimple(ANT, "Juan", "Aspra", "Aspra", 23, "ESP", 184, 77, RIGHT, CB, LB_f, 60, 68, 200000, 50, 2000000, 4, 20);
        AddSimple(ANT, "Álex", "Barbu", "Barbu", 25, "ESP", 183, 76, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 4);
        AddSimple(ANT, "Antonio", "Luna", "A. Luna", 35, "ESP", 175, 70, RIGHT, LB, CB_f, 62, 64, 200000, 50, 2000000, 5, 5);
        AddSimple(ANT, "Edu", "Sánchez", "E. Sánchez", 27, "ESP", 176, 70, RIGHT, LB, 0, 60, 64, 150000, 30, 1500000, 4, 14);
        AddSimple(ANT, "Javi", "Antón", "Antón", 25, "ESP", 178, 71, RIGHT, RB, LW_f, 62, 68, 300000, 75, 3000000, 4, 2);
        AddSimple(ANT, "Iván", "Rodríguez", "I. Rodríguez", 29, "ESP", 177, 70, RIGHT, RB, 0, 60, 64, 150000, 30, 1500000, 4, 19);
        AddSimple(ANT, "Alberto", "Quintana", "Quintana", 24, "ESP", 182, 72, RIGHT, CM, CDM_f, 60, 68, 200000, 50, 2000000, 4, 22);
        AddSimple(ANT, "David", "Ramos", "D. Ramos", 28, "ESP", 178, 71, RIGHT, CDM, CM_f, 62, 66, 250000, 62, 2500000, 5, 6);
        AddSimple(ANT, "Rafa", "Diz", "Diz", 25, "ESP", 180, 72, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 4, 23);
        AddSimple(ANT, "Dani", "Clavijo", "Clavijo", 22, "ESP", 179, 71, RIGHT, CM, 0, 62, 72, 300000, 75, 3000000, 4, 8);
        AddSimple(ANT, "Moha", "Bassele", "Bassele", 26, "SEN", 183, 73, RIGHT, CM, CDM_f, 60, 66, 250000, 62, 2500000, 4, 18);
        AddSimple(ANT, "Luismi", "Gutiérrez", "Luismi G.", 30, "ESP", 172, 67, RIGHT, CAM, CM_f, 62, 66, 300000, 75, 3000000, 5, 10);
        AddSimple(ANT, "Moi", "Parra", "Parra", 24, "ESP", 176, 68, RIGHT, LW, RW_f, 60, 68, 200000, 50, 2000000, 4, 17);
        AddSimple(ANT, "Ousama", "Siddiki", "Siddiki", 27, "MAR", 178, 69, RIGHT, RW, LW_f, 62, 66, 300000, 75, 3000000, 5, 11);
        AddSimple(ANT, "Nico", "Njalla", "Njalla", 24, "CMR", 183, 76, RIGHT, ST, RW_f, 60, 68, 250000, 62, 2500000, 4, 25);
        AddSimple(ANT, "Jonathan", "Biabiany", "Biabiany", 38, "FRA", 177, 74, RIGHT, RW, ST_f, 62, 64, 150000, 30, 1500000, 5, 7);
        AddSimple(ANT, "Adrià", "Gené", "Gené", 27, "ESP", 180, 72, RIGHT, CF, CAM_f, 62, 66, 300000, 75, 3000000, 5, 16);
        AddSimple(ANT, "Destiny", "Ilahude", "Ilahude", 22, "NGA", 182, 76, RIGHT, ST, LW_f, 60, 72, 250000, 62, 2500000, 4, 24);
        AddSimple(ANT, "Marcelo", "dos Santos", "M. Santos", 27, "BRA", 184, 77, RIGHT, ST, 0, 62, 68, 400000, 100, 4000000, 5, 9);
        AddSimple(ANT, "Oussama", "Chit", "Chit", 26, "MAR", 180, 75, RIGHT, ST, 0, 60, 66, 200000, 50, 2000000, 5, 21);

        // ══════════════════════════════════════════════════════════
        // Real Jaén (TeamId=74)
        // ══════════════════════════════════════════════════════════
        int JAE = 74;
        AddSimple(JAE, "Javier", "Rabanillo", "Rabanillo", 28, "ESP", 186, 85, RIGHT, GK, 0, 62, 66, 300000, 75, 3000000, 5, 1);
        AddSimple(JAE, "Jaime", "Morillas", "Morillas", 23, "ESP", 184, 84, RIGHT, GK, 0, 60, 68, 100000, 20, 1000000, 4, 13);
        AddSimple(JAE, "Pelayo", "Suárez", "P. Suárez", 24, "ESP", 188, 86, RIGHT, GK, 0, 58, 66, 75000, 15, 750000, 4, 25);
        AddSimple(JAE, "José", "Carrillo", "Carrillo", 26, "ESP", 185, 77, RIGHT, CB, 0, 62, 66, 200000, 50, 2000000, 5, 4);
        AddSimple(JAE, "Mauro", "Cabello", "Cabello", 28, "ESP", 184, 77, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 5, 20);
        AddSimple(JAE, "Pedro", "Fernández", "P. Fernández", 24, "ESP", 183, 76, RIGHT, CB, LB_f, 60, 68, 150000, 30, 1500000, 4, 25);
        AddSimple(JAE, "David", "Serrano", "Serrano", 27, "ESP", 176, 70, RIGHT, LB, CB_f, 62, 66, 250000, 62, 2500000, 5, 3);
        AddSimple(JAE, "Fernando", "Cortijo", "Cortijo", 29, "ESP", 175, 70, RIGHT, LB, LM_f, 60, 64, 150000, 30, 1500000, 5, 16);
        AddSimple(JAE, "Connor", "Ruane", "Ruane", 25, "ENG", 183, 76, RIGHT, CB, LB_f, 60, 66, 200000, 50, 2000000, 4, 3);
        AddSimple(JAE, "Curro", "Burgos", "Burgos", 26, "ESP", 178, 71, RIGHT, RB, 0, 60, 66, 200000, 50, 2000000, 4, 2);
        AddSimple(JAE, "Javi", "Moyano", "Moyano", 39, "ESP", 178, 71, RIGHT, RB, 0, 58, 60, 75000, 15, 750000, 5, 17);
        AddSimple(JAE, "José", "Álvarez", "J. Álvarez", 27, "ESP", 182, 72, RIGHT, CDM, CM_f, 62, 66, 250000, 62, 2500000, 5, 6);
        AddSimple(JAE, "Antonio", "Caballero", "Caballero", 32, "ESP", 180, 72, RIGHT, CM, CDM_f, 62, 66, 300000, 75, 3000000, 5, 8);
        AddSimple(JAE, "Nacho", "Vizcaíno", "Vizcaíno", 26, "ESP", 179, 71, RIGHT, CM, 0, 60, 66, 150000, 30, 1500000, 4, 14);
        AddSimple(JAE, "Adri", "Paz", "Paz", 33, "ESP", 168, 65, RIGHT, CAM, CM_f, 62, 66, 300000, 75, 3000000, 5, 10);
        AddSimple(JAE, "Ñito", "González", "Ñito", 25, "ESP", 176, 68, RIGHT, LW, RW_f, 60, 66, 200000, 50, 2000000, 4, 11);
        AddSimple(JAE, "Óscar", "Lozano", "Lozano", 29, "ESP", 178, 69, RIGHT, RW, LW_f, 62, 66, 300000, 75, 3000000, 5, 22);
        AddSimple(JAE, "Mario", "Martos", "Martos", 33, "ESP", 176, 70, RIGHT, CM, RB_f, 60, 64, 200000, 50, 2000000, 5, 11);
        AddSimple(JAE, "Moha", "Sanhaji", "Sanhaji", 24, "MAR", 178, 69, RIGHT, LW, 0, 58, 66, 100000, 20, 1000000, 4, 21);
        AddSimple(JAE, "Adrián", "Fernández", "A. Fernández", 26, "ESP", 180, 70, RIGHT, RW, 0, 60, 64, 150000, 30, 1500000, 4, 12);
        AddSimple(JAE, "Alberto", "Bernardo", "Bernardo", 27, "ESP", 182, 76, RIGHT, ST, 0, 62, 66, 300000, 75, 3000000, 5, 9);
        AddSimple(JAE, "Iván", "Breñé", "Breñé", 24, "ESP", 183, 76, RIGHT, ST, CF_f, 60, 68, 200000, 50, 2000000, 4, 18);
        AddSimple(JAE, "Marco", "Siverio", "Siverio", 28, "ESP", 184, 77, RIGHT, ST, 0, 60, 64, 150000, 30, 1500000, 4, 24);
        AddSimple(JAE, "Agus", "Alonso", "Alonso", 29, "ESP", 180, 75, RIGHT, ST, 0, 58, 64, 100000, 20, 1000000, 4, 23);
        AddSimple(JAE, "Zeidy", "Traore", "Traore", 23, "MLI", 185, 77, RIGHT, ST, RW_f, 58, 68, 100000, 20, 1000000, 4, 19);

        // ══════════════════════════════════════════════════════════
        // CF Atlético Madrileño (TeamId=75)
        // ══════════════════════════════════════════════════════════
        int MAD = 75;
        AddSimple(MAD, "Salvi", "Esquivel", "Esquivel", 20, "ESP", 191, 83, RIGHT, GK, 0, 60, 76, 500000, 142, 5000000, 3, 1);
        AddSimple(MAD, "Mario", "de Luis", "M. de Luis", 24, "ESP", 185, 73, RIGHT, GK, 0, 60, 72, 300000, 75, 3000000, 4, 13);
        AddSimple(MAD, "Luismi", "Morales", "Morales", 21, "ESP", 188, 86, RIGHT, GK, 0, 58, 74, 200000, 50, 2000000, 3, 25);
        AddSimple(MAD, "Aleksa", "Purić", "Purić", 22, "SRB", 190, 79, RIGHT, CB, 0, 60, 74, 400000, 100, 4000000, 4, 4);
        AddSimple(MAD, "Dani", "Martínez", "D. Martínez", 22, "ESP", 185, 77, RIGHT, CB, 0, 60, 72, 300000, 75, 3000000, 4, 5);
        AddSimple(MAD, "Ilias", "Kostis", "Kostis", 23, "GRE", 193, 81, RIGHT, CB, 0, 60, 72, 400000, 100, 4000000, 4, 14);
        AddSimple(MAD, "Gerónimo", "Spina", "Spina", 21, "ARG", 184, 77, RIGHT, CB, LB_f, 58, 74, 350000, 87, 3500000, 3, 15);
        AddSimple(MAD, "Matías", "Barboza", "Barboza", 24, "ESP", 182, 76, RIGHT, CB, 0, 58, 68, 200000, 50, 2000000, 4, 16);
        AddSimple(MAD, "Julio", "Díaz", "J. Díaz", 21, "ESP", 178, 71, RIGHT, LB, 0, 60, 74, 300000, 75, 3000000, 3, 3);
        AddSimple(MAD, "Marko", "Perović", "Perović", 20, "MNE", 186, 78, RIGHT, CB, RB_f, 58, 76, 350000, 87, 3500000, 3, 17);
        AddSimple(MAD, "Adrián", "Corral", "Corral", 23, "ESP", 178, 71, RIGHT, RB, 0, 60, 72, 300000, 75, 3000000, 4, 2);
        AddSimple(MAD, "Javier", "Boñar", "Boñar", 21, "ESP", 175, 70, RIGHT, RB, 0, 58, 74, 250000, 62, 2500000, 3, 18);
        AddSimple(MAD, "Romeo", "Hueso", "Hueso", 19, "ESP", 180, 72, RIGHT, CM, 0, 58, 78, 400000, 100, 4000000, 3, 19);
        AddSimple(MAD, "Javi", "Serrano", "Serrano", 23, "ESP", 178, 71, RIGHT, CDM, CM_f, 62, 74, 500000, 142, 5000000, 4, 6);
        AddSimple(MAD, "Javi", "Morcillo", "Morcillo", 20, "ESP", 180, 72, RIGHT, CM, 0, 58, 76, 300000, 75, 3000000, 3, 20);
        AddSimple(MAD, "Jorge", "Castillo", "Castillo", 20, "ESP", 180, 72, RIGHT, CM, 0, 58, 76, 300000, 75, 3000000, 3, 21);
        AddSimple(MAD, "Marcos", "Parriego", "Parriego", 20, "ESP", 180, 72, RIGHT, CM, 0, 58, 76, 300000, 75, 3000000, 3, 22);
        AddSimple(MAD, "Rayane", "Belaid", "Belaid", 21, "ESP", 178, 71, RIGHT, CM, 0, 60, 76, 400000, 100, 4000000, 3, 23);
        AddSimple(MAD, "Jano", "Monserrate", "Monserrate", 20, "ESP", 178, 71, RIGHT, CAM, CM_f, 60, 78, 500000, 142, 5000000, 3, 24);
        AddSimple(MAD, "Arnau", "Ortiz", "Ortiz", 24, "ESP", 176, 68, RIGHT, RW, LW_f, 62, 72, 500000, 142, 5000000, 4, 7);
        AddSimple(MAD, "Iker", "Luque", "Luque", 21, "ESP", 175, 68, RIGHT, CAM, LW_f, 60, 76, 400000, 100, 4000000, 3, 8);
        AddSimple(MAD, "Rafa", "Llorente", "Llorente", 23, "ESP", 183, 76, RIGHT, ST, 0, 62, 74, 600000, 171, 6000000, 4, 9);
        AddSimple(MAD, "Koke", "Mota", "Mota", 21, "ESP", 180, 72, RIGHT, CM, 0, 58, 74, 250000, 62, 2500000, 3, 10);
        AddSimple(MAD, "Miguel", "Cubo", "Cubo", 20, "ESP", 182, 76, RIGHT, ST, 0, 58, 78, 400000, 100, 4000000, 3, 11);

        // ══════════════════════════════════════════════════════════
        // CF Villarreal B (TeamId=76)
        // ══════════════════════════════════════════════════════════
        int VLB = 76;
        AddSimple(VLB, "Rubén", "Gómez", "R. Gómez", 20, "ESP", 185, 73, RIGHT, GK, 0, 60, 76, 500000, 142, 5000000, 3, 1);
        AddSimple(VLB, "Yakiv", "Kinareykin", "Kinareykin", 22, "UKR", 188, 86, RIGHT, GK, 0, 58, 72, 300000, 75, 3000000, 4, 13);
        AddSimple(VLB, "Ismael", "Sierra", "Sierra", 23, "ESP", 192, 83, RIGHT, CB, 0, 62, 74, 600000, 171, 6000000, 4, 4);
        AddSimple(VLB, "Jean Ives", "Valou", "Valou", 20, "CIV", 188, 80, RIGHT, CB, 0, 60, 76, 500000, 142, 5000000, 3, 5);
        AddSimple(VLB, "Lautaro", "Spatz", "Spatz", 22, "ARG", 184, 77, RIGHT, CB, LB_f, 60, 74, 400000, 100, 4000000, 4, 14);
        AddSimple(VLB, "Eneko", "Ortiz", "E. Ortiz", 23, "ESP", 186, 78, RIGHT, CB, 0, 60, 72, 300000, 75, 3000000, 4, 15);
        AddSimple(VLB, "Iván", "Rodríguez", "Iván R.", 22, "ESP", 183, 73, RIGHT, CB, RB_f, 58, 72, 300000, 75, 3000000, 4, 16);
        AddSimple(VLB, "Daniel", "Budesca", "Budesca", 20, "ESP", 183, 75, RIGHT, LB, CB_f, 60, 78, 500000, 142, 5000000, 3, 3);
        AddSimple(VLB, "César", "Bonafé", "Bonafé", 24, "ESP", 178, 71, RIGHT, RB, 0, 60, 72, 300000, 75, 3000000, 4, 2);
        AddSimple(VLB, "Carlos", "Macià", "Macià", 23, "ESP", 179, 71, RIGHT, RB, 0, 58, 70, 250000, 62, 2500000, 4, 17);
        AddSimple(VLB, "Alassane", "Diatta", "Diatta", 22, "SEN", 182, 72, RIGHT, CDM, CM_f, 60, 72, 400000, 100, 4000000, 4, 6);
        AddSimple(VLB, "Cheikh T.", "Thiam", "Thiam", 21, "SEN", 185, 74, RIGHT, CDM, 0, 58, 74, 350000, 87, 3500000, 3, 18);
        AddSimple(VLB, "Pablo", "Pascual", "Pascual", 23, "ESP", 180, 72, RIGHT, CM, 0, 60, 72, 300000, 75, 3000000, 4, 8);
        AddSimple(VLB, "Facundo", "Viveros", "Viveros", 22, "ARG", 178, 71, RIGHT, CM, CAM_f, 60, 74, 400000, 100, 4000000, 4, 10);
        AddSimple(VLB, "Hugo", "López", "H. López", 21, "ESP", 184, 77, RIGHT, ST, RW_f, 62, 78, 800000, 228, 8000000, 3, 19);
        AddSimple(VLB, "Joselillo", "Gaitán", "Joselillo", 23, "ESP", 176, 68, RIGHT, LW, ST_f, 62, 74, 500000, 142, 5000000, 4, 7);
        AddSimple(VLB, "Mahamoud", "Barry", "Barry", 22, "GUI", 175, 70, RIGHT, LW, RW_f, 60, 74, 400000, 100, 4000000, 4, 20);
        AddSimple(VLB, "Nizar", "El Jmili", "El Jmili", 21, "MAR", 178, 69, RIGHT, RW, LW_f, 60, 74, 400000, 100, 4000000, 3, 11);
        AddSimple(VLB, "Arnau", "Forés", "Forés", 22, "ESP", 176, 68, RIGHT, RW, 0, 58, 72, 300000, 75, 3000000, 4, 21);
        AddSimple(VLB, "Sebas", "Salazar", "Salazar", 23, "ESP", 179, 70, RIGHT, LW, RW_f, 60, 72, 300000, 75, 3000000, 4, 22);
        AddSimple(VLB, "Pau", "Cabanes", "Cabanes", 21, "ESP", 178, 74, RIGHT, ST, LW_f, 62, 78, 600000, 171, 6000000, 3, 9);
        AddSimple(VLB, "Ayman", "Arguigue", "Arguigue", 22, "MAR", 180, 75, RIGHT, ST, 0, 60, 74, 400000, 100, 4000000, 4, 23);
        AddSimple(VLB, "Etienne", "Eto'o", "Eto'o", 22, "CMR", 182, 76, RIGHT, ST, 0, 62, 76, 600000, 171, 6000000, 4, 12);
        AddSimple(VLB, "Albert", "García", "A. García", 23, "ESP", 183, 76, RIGHT, CF, ST_f, 60, 72, 300000, 75, 3000000, 4, 24);

        // ══════════════════════════════════════════════════════════
        // CE Europa (TeamId=77)
        // ══════════════════════════════════════════════════════════
        int EUR = 77;
        AddSimple(EUR, "Juan", "Flere", "Flere", 27, "ESP", 183, 83, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(EUR, "Lucas", "García", "L. García", 21, "ESP", 190, 87, RIGHT, GK, 0, 58, 72, 150000, 30, 1500000, 3, 13);
        AddSimple(EUR, "Marcel", "Sgrò", "Sgrò", 28, "ESP", 185, 77, RIGHT, CB, 0, 60, 66, 250000, 62, 2500000, 5, 6);
        AddSimple(EUR, "Fran", "Gil", "Gil", 20, "ESP", 183, 76, RIGHT, CB, 0, 58, 74, 150000, 30, 1500000, 3, 12);
        AddSimple(EUR, "Álex", "Cano", "A. Cano", 38, "ESP", 184, 77, RIGHT, CB, 0, 58, 60, 75000, 15, 750000, 5, 4);
        AddSimple(EUR, "Jordi", "Bas", "Bas", 22, "ESP", 186, 78, RIGHT, CB, 0, 58, 72, 150000, 30, 1500000, 4, 12);
        AddSimple(EUR, "Roger", "Escoruela", "Escoruela", 30, "ESP", 176, 70, RIGHT, LB, LM_f, 60, 64, 200000, 50, 2000000, 5, 3);
        AddSimple(EUR, "Joan", "Puig", "Puig", 23, "ESP", 177, 70, RIGHT, LB, 0, 58, 68, 150000, 30, 1500000, 4, 22);
        AddSimple(EUR, "Guillem", "Rodríguez", "G. Rodríguez", 28, "ESP", 178, 71, RIGHT, RB, 0, 60, 66, 250000, 62, 2500000, 5, 2);
        AddSimple(EUR, "Arnau", "Campeny", "Campeny", 29, "ESP", 176, 70, RIGHT, RB, 0, 58, 64, 200000, 50, 2000000, 5, 5);
        AddSimple(EUR, "Alejandro", "Jay", "Jay", 23, "ESP", 180, 72, RIGHT, RB, 0, 58, 68, 150000, 30, 1500000, 4, 24);
        AddSimple(EUR, "Meshak", "Babanzila", "Babanzila", 24, "COD", 182, 72, RIGHT, CDM, CM_f, 60, 68, 250000, 62, 2500000, 4, 8);
        AddSimple(EUR, "Óscar", "Vacas", "Vacas", 24, "ESP", 180, 72, RIGHT, CM, CDM_f, 58, 68, 200000, 50, 2000000, 4, 20);
        AddSimple(EUR, "Toni", "Caravaca", "Caravaca", 22, "ESP", 178, 71, RIGHT, CM, 0, 58, 70, 150000, 30, 1500000, 4, 23);
        AddSimple(EUR, "Alex", "Pla", "Pla", 34, "ESP", 176, 68, RIGHT, CM, CAM_f, 58, 60, 100000, 20, 1000000, 5, 10);
        AddSimple(EUR, "Josu", "Gallastegui", "Gallastegui", 25, "ESP", 180, 72, RIGHT, CM, 0, 58, 66, 150000, 30, 1500000, 4, 11);
        AddSimple(EUR, "Jordi", "Cano", "J. Cano", 26, "ESP", 175, 68, RIGHT, CAM, CM_f, 60, 66, 250000, 62, 2500000, 5, 14);
        AddSimple(EUR, "Carlos", "González", "C. González", 27, "ESP", 178, 69, RIGHT, LW, RW_f, 60, 66, 250000, 62, 2500000, 5, 7);
        AddSimple(EUR, "George", "Andrews", "Andrews", 25, "ENG", 180, 70, RIGHT, RW, LW_f, 58, 66, 200000, 50, 2000000, 4, 9);
        AddSimple(EUR, "Ibuki", "Nemoto", "Nemoto", 24, "JPN", 172, 67, RIGHT, LW, 0, 58, 66, 200000, 50, 2000000, 4, 15);
        AddSimple(EUR, "Adnane", "Ghailan", "Ghailan", 25, "MAR", 182, 76, RIGHT, ST, 0, 60, 68, 250000, 62, 2500000, 4, 16);
        AddSimple(EUR, "Adam", "El Mokhles", "El Mokhles", 23, "MAR", 180, 75, RIGHT, ST, LW_f, 58, 68, 150000, 30, 1500000, 4, 17);
        AddSimple(EUR, "Julián", "Mahicas", "Mahicas", 26, "ESP", 183, 76, RIGHT, ST, 0, 60, 66, 200000, 50, 2000000, 5, 18);
        AddSimple(EUR, "Khalid", "Noureddine", "Noureddine", 24, "MAR", 184, 77, RIGHT, ST, 0, 58, 68, 150000, 30, 1500000, 4, 19);

        // ══════════════════════════════════════════════════════════
        // Águilas FC (TeamId=78)
        // ══════════════════════════════════════════════════════════
        int AGU = 78;
        AddSimple(AGU, "Nico", "Rodríguez", "N. Rodríguez", 27, "ESP", 186, 85, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(AGU, "José", "Salcedo", "Salcedo", 24, "ESP", 188, 86, RIGHT, GK, 0, 58, 66, 100000, 20, 1000000, 4, 13);
        AddSimple(AGU, "Antonio", "Sánchez", "A. Sánchez", 22, "ESP", 185, 85, RIGHT, GK, 0, 56, 68, 75000, 15, 750000, 4, 25);
        AddSimple(AGU, "Uriel", "Jové", "Jové", 26, "ESP", 184, 77, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 5, 4);
        AddSimple(AGU, "Ebuka", "Nwenyi", "Nwenyi", 24, "NGA", 187, 78, RIGHT, CB, 0, 58, 68, 150000, 30, 1500000, 4, 5);
        AddSimple(AGU, "Héctor", "Martínez", "H. Martínez", 27, "ESP", 183, 76, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 14);
        AddSimple(AGU, "Diego", "Espinosa", "Espinosa", 25, "ESP", 186, 78, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 15);
        AddSimple(AGU, "Johan", "Terranova", "Terranova", 23, "ESP", 175, 70, RIGHT, LB, 0, 60, 70, 200000, 50, 2000000, 4, 3);
        AddSimple(AGU, "José", "Mas", "Mas", 26, "ESP", 177, 70, RIGHT, LB, 0, 58, 64, 100000, 20, 1000000, 4, 16);
        AddSimple(AGU, "Aitor", "Hidalgo", "Hidalgo", 27, "ESP", 178, 71, RIGHT, RB, 0, 60, 66, 200000, 50, 2000000, 5, 2);
        AddSimple(AGU, "Mario", "Abenza", "Abenza", 24, "ESP", 177, 70, RIGHT, RB, 0, 58, 66, 100000, 20, 1000000, 4, 17);
        AddSimple(AGU, "Adrián", "Pérez", "Adrián P.", 26, "ESP", 182, 72, RIGHT, CDM, CM_f, 60, 66, 200000, 50, 2000000, 5, 6);
        AddSimple(AGU, "Yasser", "El Arbaoui", "El Arbaoui", 24, "MAR", 180, 72, RIGHT, CDM, 0, 58, 68, 150000, 30, 1500000, 4, 18);
        AddSimple(AGU, "Alonso", "Nyada", "Nyada", 25, "CMR", 183, 73, RIGHT, CM, 0, 58, 66, 150000, 30, 1500000, 5, 19);
        AddSimple(AGU, "Javi", "Soto", "Soto", 26, "ESP", 178, 71, RIGHT, CM, 0, 60, 66, 150000, 30, 1500000, 5, 8);
        AddSimple(AGU, "Javi", "Pedrosa", "Pedrosa", 25, "ESP", 179, 71, RIGHT, CM, 0, 58, 66, 100000, 20, 1000000, 4, 10);
        AddSimple(AGU, "Pipo", "", "Pipo", 28, "ESP", 175, 68, RIGHT, CAM, CM_f, 58, 64, 150000, 30, 1500000, 5, 20);
        AddSimple(AGU, "Kevin", "Manzano", "Manzano", 24, "ESP", 176, 68, RIGHT, LW, RW_f, 58, 66, 100000, 20, 1000000, 4, 11);
        AddSimple(AGU, "Javi", "Castedo", "Castedo", 26, "ESP", 178, 69, RIGHT, RW, LW_f, 60, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(AGU, "Fer", "Martínez", "F. Martínez", 25, "ESP", 175, 68, RIGHT, LW, 0, 58, 64, 100000, 20, 1000000, 4, 21);
        AddSimple(AGU, "Christian", "Martínez", "C. Martínez", 27, "ESP", 174, 67, RIGHT, RW, 0, 58, 64, 100000, 20, 1000000, 4, 12);
        AddSimple(AGU, "Seth", "Vega", "Vega", 24, "ESP", 180, 75, RIGHT, ST, 0, 60, 68, 250000, 62, 2500000, 4, 9);
        AddSimple(AGU, "Boris", "Kouassi", "Kouassi", 25, "CIV", 183, 76, RIGHT, ST, 0, 58, 66, 150000, 30, 1500000, 4, 22);

        // ══════════════════════════════════════════════════════════
        // CD Teruel (TeamId=79)
        // ══════════════════════════════════════════════════════════
        int TER = 79;
        AddSimple(TER, "Rubén", "Gálvez", "R. Gálvez", 26, "ESP", 186, 85, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(TER, "Alejandro", "Palop", "Palop", 24, "ESP", 188, 86, RIGHT, GK, 0, 58, 66, 100000, 20, 1000000, 4, 13);
        AddSimple(TER, "Andrés", "Rodríguez", "A. Rodríguez", 22, "ESP", 185, 85, RIGHT, GK, 0, 56, 68, 75000, 15, 750000, 4, 25);
        AddSimple(TER, "Nico", "Van Rijn", "Van Rijn", 25, "NED", 184, 77, RIGHT, CB, 0, 60, 68, 250000, 62, 2500000, 5, 4);
        AddSimple(TER, "Abraham", "del Moral", "Del Moral", 27, "ESP", 183, 76, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 5, 5);
        AddSimple(TER, "Nathaniel", "Nicholas", "Nicholas", 24, "NGA", 186, 78, RIGHT, CB, 0, 58, 68, 150000, 30, 1500000, 4, 14);
        AddSimple(TER, "Carlos", "Vigaray", "Vigaray", 31, "ESP", 178, 71, RIGHT, LB, CB_f, 58, 64, 100000, 20, 1000000, 5, 3);
        AddSimple(TER, "Goyo", "Medina", "Medina", 26, "ESP", 175, 70, RIGHT, LB, 0, 58, 64, 100000, 20, 1000000, 4, 16);
        AddSimple(TER, "Manel", "Royo", "Royo", 27, "ESP", 178, 71, RIGHT, RB, 0, 60, 66, 200000, 50, 2000000, 5, 2);
        AddSimple(TER, "David", "Fondarella", "Fondarella", 25, "ESP", 177, 70, RIGHT, RB, 0, 58, 64, 100000, 20, 1000000, 4, 17);
        AddSimple(TER, "Joseda", "Menargues", "Menargues", 26, "ESP", 182, 72, RIGHT, CDM, CM_f, 60, 66, 200000, 50, 2000000, 5, 6);
        AddSimple(TER, "Álex", "Blesa", "Blesa", 24, "ESP", 180, 72, RIGHT, CDM, 0, 58, 66, 100000, 20, 1000000, 4, 18);
        AddSimple(TER, "Relu", "", "Relu", 27, "ESP", 180, 72, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(TER, "Hugo", "Redón", "Redón", 23, "ESP", 178, 71, RIGHT, CM, 0, 58, 68, 100000, 20, 1000000, 4, 10);
        AddSimple(TER, "Haritz", "Albisua", "Albisua", 25, "ESP", 179, 71, RIGHT, CM, 0, 58, 66, 100000, 20, 1000000, 4, 19);
        AddSimple(TER, "Mateo", "Enríquez", "Enríquez", 24, "ESP", 176, 68, RIGHT, CAM, CM_f, 60, 68, 200000, 50, 2000000, 4, 11);
        AddSimple(TER, "Álvaro", "Martí", "Martí", 26, "ESP", 175, 68, RIGHT, CAM, 0, 58, 64, 100000, 20, 1000000, 4, 20);
        AddSimple(TER, "Rafaël", "De Palmas", "De Palmas", 25, "ESP", 180, 70, RIGHT, LW, RW_f, 60, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(TER, "Álvaro", "Merencio", "Merencio", 24, "ESP", 174, 67, RIGHT, RW, LW_f, 58, 66, 150000, 30, 1500000, 4, 12);
        AddSimple(TER, "Teddy", "Sutherland", "Sutherland", 25, "ENG", 178, 69, RIGHT, LW, 0, 58, 66, 150000, 30, 1500000, 4, 21);
        AddSimple(TER, "Mamadou", "Traoré", "Traoré", 26, "MLI", 182, 76, RIGHT, ST, 0, 60, 66, 200000, 50, 2000000, 5, 9);
        AddSimple(TER, "Sergio", "Moreno", "S. Moreno", 24, "ESP", 183, 76, RIGHT, ST, 0, 58, 66, 100000, 20, 1000000, 4, 22);
        AddSimple(TER, "Iván", "Ramos", "I. Ramos", 23, "ESP", 180, 75, RIGHT, ST, 0, 58, 68, 100000, 20, 1000000, 4, 23);
        AddSimple(TER, "Lolo", "Plá", "Plá", 27, "ESP", 181, 75, RIGHT, ST, CF_f, 60, 66, 200000, 50, 2000000, 5, 24);

        // ══════════════════════════════════════════════════════════
        // CF Torremolinos (TeamId=80)
        // ══════════════════════════════════════════════════════════
        int TOR = 80;
        AddSimple(TOR, "Javi", "Cuenca", "Cuenca", 26, "ESP", 185, 85, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(TOR, "Fran", "Martínez", "F. Martínez", 24, "ESP", 188, 86, RIGHT, GK, 0, 58, 66, 100000, 20, 1000000, 4, 13);
        AddSimple(TOR, "Bernabé", "Barragán", "Barragán", 22, "ESP", 186, 85, RIGHT, GK, 0, 56, 68, 75000, 15, 750000, 4, 25);
        AddSimple(TOR, "Javi", "Mérida", "Mérida", 32, "ESP", 184, 77, RIGHT, CB, 0, 60, 64, 150000, 30, 1500000, 5, 4);
        AddSimple(TOR, "Dani", "Fernández", "D. Fernández", 26, "ESP", 183, 76, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 5);
        AddSimple(TOR, "Adrián", "Salguero", "Salguero", 25, "ESP", 186, 78, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 14);
        AddSimple(TOR, "Jon A.", "Amelibia", "Amelibia", 27, "ESP", 185, 77, RIGHT, CB, 0, 58, 64, 75000, 15, 750000, 4, 15);
        AddSimple(TOR, "Nico", "Delmonte", "Delmonte", 25, "ESP", 176, 70, RIGHT, LB, CB_f, 60, 66, 200000, 50, 2000000, 5, 3);
        AddSimple(TOR, "Fran", "García", "F. García", 26, "ESP", 175, 70, RIGHT, LB, 0, 58, 64, 100000, 20, 1000000, 4, 16);
        AddSimple(TOR, "Sergio", "Díaz", "S. Díaz", 27, "ESP", 178, 71, RIGHT, RB, 0, 60, 66, 200000, 50, 2000000, 5, 2);
        AddSimple(TOR, "Edu", "López", "E. López", 24, "ESP", 177, 70, RIGHT, RB, 0, 58, 66, 100000, 20, 1000000, 4, 17);
        AddSimple(TOR, "Rodrigo", "Ajegun", "Ajegun", 25, "NGA", 182, 72, RIGHT, CDM, CM_f, 60, 68, 250000, 62, 2500000, 5, 6);
        AddSimple(TOR, "Ángel", "Climent", "Climent", 26, "ESP", 180, 72, RIGHT, CDM, 0, 58, 64, 100000, 20, 1000000, 4, 18);
        AddSimple(TOR, "Cristóbal", "Moreno", "C. Moreno", 27, "ESP", 180, 72, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(TOR, "Gori", "Gracia", "G. Gracia", 26, "ESP", 178, 71, RIGHT, CM, 0, 58, 64, 100000, 20, 1000000, 4, 10);
        AddSimple(TOR, "Lesedi", "Steve", "Steve", 24, "RSA", 179, 71, RIGHT, CM, 0, 58, 68, 150000, 30, 1500000, 4, 19);
        AddSimple(TOR, "Fran", "Gallego", "Gallego", 25, "ESP", 176, 68, RIGHT, CAM, CM_f, 60, 66, 200000, 50, 2000000, 5, 11);
        AddSimple(TOR, "Alex", "Camacho", "Camacho", 24, "ESP", 175, 68, RIGHT, CAM, 0, 58, 66, 100000, 20, 1000000, 4, 20);
        AddSimple(TOR, "Pau", "Pérez", "Pau P.", 25, "ESP", 180, 70, RIGHT, LW, RW_f, 60, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(TOR, "Isaac", "González", "I. González", 24, "ESP", 174, 67, RIGHT, RW, LW_f, 58, 66, 150000, 30, 1500000, 4, 12);
        AddSimple(TOR, "Iván", "Laca", "Laca", 26, "ESP", 178, 69, RIGHT, LW, 0, 58, 64, 100000, 20, 1000000, 4, 21);
        AddSimple(TOR, "Christian", "Albert", "Albert", 25, "FRA", 182, 76, RIGHT, ST, 0, 60, 68, 250000, 62, 2500000, 5, 9);
        AddSimple(TOR, "Ibán", "Ribeiro", "Ribeiro", 24, "ESP", 183, 76, RIGHT, ST, 0, 58, 68, 100000, 20, 1000000, 4, 22);
        AddSimple(TOR, "Peque", "Polo", "Polo", 23, "ESP", 180, 75, RIGHT, ST, CF_f, 58, 68, 100000, 20, 1000000, 4, 23);
        AddSimple(TOR, "Pito", "Camacho", "Camacho", 25, "ESP", 181, 75, RIGHT, ST, 0, 58, 66, 100000, 20, 1000000, 4, 24);

        // ══════════════════════════════════════════════════════════
        // UE Sant Andreu (TeamId=81)
        // ══════════════════════════════════════════════════════════
        int SAN = 81;
        AddSimple(SAN, "Iñaki", "Álvarez", "Álvarez", 27, "ESP", 186, 85, RIGHT, GK, 0, 60, 66, 200000, 50, 2000000, 5, 1);
        AddSimple(SAN, "Raúl", "García-Alejo", "García-Alejo", 24, "ESP", 185, 85, RIGHT, GK, 0, 58, 66, 100000, 20, 1000000, 4, 13);
        AddSimple(SAN, "Andreu", "Hernández", "A. Hernández", 22, "ESP", 188, 86, RIGHT, GK, 0, 56, 68, 75000, 15, 750000, 4, 25);
        AddSimple(SAN, "Javi", "Gómez", "J. Gómez", 27, "ESP", 184, 77, RIGHT, CB, 0, 60, 66, 200000, 50, 2000000, 5, 4);
        AddSimple(SAN, "Carlos", "Blanco", "Blanco", 26, "ESP", 183, 76, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 5);
        AddSimple(SAN, "Lucas", "Viña", "Viña", 25, "ESP", 186, 78, RIGHT, CB, 0, 58, 64, 100000, 20, 1000000, 4, 14);
        AddSimple(SAN, "Rubén", "Bonachera", "Bonachera", 27, "ESP", 176, 70, RIGHT, LB, CB_f, 60, 66, 200000, 50, 2000000, 5, 3);
        AddSimple(SAN, "Jordi", "Méndez", "Méndez", 25, "ESP", 175, 70, RIGHT, LB, 0, 58, 66, 100000, 20, 1000000, 4, 16);
        AddSimple(SAN, "Pablo", "Santiago", "Santiago", 27, "ESP", 178, 71, RIGHT, RB, 0, 60, 66, 200000, 50, 2000000, 5, 2);
        AddSimple(SAN, "Dani", "Torices", "Torices", 25, "ESP", 177, 70, RIGHT, RB, 0, 58, 66, 100000, 20, 1000000, 4, 17);
        AddSimple(SAN, "Noel", "Carbonell", "Carbonell", 26, "ESP", 182, 72, RIGHT, CDM, CM_f, 60, 66, 200000, 50, 2000000, 5, 6);
        AddSimple(SAN, "Pau", "Darbra", "Darbra", 24, "ESP", 180, 72, RIGHT, CDM, 0, 58, 66, 100000, 20, 1000000, 4, 18);
        AddSimple(SAN, "Albertito", "", "Albertito", 27, "ESP", 180, 72, RIGHT, CM, 0, 60, 66, 200000, 50, 2000000, 5, 8);
        AddSimple(SAN, "Sergi", "García", "S. García", 26, "ESP", 178, 71, RIGHT, CM, 0, 58, 64, 100000, 20, 1000000, 4, 10);
        AddSimple(SAN, "Josep", "Señé", "Señé", 34, "ESP", 179, 71, RIGHT, CM, 0, 58, 62, 75000, 15, 750000, 5, 19);
        AddSimple(SAN, "Sergi", "Serrano", "S. Serrano", 25, "ESP", 176, 68, RIGHT, CAM, CM_f, 60, 66, 200000, 50, 2000000, 5, 11);
        AddSimple(SAN, "Bouba", "Barry", "Barry", 26, "SEN", 180, 72, RIGHT, CM, CDM_f, 58, 64, 100000, 20, 1000000, 4, 20);
        AddSimple(SAN, "Alexis", "García", "Alexis G.", 25, "ESP", 175, 68, RIGHT, CAM, 0, 58, 66, 100000, 20, 1000000, 4, 21);
        AddSimple(SAN, "Armand", "Vallés", "Vallés", 27, "ESP", 180, 70, RIGHT, LW, RW_f, 60, 66, 200000, 50, 2000000, 5, 7);
        AddSimple(SAN, "Max", "Marcet", "Marcet", 24, "ESP", 174, 67, RIGHT, RW, LW_f, 58, 66, 150000, 30, 1500000, 4, 12);
        AddSimple(SAN, "Marcos", "Mendes", "Mendes", 25, "BRA", 178, 69, RIGHT, LW, 0, 58, 66, 150000, 30, 1500000, 4, 22);
        AddSimple(SAN, "Walid", "Meddeb", "Meddeb", 26, "TUN", 182, 76, RIGHT, ST, 0, 60, 68, 250000, 62, 2500000, 5, 9);

        // ══════════════════════════════════════════════════════════
        // CF Rayo Majadahonda (TeamId=82)
        // ══════════════════════════════════════════════════════════
        int MAJ = 82;
        AddSimple(MAJ, "Miguel", "Prieto", "Prieto", 24, "ESP", 184, 80, RIGHT, GK, 0, 62, 70, 400000, 100, 4000000, 5, 13);
        AddSimple(MAJ, "Josan", "Gordo", "Gordo", 21, "ESP", 188, 78, RIGHT, GK, 0, 60, 72, 300000, 75, 3000000, 4, 1);
        AddSimple(MAJ, "Savo", "Veinovich", "Veinovich", 20, "ARG", 190, 70, RIGHT, GK, 0, 58, 74, 250000, 62, 2500000, 3, 25);
        AddSimple(MAJ, "Dani", "Ramos", "D. Ramos", 31, "ESP", 185, 78, RIGHT, CB, 0, 62, 66, 400000, 100, 4000000, 5, 4);
        AddSimple(MAJ, "Isma", "Aizpiri", "Aizpiri", 28, "ESP", 182, 72, RIGHT, CB, 0, 62, 68, 350000, 87, 3500000, 5, 5);
        AddSimple(MAJ, "Álvaro", "Télis", "Télis", 29, "URU", 193, 79, RIGHT, CB, 0, 60, 66, 300000, 75, 3000000, 5, 18);
        AddSimple(MAJ, "Juan", "Durán", "Durán", 25, "ESP", 180, 74, RIGHT, CB, LB_f, 60, 68, 250000, 62, 2500000, 4, 23);
        AddSimple(MAJ, "Sergio", "Carmona", "Carmona", 21, "ESP", 184, 74, RIGHT, CB, 0, 58, 72, 200000, 50, 2000000, 3, 20);
        AddSimple(MAJ, "Álex", "García", "Á. García", 23, "ESP", 194, 80, RIGHT, LB, CB_f, 60, 72, 350000, 87, 3500000, 4, 2);
        AddSimple(MAJ, "Javi", "Gordo", "J. Gordo", 21, "ESP", 179, 70, RIGHT, LB, 0, 58, 72, 200000, 50, 2000000, 3, 27);
        AddSimple(MAJ, "Miguel", "Acosta", "Acosta", 28, "ESP", 175, 72, RIGHT, RB, 0, 60, 66, 250000, 62, 2500000, 5, 15);
        AddSimple(MAJ, "Dani", "Vidal", "Vidal", 26, "ESP", 180, 70, RIGHT, CDM, CB_f, 62, 68, 350000, 87, 3500000, 5, 6);
        AddSimple(MAJ, "Ato", "", "Ato", 27, "ESP", 183, 78, RIGHT, CM, CDM_f, 60, 66, 250000, 62, 2500000, 5, 21);
        AddSimple(MAJ, "Koke", "San José", "San José", 25, "ESP", 178, 71, RIGHT, CM, 0, 60, 68, 250000, 62, 2500000, 4, 17);
        AddSimple(MAJ, "Antonio", "Amaro", "Amaro", 34, "ESP", 178, 75, RIGHT, CM, CDM_f, 60, 64, 200000, 50, 2000000, 5, 8);
        AddSimple(MAJ, "Diego", "Rodríguez", "D. Rodríguez", 21, "ESP", 186, 75, RIGHT, CM, 0, 58, 74, 200000, 50, 2000000, 3, 22);
        AddSimple(MAJ, "Yuya", "Yoshimura", "Yoshimura", 24, "JPN", 172, 67, RIGHT, CAM, CM_f, 60, 68, 300000, 75, 3000000, 4, 10);
        AddSimple(MAJ, "Dani", "Plomer", "Plomer", 26, "ESP", 180, 72, RIGHT, RW, LW_f, 62, 68, 400000, 100, 4000000, 5, 10);
        AddSimple(MAJ, "Javi", "Martín", "J. Martín", 25, "ESP", 178, 69, RIGHT, LW, RW_f, 60, 68, 300000, 75, 3000000, 4, 16);
        AddSimple(MAJ, "Fran", "Pérez", "F. Pérez", 27, "ESP", 174, 67, RIGHT, RW, LW_f, 60, 66, 250000, 62, 2500000, 5, 19);
        AddSimple(MAJ, "Pol", "Prats", "Prats", 24, "ESP", 176, 68, RIGHT, LW, 0, 60, 70, 300000, 75, 3000000, 4, 11);
        AddSimple(MAJ, "Iñigo", "Ramos", "I. Ramos", 23, "ESP", 178, 69, RIGHT, RW, 0, 58, 70, 200000, 50, 2000000, 4, 28);
        AddSimple(MAJ, "Ilies", "Faure", "Faure", 26, "FRA", 183, 76, RIGHT, ST, LW_f, 62, 70, 500000, 142, 5000000, 5, 9);
        AddSimple(MAJ, "Gonzalo", "Calçada", "Calçada", 24, "ESP", 182, 76, RIGHT, ST, 0, 60, 68, 300000, 75, 3000000, 4, 24);

        // ══════════════════════════════════════════════════════════════
        // UEFA CHAMPIONS LEAGUE (LeagueId=5) — 15 equipos
        // ══════════════════════════════════════════════════════════════

        // ══════════════════════════════════════════════════════════════
        // Liverpool FC (TeamId=83)
        // ══════════════════════════════════════════════════════════════
        int LIV = 83;
        AddSimple(LIV, "Giorgi", "Mamardashvili", "Mamardashvili", 24, "GEO", 197, 90, LEFT, GK, 0, 82, 88, 30000000, 8000, 60000000, 6, 1);
        AddSimple(LIV, "Alisson", "Becker", "Alisson", 32, "BRA", 191, 91, RIGHT, GK, 0, 89, 89, 35000000, 12000, 70000000, 3, 13);
        AddSimple(LIV, "Freddie", "Woodman", "Woodman", 28, "ENG", 188, 85, RIGHT, GK, 0, 72, 74, 2000000, 500, 4000000, 3, 22);
        AddSimple(LIV, "Harvey", "Davies", "H. Davies", 21, "ENG", 185, 83, RIGHT, GK, 0, 65, 78, 800000, 100, 1500000, 4, 30);
        AddSimple(LIV, "Giovanni", "Leoni", "Leoni", 18, "ITA", 192, 85, RIGHT, GK, 0, 60, 80, 500000, 50, 800000, 5, 41);
        AddSimple(LIV, "Virgil", "van Dijk", "van Dijk", 33, "NED", 193, 92, RIGHT, CB, 0, 90, 90, 40000000, 15000, 80000000, 3, 4);
        AddSimple(LIV, "Joe", "Gomez", "Gomez", 28, "ENG", 188, 82, RIGHT, CB, RB_f, 80, 82, 22000000, 6000, 45000000, 4, 2);
        AddSimple(LIV, "Rhys", "Williams", "R. Williams", 24, "ENG", 195, 84, RIGHT, CB, 0, 68, 74, 3000000, 500, 6000000, 3, 47);
        AddSimple(LIV, "Milos", "Kerkez", "Kerkez", 21, "HUN", 180, 75, LEFT, LB, LWB_f, 78, 86, 25000000, 5000, 50000000, 5, 21);
        AddSimple(LIV, "Andrew", "Robertson", "Robertson", 31, "SCO", 178, 75, LEFT, LB, 0, 85, 85, 30000000, 10000, 60000000, 3, 26);
        AddSimple(LIV, "Jeremie", "Frimpong", "Frimpong", 24, "NED", 173, 68, RIGHT, RB, RWB_f, 84, 88, 35000000, 7000, 70000000, 5, 66);
        AddSimple(LIV, "Conor", "Bradley", "Bradley", 21, "NIR", 181, 76, RIGHT, RB, LB_f, 74, 84, 15000000, 2500, 30000000, 5, 84);
        AddSimple(LIV, "Calvin", "Ramsay", "Ramsay", 21, "SCO", 178, 72, RIGHT, RB, 0, 66, 76, 3000000, 500, 6000000, 4, 83);
        AddSimple(LIV, "Ryan", "Gravenberch", "Gravenberch", 23, "NED", 190, 82, RIGHT, CM, CDM_f, 82, 88, 35000000, 7000, 70000000, 5, 38);
        AddSimple(LIV, "Wataru", "Endo", "Endo", 32, "JPN", 178, 75, RIGHT, CDM, CM_f, 80, 80, 12000000, 5000, 25000000, 2, 3);
        AddSimple(LIV, "Stefan", "Bajcetic", "Bajcetic", 20, "ESP", 185, 78, RIGHT, CDM, CM_f, 74, 86, 18000000, 2000, 35000000, 5, 43);
        AddSimple(LIV, "Alexis", "Mac Allister", "Mac Allister", 26, "ARG", 176, 72, RIGHT, CM, CAM_f, 86, 88, 45000000, 12000, 90000000, 4, 10);
        AddSimple(LIV, "Curtis", "Jones", "Jones", 24, "ENG", 185, 80, RIGHT, CM, CAM_f, 81, 85, 28000000, 6000, 55000000, 5, 17);
        AddSimple(LIV, "Trey", "Nyoni", "Nyoni", 18, "ENG", 175, 68, RIGHT, CM, CAM_f, 62, 85, 2000000, 100, 4000000, 5, 42);
        AddSimple(LIV, "Florian", "Wirtz", "Wirtz", 22, "GER", 176, 72, RIGHT, CAM, LW_f, 87, 92, 80000000, 10000, 160000000, 5, 11);
        AddSimple(LIV, "Dominik", "Szoboszlai", "Szoboszlai", 24, "HUN", 186, 78, RIGHT, CM, CAM_f, 84, 88, 40000000, 8000, 80000000, 5, 8);
        AddSimple(LIV, "Cody", "Gakpo", "Gakpo", 26, "NED", 193, 84, RIGHT, LW, ST_f, 83, 86, 35000000, 8000, 70000000, 4, 18);
        AddSimple(LIV, "Rio", "Ngumoha", "Ngumoha", 17, "ENG", 175, 67, RIGHT, LW, RW_f, 55, 85, 1500000, 50, 3000000, 5, 50);
        AddSimple(LIV, "Federico", "Chiesa", "Chiesa", 27, "ITA", 175, 72, LEFT, RW, LW_f, 83, 84, 25000000, 7000, 50000000, 3, 14);
        AddSimple(LIV, "Alexander", "Isak", "Isak", 25, "SWE", 192, 84, RIGHT, ST, 0, 88, 90, 80000000, 12000, 160000000, 5, 9);
        AddSimple(LIV, "Hugo", "Ekitiké", "Ekitiké", 22, "FRA", 189, 82, RIGHT, ST, CF_f, 80, 86, 30000000, 5000, 60000000, 5, 19);

        // ══════════════════════════════════════════════════════════════
        // Arsenal FC (TeamId=84)
        // ══════════════════════════════════════════════════════════════
        int ARS = 84;
        AddSimple(ARS, "David", "Raya", "Raya", 29, "ESP", 183, 84, RIGHT, GK, 0, 85, 86, 28000000, 8000, 55000000, 4, 22);
        AddSimple(ARS, "Kepa", "Arrizabalaga", "Kepa", 30, "ESP", 186, 85, RIGHT, GK, 0, 78, 79, 8000000, 4000, 15000000, 3, 1);
        AddSimple(ARS, "William", "Saliba", "Saliba", 24, "FRA", 192, 86, RIGHT, CB, 0, 87, 91, 55000000, 10000, 110000000, 5, 2);
        AddSimple(ARS, "Gabriel", "Dos Santos", "Gabriel", 27, "BRA", 190, 84, LEFT, CB, 0, 86, 88, 45000000, 9000, 90000000, 4, 6);
        AddSimple(ARS, "Piero", "Hincapié", "Hincapié", 23, "ECU", 184, 78, LEFT, CB, LB_f, 82, 88, 35000000, 6000, 70000000, 5, 3);
        AddSimple(ARS, "Cristhian", "Mosquera", "Mosquera", 21, "ESP", 188, 82, RIGHT, CB, 0, 74, 84, 15000000, 2500, 30000000, 5, 4);
        AddSimple(ARS, "Riccardo", "Calafiori", "Calafiori", 23, "ITA", 188, 80, LEFT, LB, CB_f, 80, 86, 30000000, 5000, 60000000, 5, 33);
        AddSimple(ARS, "Jurriën", "Timber", "Timber", 24, "NED", 182, 78, RIGHT, CB, RB_f, 83, 87, 40000000, 7000, 80000000, 4, 12);
        AddSimple(ARS, "Ben", "White", "White", 27, "ENG", 185, 78, RIGHT, RB, CB_f, 84, 85, 35000000, 8000, 70000000, 4, 4);
        AddSimple(ARS, "Martín", "Zubimendi", "Zubimendi", 26, "ESP", 181, 78, RIGHT, CDM, CM_f, 84, 87, 45000000, 8000, 90000000, 5, 5);
        AddSimple(ARS, "Christian", "Nørgaard", "Nørgaard", 31, "DEN", 187, 80, RIGHT, CDM, CM_f, 80, 81, 15000000, 6000, 30000000, 3, 29);
        AddSimple(ARS, "Declan", "Rice", "Rice", 26, "ENG", 185, 82, RIGHT, CDM, CM_f, 88, 89, 70000000, 14000, 140000000, 5, 41);
        AddSimple(ARS, "Myles", "Lewis-Skelly", "Lewis-Skelly", 18, "ENG", 178, 72, LEFT, CM, LB_f, 65, 85, 5000000, 200, 10000000, 5, 49);
        AddSimple(ARS, "Mikel", "Merino", "Merino", 29, "ESP", 189, 83, LEFT, CM, CDM_f, 83, 84, 25000000, 7000, 50000000, 3, 23);
        AddSimple(ARS, "Martin", "Ødegaard", "Ødegaard", 26, "NOR", 178, 74, LEFT, CAM, CM_f, 87, 89, 60000000, 12000, 120000000, 4, 8);
        AddSimple(ARS, "Eberechi", "Eze", "Eze", 27, "ENG", 178, 72, RIGHT, CAM, LW_f, 84, 85, 35000000, 8000, 70000000, 4, 10);
        AddSimple(ARS, "Gabriel", "Martinelli", "Martinelli", 24, "BRA", 178, 72, LEFT, LW, RW_f, 84, 87, 45000000, 8000, 90000000, 5, 11);
        AddSimple(ARS, "Leandro", "Trossard", "Trossard", 30, "BEL", 172, 68, RIGHT, LW, ST_f, 82, 83, 20000000, 7000, 40000000, 3, 19);
        AddSimple(ARS, "Bukayo", "Saka", "Saka", 23, "ENG", 178, 72, LEFT, RW, LW_f, 89, 92, 100000000, 15000, 200000000, 5, 7);
        AddSimple(ARS, "Noni", "Madueke", "Madueke", 23, "ENG", 176, 70, LEFT, RW, LW_f, 80, 85, 25000000, 5000, 50000000, 5, 20);
        AddSimple(ARS, "Max", "Dowman", "Dowman", 17, "ENG", 180, 70, RIGHT, CAM, CM_f, 55, 85, 1000000, 50, 2000000, 5, 55);
        AddSimple(ARS, "Viktor", "Gyökeres", "Gyökeres", 27, "SWE", 187, 84, RIGHT, ST, 0, 87, 88, 60000000, 10000, 120000000, 4, 9);
        AddSimple(ARS, "Kai", "Havertz", "Havertz", 26, "GER", 193, 85, LEFT, ST, CAM_f, 83, 86, 40000000, 10000, 80000000, 4, 29);
        AddSimple(ARS, "Gabriel", "Jesus", "G. Jesus", 28, "BRA", 175, 72, RIGHT, ST, RW_f, 83, 84, 30000000, 9000, 60000000, 3, 9);

        // ══════════════════════════════════════════════════════════════
        // Manchester City FC (TeamId=85)
        // ══════════════════════════════════════════════════════════════
        int MCI = 85;
        AddSimple(MCI, "Gianluigi", "Donnarumma", "Donnarumma", 26, "ITA", 196, 90, LEFT, GK, 0, 90, 92, 80000000, 15000, 160000000, 5, 1);
        AddSimple(MCI, "James", "Trafford", "Trafford", 22, "ENG", 192, 85, RIGHT, GK, 0, 72, 84, 12000000, 2000, 25000000, 5, 18);
        AddSimple(MCI, "Marcus", "Bettinelli", "Bettinelli", 33, "ENG", 194, 86, RIGHT, GK, 0, 70, 70, 1000000, 500, 2000000, 2, 33);
        AddSimple(MCI, "Marc", "Guéhi", "Guéhi", 25, "ENG", 182, 78, LEFT, CB, 0, 83, 86, 35000000, 7000, 70000000, 5, 6);
        AddSimple(MCI, "Josko", "Gvardiol", "Gvardiol", 23, "CRO", 185, 80, LEFT, CB, LB_f, 87, 91, 65000000, 10000, 130000000, 5, 24);
        AddSimple(MCI, "Rúben", "Dias", "Rúben Dias", 28, "POR", 186, 82, RIGHT, CB, 0, 89, 90, 55000000, 12000, 110000000, 4, 3);
        AddSimple(MCI, "Abdukodir", "Khusanov", "Khusanov", 21, "UZB", 186, 80, RIGHT, CB, 0, 74, 84, 15000000, 2000, 30000000, 5, 5);
        AddSimple(MCI, "John", "Stones", "Stones", 31, "ENG", 188, 80, RIGHT, CB, CDM_f, 85, 86, 30000000, 10000, 60000000, 3, 5);
        AddSimple(MCI, "Nathan", "Aké", "Aké", 30, "NED", 180, 76, LEFT, CB, LB_f, 83, 84, 22000000, 8000, 45000000, 3, 6);
        AddSimple(MCI, "Max", "Alleyne", "Alleyne", 19, "ENG", 188, 80, RIGHT, CB, 0, 60, 82, 2000000, 100, 4000000, 5, 82);
        AddSimple(MCI, "Nico", "O'Reilly", "O'Reilly", 20, "ENG", 185, 78, LEFT, CM, LB_f, 65, 82, 4000000, 200, 8000000, 5, 75);
        AddSimple(MCI, "Rayan", "Aït-Nouri", "Aït-Nouri", 24, "ALG", 180, 75, LEFT, LB, LWB_f, 80, 84, 25000000, 5000, 50000000, 5, 25);
        AddSimple(MCI, "Matheus", "Nunes", "Nunes", 26, "POR", 183, 76, RIGHT, CM, 0, 81, 84, 30000000, 7000, 60000000, 4, 27);
        AddSimple(MCI, "Rico", "Lewis", "Lewis", 20, "ENG", 169, 65, RIGHT, RB, CM_f, 78, 88, 30000000, 4000, 60000000, 5, 82);
        AddSimple(MCI, "Rodri", "Hernández", "Rodri", 29, "ESP", 190, 85, RIGHT, CDM, CM_f, 91, 92, 100000000, 20000, 200000000, 4, 16);
        AddSimple(MCI, "Nico", "González", "N. González", 23, "ESP", 186, 80, RIGHT, CM, CDM_f, 80, 86, 30000000, 5000, 60000000, 5, 14);
        AddSimple(MCI, "Kalvin", "Phillips", "Phillips", 29, "ENG", 178, 76, RIGHT, CDM, CM_f, 76, 78, 8000000, 5000, 15000000, 2, 4);
        AddSimple(MCI, "Tijjani", "Reijnders", "Reijnders", 26, "NED", 185, 78, RIGHT, CM, CAM_f, 84, 86, 40000000, 8000, 80000000, 4, 8);
        AddSimple(MCI, "Sverre", "Nypan", "Nypan", 18, "NOR", 180, 75, RIGHT, CM, CAM_f, 62, 85, 3000000, 100, 6000000, 5, 42);
        AddSimple(MCI, "Mateo", "Kovacic", "Kovacic", 31, "CRO", 177, 75, RIGHT, CM, 0, 83, 84, 20000000, 8000, 40000000, 2, 8);
        AddSimple(MCI, "Rayan", "Cherki", "Cherki", 21, "FRA", 176, 72, RIGHT, CAM, LW_f, 80, 88, 30000000, 4000, 60000000, 5, 28);
        AddSimple(MCI, "Phil", "Foden", "Foden", 25, "ENG", 171, 70, LEFT, RW, CM_f, 88, 90, 80000000, 15000, 160000000, 5, 47);
        AddSimple(MCI, "Jérémy", "Doku", "Doku", 23, "BEL", 173, 68, RIGHT, RW, LW_f, 84, 88, 50000000, 8000, 100000000, 5, 11);
        AddSimple(MCI, "Savinho", "", "Savinho", 21, "BRA", 176, 70, LEFT, RW, LW_f, 80, 86, 30000000, 4000, 60000000, 5, 26);
        AddSimple(MCI, "Antoine", "Semenyo", "Semenyo", 25, "GHA", 185, 80, RIGHT, ST, RW_f, 80, 84, 25000000, 5000, 50000000, 4, 10);
        AddSimple(MCI, "Erling", "Haaland", "Haaland", 25, "NOR", 194, 88, LEFT, ST, 0, 92, 93, 150000000, 35000, 300000000, 5, 9);
        AddSimple(MCI, "Omar", "Marmoush", "Marmoush", 26, "EGY", 183, 78, RIGHT, ST, LW_f, 83, 85, 35000000, 7000, 70000000, 4, 7);

        // ══════════════════════════════════════════════════════════════
        // Chelsea FC (TeamId=86)
        // ══════════════════════════════════════════════════════════════
        int CHE = 86;
        AddSimple(CHE, "Robert", "Sánchez", "Sánchez", 27, "ESP", 197, 88, RIGHT, GK, 0, 78, 81, 15000000, 5000, 30000000, 4, 1);
        AddSimple(CHE, "Filip", "Jørgensen", "Jørgensen", 23, "DEN", 192, 85, RIGHT, GK, 0, 74, 84, 15000000, 2500, 30000000, 5, 12);
        AddSimple(CHE, "Gabriel", "Slonina", "Slonina", 21, "USA", 193, 86, RIGHT, GK, 0, 68, 82, 6000000, 1000, 12000000, 5, 36);
        AddSimple(CHE, "Teddy", "Sharman-Lowe", "Sharman-Lowe", 22, "ENG", 189, 84, RIGHT, GK, 0, 62, 76, 1500000, 200, 3000000, 4, 47);
        AddSimple(CHE, "Levi", "Colwill", "Colwill", 22, "ENG", 187, 80, LEFT, CB, LB_f, 82, 88, 35000000, 6000, 70000000, 5, 26);
        AddSimple(CHE, "Trevoh", "Chalobah", "Chalobah", 25, "ENG", 190, 83, RIGHT, CB, CDM_f, 78, 82, 15000000, 4000, 30000000, 4, 14);
        AddSimple(CHE, "Wesley", "Fofana", "Fofana", 24, "FRA", 186, 80, RIGHT, CB, 0, 82, 86, 30000000, 7000, 60000000, 5, 29);
        AddSimple(CHE, "Mamadou", "Sarr", "M. Sarr", 19, "FRA", 190, 82, RIGHT, CB, 0, 64, 82, 4000000, 200, 8000000, 5, 34);
        AddSimple(CHE, "Benoît", "Badiashile", "Badiashile", 24, "FRA", 194, 84, LEFT, CB, 0, 80, 85, 25000000, 5000, 50000000, 5, 4);
        AddSimple(CHE, "Jorrel", "Hato", "Hato", 19, "NED", 183, 76, LEFT, CB, LB_f, 76, 88, 25000000, 2000, 50000000, 5, 35);
        AddSimple(CHE, "Reece", "James", "James", 25, "ENG", 179, 78, RIGHT, RB, CB_f, 85, 87, 45000000, 10000, 90000000, 4, 24);
        AddSimple(CHE, "Malo", "Gusto", "Gusto", 22, "FRA", 178, 74, RIGHT, RB, 0, 80, 86, 30000000, 5000, 60000000, 5, 27);
        AddSimple(CHE, "Josh", "Acheampong", "Acheampong", 19, "ENG", 185, 78, RIGHT, RB, CB_f, 66, 84, 5000000, 200, 10000000, 5, 34);
        AddSimple(CHE, "Moisés", "Caicedo", "Caicedo", 23, "ECU", 178, 76, RIGHT, CDM, CM_f, 86, 90, 60000000, 10000, 120000000, 5, 25);
        AddSimple(CHE, "Roméo", "Lavia", "Lavia", 21, "BEL", 181, 76, RIGHT, CDM, CM_f, 78, 86, 25000000, 4000, 50000000, 5, 45);
        AddSimple(CHE, "Dário", "Essugo", "Essugo", 20, "POR", 182, 78, RIGHT, CDM, CM_f, 68, 84, 8000000, 1000, 16000000, 5, 38);
        AddSimple(CHE, "Enzo", "Fernández", "Enzo", 24, "ARG", 178, 74, RIGHT, CM, CAM_f, 85, 88, 50000000, 10000, 100000000, 5, 8);
        AddSimple(CHE, "Cole", "Palmer", "Palmer", 23, "ENG", 185, 78, LEFT, CAM, RW_f, 87, 91, 80000000, 12000, 160000000, 5, 20);
        AddSimple(CHE, "Jamie", "Gittens", "Gittens", 20, "ENG", 177, 72, RIGHT, LW, RW_f, 78, 86, 25000000, 3000, 50000000, 5, 10);
        AddSimple(CHE, "Mykhaylo", "Mudryk", "Mudryk", 24, "UKR", 175, 70, RIGHT, LW, RW_f, 80, 85, 25000000, 6000, 50000000, 4, 10);
        AddSimple(CHE, "Estêvão", "Willian", "Estêvão", 18, "BRA", 176, 70, LEFT, RW, LW_f, 72, 88, 20000000, 1000, 40000000, 5, 41);
        AddSimple(CHE, "Pedro", "Neto", "Neto", 25, "POR", 172, 68, LEFT, RW, LW_f, 82, 85, 30000000, 6000, 60000000, 4, 7);
        AddSimple(CHE, "João", "Pedro", "João Pedro", 23, "BRA", 182, 78, RIGHT, ST, CF_f, 82, 86, 35000000, 6000, 70000000, 5, 18);
        AddSimple(CHE, "Liam", "Delap", "Delap", 22, "ENG", 186, 82, RIGHT, ST, 0, 74, 84, 15000000, 2500, 30000000, 5, 19);
        AddSimple(CHE, "Marc", "Guiu", "Guiu", 19, "ESP", 187, 80, RIGHT, ST, 0, 70, 84, 10000000, 1500, 20000000, 5, 31);

        // ══════════════════════════════════════════════════════════════
        // Aston Villa FC (TeamId=87)
        // ══════════════════════════════════════════════════════════════
        int AVL = 87;
        AddSimple(AVL, "Emiliano", "Martínez", "Dibu", 32, "ARG", 195, 88, RIGHT, GK, 0, 87, 88, 35000000, 12000, 70000000, 4, 1);
        AddSimple(AVL, "Marco", "Bizot", "Bizot", 34, "NED", 193, 86, LEFT, GK, 0, 76, 77, 5000000, 4000, 10000000, 2, 22);
        AddSimple(AVL, "Ezri", "Konsa", "Konsa", 27, "ENG", 183, 78, RIGHT, CB, 0, 82, 84, 28000000, 6000, 55000000, 4, 4);
        AddSimple(AVL, "Pau", "Torres", "Pau Torres", 28, "ESP", 191, 84, LEFT, CB, 0, 84, 86, 35000000, 8000, 70000000, 4, 14);
        AddSimple(AVL, "Victor", "Lindelöf", "Lindelöf", 30, "SWE", 187, 80, RIGHT, CB, 0, 80, 81, 10000000, 6000, 20000000, 3, 2);
        AddSimple(AVL, "Tyrone", "Mings", "Mings", 31, "ENG", 196, 86, LEFT, CB, 0, 79, 80, 12000000, 6000, 25000000, 3, 5);
        AddSimple(AVL, "Ian", "Maatsen", "Maatsen", 23, "NED", 178, 72, LEFT, LB, LWB_f, 78, 85, 25000000, 4000, 50000000, 5, 17);
        AddSimple(AVL, "Lucas", "Digne", "Digne", 31, "FRA", 178, 75, LEFT, LB, 0, 80, 81, 10000000, 7000, 20000000, 2, 12);
        AddSimple(AVL, "Matty", "Cash", "Cash", 27, "POL", 185, 78, RIGHT, RB, 0, 80, 82, 20000000, 6000, 40000000, 4, 2);
        AddSimple(AVL, "Andrés", "García", "A. García", 22, "ESP", 174, 72, RIGHT, RB, 0, 72, 82, 10000000, 2000, 20000000, 5, 24);
        AddSimple(AVL, "Amadou", "Onana", "Onana", 23, "BEL", 195, 86, RIGHT, CDM, CM_f, 82, 86, 30000000, 6000, 60000000, 5, 24);
        AddSimple(AVL, "Boubacar", "Kamara", "Kamara", 25, "FRA", 184, 78, RIGHT, CDM, CB_f, 82, 85, 30000000, 7000, 60000000, 4, 44);
        AddSimple(AVL, "Lamare", "Bogarde", "Bogarde", 21, "NED", 185, 78, RIGHT, CDM, CB_f, 68, 82, 6000000, 800, 12000000, 5, 26);
        AddSimple(AVL, "Youri", "Tielemans", "Tielemans", 28, "BEL", 176, 74, RIGHT, CM, 0, 82, 84, 30000000, 8000, 60000000, 3, 8);
        AddSimple(AVL, "Douglas", "Luiz", "D. Luiz", 27, "BRA", 178, 74, RIGHT, CM, CDM_f, 85, 87, 45000000, 10000, 90000000, 4, 6);
        AddSimple(AVL, "John", "McGinn", "McGinn", 30, "SCO", 178, 75, LEFT, CM, 0, 82, 83, 22000000, 8000, 45000000, 3, 7);
        AddSimple(AVL, "Ross", "Barkley", "Barkley", 31, "ENG", 189, 82, RIGHT, CM, CAM_f, 77, 78, 6000000, 5000, 12000000, 2, 20);
        AddSimple(AVL, "Morgan", "Rogers", "Rogers", 22, "ENG", 188, 80, RIGHT, CAM, LW_f, 76, 86, 20000000, 3000, 40000000, 5, 27);
        AddSimple(AVL, "Harvey", "Elliott", "Elliott", 22, "ENG", 170, 66, LEFT, RW, CM_f, 78, 86, 25000000, 4000, 50000000, 5, 19);
        AddSimple(AVL, "Jadon", "Sancho", "Sancho", 25, "ENG", 180, 76, RIGHT, LW, RW_f, 82, 85, 30000000, 8000, 60000000, 4, 10);
        AddSimple(AVL, "Emiliano", "Buendía", "Buendía", 28, "ARG", 172, 68, RIGHT, CAM, RW_f, 81, 83, 15000000, 7000, 30000000, 3, 10);
        AddSimple(AVL, "Lewis", "Dobbin", "Dobbin", 22, "ENG", 175, 68, RIGHT, LW, ST_f, 68, 80, 5000000, 500, 10000000, 4, 27);
        AddSimple(AVL, "Leon", "Bailey", "Bailey", 27, "JAM", 178, 72, LEFT, RW, LW_f, 82, 84, 25000000, 8000, 50000000, 4, 31);
        AddSimple(AVL, "Alysson", "Rabelo", "Alysson", 24, "BRA", 183, 78, RIGHT, ST, 0, 72, 80, 8000000, 1500, 16000000, 4, 36);
        AddSimple(AVL, "Ollie", "Watkins", "Watkins", 29, "ENG", 180, 76, RIGHT, ST, 0, 86, 87, 50000000, 12000, 100000000, 4, 11);
        AddSimple(AVL, "Tammy", "Abraham", "Abraham", 27, "ENG", 190, 84, RIGHT, ST, 0, 80, 82, 20000000, 7000, 40000000, 3, 9);

        // ══════════════════════════════════════════════════════════════
        // Newcastle United FC (TeamId=88)
        // ══════════════════════════════════════════════════════════════
        int NEW = 88;
        AddSimple(NEW, "Aaron", "Ramsdale", "Ramsdale", 27, "ENG", 188, 84, RIGHT, GK, 0, 83, 85, 28000000, 8000, 55000000, 4, 1);
        AddSimple(NEW, "Nick", "Pope", "Pope", 33, "ENG", 198, 90, RIGHT, GK, 0, 81, 82, 10000000, 7000, 20000000, 2, 22);
        AddSimple(NEW, "John", "Ruddy", "Ruddy", 38, "ENG", 193, 86, RIGHT, GK, 0, 72, 72, 500000, 300, 1000000, 1, 26);
        AddSimple(NEW, "Malick", "Thiaw", "Thiaw", 23, "GER", 194, 86, RIGHT, CB, 0, 82, 88, 35000000, 5000, 70000000, 5, 4);
        AddSimple(NEW, "Sven", "Botman", "Botman", 25, "NED", 193, 84, LEFT, CB, 0, 84, 88, 40000000, 7000, 80000000, 5, 5);
        AddSimple(NEW, "Fabian", "Schär", "Schär", 33, "SUI", 186, 82, RIGHT, CB, CDM_f, 82, 83, 8000000, 6000, 16000000, 2, 6);
        AddSimple(NEW, "Lewis", "Hall", "Hall", 20, "ENG", 180, 72, LEFT, LB, CM_f, 74, 86, 20000000, 2000, 40000000, 5, 20);
        AddSimple(NEW, "Matt", "Targett", "Targett", 29, "ENG", 183, 76, LEFT, LB, 0, 76, 78, 6000000, 4000, 12000000, 3, 13);
        AddSimple(NEW, "Alex", "Murphy", "A. Murphy", 21, "IRL", 178, 72, RIGHT, LB, 0, 66, 78, 3000000, 500, 6000000, 5, 28);
        AddSimple(NEW, "Tino", "Livramento", "Livramento", 22, "ENG", 181, 76, RIGHT, RB, LB_f, 80, 86, 30000000, 4000, 60000000, 5, 21);
        AddSimple(NEW, "Kieran", "Trippier", "Trippier", 34, "ENG", 173, 72, RIGHT, RB, 0, 84, 84, 10000000, 10000, 20000000, 2, 2);
        AddSimple(NEW, "Sandro", "Tonali", "Tonali", 25, "ITA", 181, 78, RIGHT, CDM, CM_f, 86, 89, 50000000, 10000, 100000000, 5, 8);
        AddSimple(NEW, "Bruno", "Guimarães", "Bruno G.", 27, "BRA", 182, 78, RIGHT, CM, CDM_f, 86, 88, 50000000, 10000, 100000000, 4, 39);
        AddSimple(NEW, "Joelinton", "", "Joelinton", 28, "BRA", 186, 82, RIGHT, CM, ST_f, 82, 84, 25000000, 8000, 50000000, 4, 7);
        AddSimple(NEW, "Lewis", "Miley", "Miley", 19, "ENG", 185, 76, LEFT, CM, 0, 74, 86, 20000000, 1500, 40000000, 5, 67);
        AddSimple(NEW, "Joe", "Willock", "Willock", 25, "ENG", 179, 74, RIGHT, CM, 0, 78, 82, 15000000, 5000, 30000000, 4, 28);
        AddSimple(NEW, "Joe", "White", "J. White", 22, "ENG", 185, 78, RIGHT, CM, 0, 68, 80, 5000000, 500, 10000000, 5, 40);
        AddSimple(NEW, "Anthony", "Gordon", "Gordon", 24, "ENG", 182, 76, LEFT, LW, RW_f, 84, 88, 45000000, 8000, 90000000, 5, 10);
        AddSimple(NEW, "Harvey", "Barnes", "Barnes", 27, "ENG", 174, 72, RIGHT, LW, RW_f, 81, 83, 20000000, 7000, 40000000, 4, 11);
        AddSimple(NEW, "Anthony", "Elanga", "Elanga", 23, "SWE", 178, 72, RIGHT, RW, LW_f, 80, 85, 25000000, 5000, 50000000, 5, 24);
        AddSimple(NEW, "Jacob", "Murphy", "J. Murphy", 30, "ENG", 179, 74, RIGHT, RW, 0, 76, 77, 4000000, 3000, 8000000, 2, 23);
        AddSimple(NEW, "Nick", "Woltemade", "Woltemade", 23, "GER", 198, 88, RIGHT, ST, CF_f, 74, 82, 10000000, 2000, 20000000, 5, 29);
        AddSimple(NEW, "William", "Osula", "Osula", 21, "DEN", 192, 84, RIGHT, ST, 0, 68, 82, 6000000, 800, 12000000, 5, 30);
        AddSimple(NEW, "Yoane", "Wissa", "Wissa", 28, "COD", 176, 72, RIGHT, ST, LW_f, 82, 84, 25000000, 7000, 50000000, 4, 14);

        // ══════════════════════════════════════════════════════════════
        // SSC Napoli (TeamId=89)
        // ══════════════════════════════════════════════════════════════
        int NAP = 89;
        AddSimple(NAP, "Vanja", "Milinković-Savić", "Milinković-Savić", 28, "SRB", 202, 92, RIGHT, GK, 0, 82, 84, 20000000, 7000, 40000000, 4, 1);
        AddSimple(NAP, "Alex", "Meret", "Meret", 28, "ITA", 190, 85, RIGHT, GK, 0, 80, 82, 15000000, 6000, 30000000, 3, 14);
        AddSimple(NAP, "Nikita", "Contini", "Contini", 29, "ITA", 190, 84, RIGHT, GK, 0, 72, 74, 2000000, 500, 4000000, 3, 16);
        AddSimple(NAP, "Mathias", "Ferrante", "Ferrante", 22, "ITA", 188, 82, RIGHT, GK, 0, 62, 76, 1500000, 200, 3000000, 5, 44);
        AddSimple(NAP, "Alessandro", "Buongiorno", "Buongiorno", 26, "ITA", 190, 84, LEFT, CB, 0, 85, 88, 45000000, 8000, 90000000, 5, 4);
        AddSimple(NAP, "Sam", "Beukema", "Beukema", 26, "NED", 188, 82, RIGHT, CB, 0, 80, 84, 25000000, 5000, 50000000, 4, 5);
        AddSimple(NAP, "Amir", "Rrahmani", "Rrahmani", 31, "KVX", 190, 84, RIGHT, CB, 0, 82, 83, 12000000, 6000, 25000000, 3, 13);
        AddSimple(NAP, "Juan", "Jesus", "Juan Jesus", 33, "BRA", 185, 80, LEFT, CB, 0, 78, 79, 3000000, 4000, 6000000, 2, 3);
        AddSimple(NAP, "Miguel", "Gutiérrez", "M. Gutiérrez", 23, "ESP", 180, 74, LEFT, LB, LWB_f, 82, 87, 35000000, 6000, 70000000, 5, 30);
        AddSimple(NAP, "Mathías", "Olivera", "Olivera", 27, "URU", 175, 72, LEFT, LB, 0, 80, 82, 15000000, 5000, 30000000, 4, 17);
        AddSimple(NAP, "Leonardo", "Spinazzola", "Spinazzola", 32, "ITA", 186, 78, RIGHT, LB, LWB_f, 78, 79, 3000000, 4000, 6000000, 2, 37);
        AddSimple(NAP, "Giovanni", "Di Lorenzo", "Di Lorenzo", 31, "ITA", 183, 78, RIGHT, RB, CB_f, 83, 84, 15000000, 8000, 30000000, 3, 22);
        AddSimple(NAP, "Pasquale", "Mazzocchi", "Mazzocchi", 29, "ITA", 183, 78, RIGHT, RB, LB_f, 76, 78, 4000000, 3000, 8000000, 3, 23);
        AddSimple(NAP, "Billy", "Gilmour", "Gilmour", 24, "SCO", 170, 64, RIGHT, CM, CDM_f, 78, 84, 20000000, 4000, 40000000, 5, 6);
        AddSimple(NAP, "Stanislav", "Lobotka", "Lobotka", 30, "SVK", 170, 66, RIGHT, CDM, CM_f, 84, 85, 25000000, 8000, 50000000, 3, 68);
        AddSimple(NAP, "Scott", "McTominay", "McTominay", 28, "SCO", 193, 86, RIGHT, CM, CDM_f, 83, 85, 30000000, 8000, 60000000, 4, 8);
        AddSimple(NAP, "Frank", "Anguissa", "Anguissa", 29, "CMR", 184, 80, RIGHT, CM, CDM_f, 83, 84, 25000000, 8000, 50000000, 4, 99);
        AddSimple(NAP, "Antonio", "Vergara", "Vergara", 22, "ITA", 176, 68, RIGHT, CAM, CM_f, 68, 82, 6000000, 800, 12000000, 5, 71);
        AddSimple(NAP, "Eljif", "Elmas", "Elmas", 25, "MKD", 182, 76, RIGHT, CAM, CM_f, 82, 85, 25000000, 6000, 50000000, 4, 7);
        AddSimple(NAP, "Kevin", "De Bruyne", "De Bruyne", 34, "BEL", 181, 76, RIGHT, CAM, CM_f, 88, 88, 30000000, 18000, 60000000, 2, 17);
        AddSimple(NAP, "David", "Neres", "Neres", 28, "BRA", 175, 70, LEFT, RW, LW_f, 82, 83, 20000000, 7000, 40000000, 3, 11);
        AddSimple(NAP, "Matteo", "Politano", "Politano", 31, "ITA", 171, 68, LEFT, RW, LW_f, 80, 81, 10000000, 6000, 20000000, 3, 21);
        AddSimple(NAP, "Rasmus", "Højlund", "Højlund", 22, "DEN", 191, 84, LEFT, ST, 0, 84, 88, 50000000, 8000, 100000000, 5, 9);
        AddSimple(NAP, "Giovane", "", "Giovane", 21, "BRA", 185, 80, RIGHT, ST, 0, 68, 82, 6000000, 800, 12000000, 5, 73);
        AddSimple(NAP, "Romelu", "Lukaku", "Lukaku", 32, "BEL", 191, 94, LEFT, ST, 0, 84, 84, 25000000, 12000, 50000000, 3, 10);

        // ══════════════════════════════════════════════════════════════
        // Inter Milan (TeamId=90)
        // ══════════════════════════════════════════════════════════════
        int INT = 90;
        AddSimple(INT, "Josep", "Martínez", "J. Martínez", 27, "ESP", 190, 84, RIGHT, GK, 0, 76, 80, 10000000, 4000, 20000000, 4, 12);
        AddSimple(INT, "Yann", "Sommer", "Sommer", 36, "SUI", 183, 82, RIGHT, GK, 0, 85, 85, 8000000, 10000, 16000000, 1, 1);
        AddSimple(INT, "Raffaele", "Di Gennaro", "Di Gennaro", 31, "ITA", 186, 84, RIGHT, GK, 0, 70, 72, 500000, 300, 1000000, 2, 50);
        AddSimple(INT, "Alessandro", "Bastoni", "Bastoni", 26, "ITA", 190, 84, LEFT, CB, LB_f, 87, 90, 60000000, 10000, 120000000, 5, 95);
        AddSimple(INT, "Yann", "Bisseck", "Bisseck", 24, "GER", 196, 88, RIGHT, CB, 0, 82, 86, 30000000, 5000, 60000000, 5, 31);
        AddSimple(INT, "Manuel", "Akanji", "Akanji", 29, "SUI", 186, 82, RIGHT, CB, 0, 84, 85, 25000000, 8000, 50000000, 3, 6);
        AddSimple(INT, "Stefan", "de Vrij", "de Vrij", 33, "NED", 189, 84, RIGHT, CB, 0, 82, 83, 8000000, 7000, 16000000, 2, 6);
        AddSimple(INT, "Francesco", "Acerbi", "Acerbi", 37, "ITA", 192, 86, LEFT, CB, 0, 78, 78, 2000000, 4000, 4000000, 1, 15);
        AddSimple(INT, "Federico", "Dimarco", "Dimarco", 27, "ITA", 171, 68, LEFT, LB, LWB_f, 86, 88, 50000000, 10000, 100000000, 5, 32);
        AddSimple(INT, "Carlos", "Augusto", "C. Augusto", 26, "BRA", 186, 80, LEFT, LB, LWB_f, 80, 84, 20000000, 5000, 40000000, 4, 30);
        AddSimple(INT, "Denzel", "Dumfries", "Dumfries", 29, "NED", 188, 82, RIGHT, RB, RWB_f, 84, 85, 25000000, 8000, 50000000, 3, 2);
        AddSimple(INT, "Matteo", "Darmian", "Darmian", 35, "ITA", 182, 76, RIGHT, RB, CB_f, 79, 79, 3000000, 5000, 6000000, 1, 36);
        AddSimple(INT, "Hakan", "Çalhanoğlu", "Çalhanoğlu", 31, "TUR", 178, 74, LEFT, CM, CDM_f, 86, 87, 35000000, 10000, 70000000, 3, 20);
        AddSimple(INT, "Nicolò", "Barella", "Barella", 28, "ITA", 175, 72, RIGHT, CM, 0, 87, 89, 60000000, 12000, 120000000, 4, 23);
        AddSimple(INT, "Petar", "Sučić", "Sučić", 21, "CRO", 180, 76, RIGHT, CM, CDM_f, 72, 86, 15000000, 2000, 30000000, 5, 17);
        AddSimple(INT, "Davide", "Frattesi", "Frattesi", 25, "ITA", 178, 76, RIGHT, CM, CAM_f, 84, 87, 40000000, 8000, 80000000, 5, 16);
        AddSimple(INT, "Andy", "Diouf", "Diouf", 22, "FRA", 183, 78, RIGHT, CDM, CM_f, 70, 84, 8000000, 1000, 16000000, 5, 45);
        AddSimple(INT, "Piotr", "Zielinski", "Zielinski", 31, "POL", 180, 76, RIGHT, CM, CAM_f, 84, 85, 20000000, 8000, 40000000, 3, 7);
        AddSimple(INT, "Henrikh", "Mkhitaryan", "Mkhitaryan", 36, "ARM", 177, 74, RIGHT, CAM, CM_f, 81, 81, 3000000, 6000, 6000000, 1, 22);
        AddSimple(INT, "Luis", "Henrique", "L. Henrique", 23, "BRA", 181, 76, RIGHT, LW, RW_f, 74, 84, 15000000, 3000, 30000000, 5, 11);
        AddSimple(INT, "Lautaro", "Martínez", "Lautaro", 27, "ARG", 174, 74, RIGHT, ST, 0, 88, 90, 70000000, 15000, 140000000, 5, 10);
        AddSimple(INT, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 84, LEFT, ST, LW_f, 86, 88, 55000000, 12000, 110000000, 5, 9);
        AddSimple(INT, "Ange-Yoan", "Bonny", "Bonny", 21, "FRA", 188, 82, RIGHT, ST, CF_f, 74, 84, 15000000, 2500, 30000000, 5, 14);

        // ══════════════════════════════════════════════════════════════
        // Como 1907 (TeamId=91)
        // ══════════════════════════════════════════════════════════════
        int COM = 91;
        AddSimple(COM, "Jean", "Butez", "Butez", 30, "BEL", 187, 84, RIGHT, GK, 0, 74, 77, 5000000, 3000, 10000000, 3, 1);
        AddSimple(COM, "Noel", "Törnqvist", "Törnqvist", 23, "SWE", 192, 84, RIGHT, GK, 0, 68, 78, 3000000, 800, 6000000, 5, 12);
        AddSimple(COM, "Henrique", "Menke", "Menke", 22, "BRA", 190, 82, RIGHT, GK, 0, 62, 76, 1500000, 200, 3000000, 5, 42);
        AddSimple(COM, "Mauro", "Vigorito", "Vigorito", 34, "ITA", 188, 85, RIGHT, GK, 0, 70, 71, 500000, 300, 1000000, 2, 22);
        AddSimple(COM, "Jacobo", "Ramón", "Ramón", 22, "ESP", 186, 78, RIGHT, CB, 0, 66, 78, 3000000, 500, 6000000, 5, 23);
        AddSimple(COM, "Diego", "Carlos", "Diego Carlos", 32, "BRA", 185, 82, RIGHT, CB, 0, 78, 79, 4000000, 5000, 8000000, 2, 4);
        AddSimple(COM, "Marc", "Kempf", "Kempf", 30, "GER", 186, 82, RIGHT, CB, 0, 76, 78, 4000000, 4000, 8000000, 3, 5);
        AddSimple(COM, "Edoardo", "Goldaniga", "Goldaniga", 31, "ITA", 188, 84, RIGHT, CB, 0, 74, 76, 2000000, 3000, 4000000, 2, 13);
        AddSimple(COM, "Álex", "Valle", "Valle", 21, "ESP", 178, 72, LEFT, LB, 0, 66, 78, 3000000, 500, 6000000, 5, 26);
        AddSimple(COM, "Alberto", "Moreno", "A. Moreno", 32, "ESP", 171, 68, LEFT, LB, 0, 76, 77, 2000000, 4000, 4000000, 2, 18);
        AddSimple(COM, "Ivan", "Smolcic", "Smolcic", 24, "CRO", 186, 80, RIGHT, CB, 0, 70, 78, 4000000, 1000, 8000000, 4, 6);
        AddSimple(COM, "Ignace", "Van der Brempt", "Van der Brempt", 23, "BEL", 182, 76, RIGHT, RB, 0, 68, 78, 3000000, 800, 6000000, 5, 80);
        AddSimple(COM, "Mërgim", "Vojvoda", "Vojvoda", 30, "KVX", 187, 80, RIGHT, RB, LB_f, 74, 76, 2000000, 3000, 4000000, 2, 28);
        AddSimple(COM, "Máximo", "Perrone", "Perrone", 22, "ARG", 176, 70, RIGHT, CDM, CM_f, 72, 82, 10000000, 1500, 20000000, 5, 36);
        AddSimple(COM, "Lucas", "Da Cunha", "Da Cunha", 24, "FRA", 168, 64, RIGHT, CM, CAM_f, 72, 80, 6000000, 1500, 12000000, 4, 33);
        AddSimple(COM, "Maxence", "Caqueret", "Caqueret", 25, "FRA", 174, 70, RIGHT, CM, CDM_f, 78, 82, 15000000, 4000, 30000000, 4, 8);
        AddSimple(COM, "Adrian", "Lahdo", "Lahdo", 22, "SWE", 180, 72, RIGHT, LW, RW_f, 68, 80, 4000000, 800, 8000000, 5, 11);
        AddSimple(COM, "Sergi", "Roberto", "Sergi Roberto", 33, "ESP", 178, 74, RIGHT, CM, RB_f, 78, 79, 3000000, 5000, 6000000, 2, 20);
        AddSimple(COM, "Nico", "Paz", "Paz", 20, "ARG", 186, 78, LEFT, CAM, CM_f, 76, 86, 20000000, 2000, 40000000, 5, 79);
        AddSimple(COM, "Martin", "Baturina", "Baturina", 22, "CRO", 175, 70, RIGHT, CAM, CM_f, 76, 86, 20000000, 2500, 40000000, 5, 10);
        AddSimple(COM, "Jesús", "Rodríguez", "Jesús R.", 19, "ESP", 173, 68, RIGHT, LW, RW_f, 62, 82, 3000000, 200, 6000000, 5, 41);
        AddSimple(COM, "Assane", "Diao", "Diao", 19, "ESP", 185, 78, RIGHT, LW, RW_f, 70, 84, 8000000, 800, 16000000, 5, 38);
        AddSimple(COM, "Jayden", "Addai", "Addai", 19, "NED", 178, 72, LEFT, LW, RW_f, 60, 80, 2000000, 100, 4000000, 5, 44);
        AddSimple(COM, "Nicolas", "Kühn", "Kühn", 25, "GER", 177, 72, LEFT, RW, LW_f, 74, 80, 6000000, 2000, 12000000, 4, 7);
        AddSimple(COM, "Anastasios", "Douvikas", "Douvikas", 25, "GRE", 186, 80, RIGHT, ST, 0, 76, 82, 12000000, 3000, 25000000, 4, 9);
        AddSimple(COM, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, RIGHT, ST, 0, 82, 83, 15000000, 10000, 30000000, 2, 19);

        // ══════════════════════════════════════════════════════════════
        // Juventus FC (TeamId=92)
        // ══════════════════════════════════════════════════════════════
        int JUV = 92;
        AddSimple(JUV, "Michele", "Di Gregorio", "Di Gregorio", 27, "ITA", 187, 82, RIGHT, GK, 0, 82, 85, 20000000, 6000, 40000000, 4, 1);
        AddSimple(JUV, "Mattia", "Perin", "Perin", 32, "ITA", 188, 84, RIGHT, GK, 0, 80, 82, 6000000, 5000, 12000000, 2, 23);
        AddSimple(JUV, "Carlo", "Pinsoglio", "Pinsoglio", 35, "ITA", 194, 86, LEFT, GK, 0, 72, 73, 500000, 300, 1000000, 1, 36);
        AddSimple(JUV, "Bremer", "", "Bremer", 28, "BRA", 188, 84, RIGHT, CB, 0, 86, 88, 45000000, 10000, 90000000, 5, 3);
        AddSimple(JUV, "Pierre", "Kalulu", "Kalulu", 25, "FRA", 182, 78, RIGHT, CB, RB_f, 82, 86, 30000000, 6000, 60000000, 5, 15);
        AddSimple(JUV, "Lloyd", "Kelly", "Kelly", 26, "ENG", 190, 82, LEFT, CB, LB_f, 76, 82, 12000000, 5000, 25000000, 4, 5);
        AddSimple(JUV, "Federico", "Gatti", "Gatti", 26, "ITA", 190, 84, RIGHT, CB, 0, 82, 85, 25000000, 6000, 50000000, 5, 4);
        AddSimple(JUV, "Andrea", "Cambiaso", "Cambiaso", 25, "ITA", 181, 76, LEFT, LB, RB_f, 82, 86, 35000000, 6000, 70000000, 5, 27);
        AddSimple(JUV, "Juan", "Cabal", "Cabal", 24, "COL", 186, 80, LEFT, LB, CB_f, 78, 84, 20000000, 4000, 40000000, 5, 12);
        AddSimple(JUV, "Emil", "Holm", "Holm", 25, "SWE", 187, 80, RIGHT, RB, 0, 78, 82, 15000000, 4000, 30000000, 5, 16);
        AddSimple(JUV, "Manuel", "Locatelli", "Locatelli", 27, "ITA", 186, 80, RIGHT, CDM, CM_f, 82, 85, 30000000, 7000, 60000000, 4, 5);
        AddSimple(JUV, "Teun", "Koopmeiners", "Koopmeiners", 27, "NED", 184, 80, RIGHT, CM, CAM_f, 85, 88, 45000000, 10000, 90000000, 5, 8);
        AddSimple(JUV, "Khéphren", "Thuram", "K. Thuram", 24, "FRA", 192, 84, RIGHT, CM, CDM_f, 82, 86, 30000000, 6000, 60000000, 5, 19);
        AddSimple(JUV, "Weston", "McKennie", "McKennie", 26, "USA", 185, 82, RIGHT, CM, 0, 80, 83, 20000000, 6000, 40000000, 3, 16);
        AddSimple(JUV, "Fabio", "Miretti", "Miretti", 21, "ITA", 179, 74, RIGHT, CM, CAM_f, 74, 84, 15000000, 2500, 30000000, 5, 20);
        AddSimple(JUV, "Filip", "Kostić", "Kostić", 32, "SRB", 184, 78, LEFT, LM, LW_f, 80, 81, 8000000, 6000, 16000000, 2, 11);
        AddSimple(JUV, "Vasilije", "Adžić", "Adžić", 19, "MNE", 185, 76, RIGHT, CM, CAM_f, 64, 84, 4000000, 200, 8000000, 5, 43);
        AddSimple(JUV, "Kenan", "Yıldız", "Yıldız", 22, "TUR", 187, 80, RIGHT, LW, CAM_f, 82, 88, 40000000, 5000, 80000000, 5, 10);
        AddSimple(JUV, "Jérémie", "Boga", "Boga", 28, "CIV", 172, 68, RIGHT, LW, RW_f, 78, 81, 10000000, 5000, 20000000, 3, 7);
        AddSimple(JUV, "Francisco", "Conceição", "Conceição", 22, "POR", 170, 66, LEFT, RW, LW_f, 78, 86, 25000000, 3000, 50000000, 5, 7);
        AddSimple(JUV, "Edon", "Zhegrova", "Zhegrova", 26, "KVX", 181, 74, LEFT, RW, LW_f, 80, 84, 20000000, 5000, 40000000, 4, 17);
        AddSimple(JUV, "Dušan", "Vlahović", "Vlahović", 25, "SRB", 190, 84, LEFT, ST, 0, 86, 88, 50000000, 12000, 100000000, 4, 9);
        AddSimple(JUV, "Jonathan", "David", "David", 26, "CAN", 180, 76, RIGHT, ST, LW_f, 85, 87, 45000000, 8000, 90000000, 4, 14);
        AddSimple(JUV, "Loïs", "Openda", "Openda", 25, "BEL", 178, 72, RIGHT, ST, RW_f, 84, 87, 40000000, 8000, 80000000, 5, 18);
        AddSimple(JUV, "Arkadiusz", "Milik", "Milik", 31, "POL", 186, 82, LEFT, ST, 0, 78, 80, 6000000, 6000, 12000000, 2, 14);

        // ══════════════════════════════════════════════════════════════
        // FC Bayern München (TeamId=93)
        // ══════════════════════════════════════════════════════════════
        int BAY = 93;
        AddSimple(BAY, "Jonas", "Urbig", "Urbig", 21, "GER", 191, 84, RIGHT, GK, 0, 70, 82, 6000000, 800, 12000000, 5, 26);
        AddSimple(BAY, "Manuel", "Neuer", "Neuer", 39, "GER", 193, 88, RIGHT, GK, 0, 88, 88, 8000000, 15000, 16000000, 1, 1);
        AddSimple(BAY, "Sven", "Ulreich", "Ulreich", 36, "GER", 192, 86, RIGHT, GK, 0, 74, 74, 500000, 500, 1000000, 1, 26);
        AddSimple(BAY, "Dayot", "Upamecano", "Upamecano", 26, "FRA", 186, 82, RIGHT, CB, 0, 86, 89, 50000000, 10000, 100000000, 5, 2);
        AddSimple(BAY, "Jonathan", "Tah", "Tah", 29, "GER", 195, 88, RIGHT, CB, 0, 86, 87, 30000000, 10000, 60000000, 3, 4);
        AddSimple(BAY, "Min-jae", "Kim", "Kim", 28, "KOR", 190, 84, RIGHT, CB, 0, 85, 87, 40000000, 8000, 80000000, 4, 3);
        AddSimple(BAY, "Hiroki", "Ito", "Ito", 26, "JPN", 188, 82, LEFT, CB, LB_f, 82, 85, 25000000, 6000, 50000000, 4, 21);
        AddSimple(BAY, "Alphonso", "Davies", "Davies", 24, "CAN", 183, 76, LEFT, LB, LWB_f, 86, 89, 60000000, 10000, 120000000, 5, 19);
        AddSimple(BAY, "Raphaël", "Guerreiro", "Guerreiro", 31, "POR", 170, 68, LEFT, LB, CM_f, 82, 83, 15000000, 8000, 30000000, 2, 22);
        AddSimple(BAY, "Josip", "Stanisic", "Stanisic", 25, "CRO", 186, 80, RIGHT, RB, CB_f, 80, 84, 20000000, 5000, 40000000, 4, 44);
        AddSimple(BAY, "Konrad", "Laimer", "Laimer", 28, "AUT", 180, 76, RIGHT, CM, CDM_f, 82, 84, 20000000, 7000, 40000000, 3, 27);
        AddSimple(BAY, "Aleksandar", "Pavlovic", "Pavlovic", 21, "GER", 188, 80, RIGHT, CDM, CM_f, 78, 88, 30000000, 3000, 60000000, 5, 45);
        AddSimple(BAY, "Joshua", "Kimmich", "Kimmich", 30, "GER", 177, 74, RIGHT, CDM, RB_f, 88, 89, 60000000, 15000, 120000000, 3, 6);
        AddSimple(BAY, "Tom", "Bischof", "Bischof", 20, "GER", 180, 74, RIGHT, CM, CAM_f, 70, 86, 10000000, 1000, 20000000, 5, 28);
        AddSimple(BAY, "Leon", "Goretzka", "Goretzka", 30, "GER", 189, 84, RIGHT, CM, CDM_f, 84, 85, 25000000, 12000, 50000000, 3, 8);
        AddSimple(BAY, "Jamal", "Musiala", "Musiala", 22, "GER", 180, 74, RIGHT, CAM, LW_f, 88, 93, 100000000, 12000, 200000000, 5, 42);
        AddSimple(BAY, "Luis", "Díaz", "Luis Díaz", 28, "COL", 180, 74, RIGHT, LW, RW_f, 86, 88, 50000000, 12000, 100000000, 4, 7);
        AddSimple(BAY, "Michael", "Olise", "Olise", 23, "FRA", 184, 76, LEFT, RW, LW_f, 84, 88, 50000000, 8000, 100000000, 5, 17);
        AddSimple(BAY, "Serge", "Gnabry", "Gnabry", 29, "GER", 175, 72, RIGHT, RW, LW_f, 83, 84, 20000000, 10000, 40000000, 3, 7);
        AddSimple(BAY, "Harry", "Kane", "Kane", 31, "ENG", 188, 86, RIGHT, ST, 0, 91, 92, 100000000, 25000, 200000000, 3, 9);
        AddSimple(BAY, "Nicolas", "Jackson", "Jackson", 24, "SEN", 186, 82, RIGHT, ST, 0, 82, 86, 35000000, 6000, 70000000, 5, 14);

        // ══════════════════════════════════════════════════════════════
        // Bayer 04 Leverkusen (TeamId=94)
        // ══════════════════════════════════════════════════════════════
        int B04 = 94;
        AddSimple(B04, "Mark", "Flekken", "Flekken", 32, "NED", 189, 84, RIGHT, GK, 0, 80, 82, 10000000, 6000, 20000000, 3, 1);
        AddSimple(B04, "Jonas", "Omlin", "Omlin", 31, "SUI", 190, 85, RIGHT, GK, 0, 78, 80, 8000000, 5000, 16000000, 3, 21);
        AddSimple(B04, "Janis", "Blaswich", "Blaswich", 34, "GER", 193, 86, RIGHT, GK, 0, 76, 77, 2000000, 3000, 4000000, 2, 36);
        AddSimple(B04, "Edmond", "Tapsoba", "Tapsoba", 26, "BFA", 194, 84, RIGHT, CB, 0, 84, 87, 40000000, 8000, 80000000, 5, 12);
        AddSimple(B04, "Loïc", "Badé", "Badé", 25, "FRA", 186, 80, RIGHT, CB, 0, 80, 84, 25000000, 5000, 50000000, 5, 4);
        AddSimple(B04, "Alejandro", "Grimaldo", "Grimaldo", 29, "ESP", 171, 68, LEFT, LB, LWB_f, 86, 87, 40000000, 10000, 80000000, 4, 20);
        AddSimple(B04, "Arthur", "", "Arthur", 22, "BRA", 178, 74, RIGHT, RB, RWB_f, 76, 84, 15000000, 3000, 30000000, 5, 13);
        AddSimple(B04, "Lucas", "Vázquez", "Lucas V.", 33, "ESP", 173, 72, RIGHT, RB, RW_f, 80, 81, 5000000, 6000, 10000000, 2, 2);
        AddSimple(B04, "Robert", "Andrich", "Andrich", 30, "GER", 187, 82, RIGHT, CDM, CM_f, 82, 83, 15000000, 7000, 30000000, 3, 8);
        AddSimple(B04, "Exequiel", "Palacios", "Palacios", 26, "ARG", 177, 74, RIGHT, CM, CDM_f, 84, 87, 40000000, 8000, 80000000, 5, 25);
        AddSimple(B04, "Aleix", "García", "Aleix García", 27, "ESP", 180, 74, RIGHT, CM, 0, 82, 84, 25000000, 6000, 50000000, 4, 10);
        AddSimple(B04, "Ibrahim", "Maza", "Maza", 19, "GER", 178, 72, RIGHT, CAM, CM_f, 68, 84, 6000000, 500, 12000000, 5, 34);
        AddSimple(B04, "Malik", "Tillman", "Tillman", 23, "USA", 187, 80, RIGHT, CAM, LW_f, 78, 85, 20000000, 4000, 40000000, 5, 11);
        AddSimple(B04, "Jonas", "Hofmann", "Hofmann", 32, "GER", 176, 72, LEFT, RW, CAM_f, 82, 83, 10000000, 7000, 20000000, 2, 7);
        AddSimple(B04, "Eliesse", "Ben Seghir", "Ben Seghir", 20, "MAR", 178, 72, RIGHT, CAM, LW_f, 76, 86, 20000000, 2000, 40000000, 5, 18);
        AddSimple(B04, "Martin", "Terrier", "Terrier", 28, "FRA", 184, 78, RIGHT, LW, ST_f, 82, 84, 20000000, 7000, 40000000, 4, 9);
        AddSimple(B04, "Nathan", "Tella", "Tella", 25, "NGA", 173, 68, RIGHT, RW, LW_f, 80, 84, 20000000, 5000, 40000000, 5, 19);
        AddSimple(B04, "Montrell", "Culbreath", "Culbreath", 21, "USA", 183, 78, RIGHT, ST, LW_f, 62, 78, 2000000, 200, 4000000, 5, 33);
        AddSimple(B04, "Patrik", "Schick", "Schick", 29, "CZE", 186, 82, LEFT, ST, 0, 84, 85, 25000000, 8000, 50000000, 3, 14);

        // ══════════════════════════════════════════════════════════════
        // Eintracht Frankfurt (TeamId=95)
        // ══════════════════════════════════════════════════════════════
        int FRA = 95;
        AddSimple(FRA, "Kauã", "Santos", "K. Santos", 22, "BRA", 190, 84, RIGHT, GK, 0, 74, 84, 12000000, 2000, 25000000, 5, 40);
        AddSimple(FRA, "Michael", "Zetterer", "Zetterer", 29, "GER", 197, 88, RIGHT, GK, 0, 76, 78, 5000000, 4000, 10000000, 3, 1);
        AddSimple(FRA, "Jens", "Grahl", "Grahl", 36, "GER", 195, 86, RIGHT, GK, 0, 72, 73, 500000, 300, 1000000, 1, 33);
        AddSimple(FRA, "Arthur", "Theate", "Theate", 25, "BEL", 185, 80, LEFT, CB, LB_f, 82, 86, 30000000, 6000, 60000000, 5, 3);
        AddSimple(FRA, "Robin", "Koch", "Koch", 30, "GER", 190, 84, RIGHT, CB, CDM_f, 82, 83, 15000000, 7000, 30000000, 3, 4);
        AddSimple(FRA, "Aurèle", "Amenda", "Amenda", 21, "SUI", 194, 86, RIGHT, CB, 0, 74, 84, 15000000, 2000, 30000000, 5, 5);
        AddSimple(FRA, "Nathaniel", "Brown", "N. Brown", 21, "GER", 185, 78, LEFT, CB, LB_f, 68, 82, 6000000, 800, 12000000, 5, 44);
        AddSimple(FRA, "Rasmus", "Kristensen", "Kristensen", 27, "DEN", 187, 80, RIGHT, RB, CB_f, 78, 81, 10000000, 5000, 20000000, 3, 13);
        AddSimple(FRA, "Ellyes", "Skhiri", "Skhiri", 30, "TUN", 185, 80, RIGHT, CDM, CM_f, 80, 82, 12000000, 6000, 25000000, 3, 15);
        AddSimple(FRA, "Hugo", "Larsson", "Larsson", 21, "SWE", 187, 78, RIGHT, CM, CDM_f, 76, 86, 20000000, 3000, 40000000, 5, 16);
        AddSimple(FRA, "Oscar", "Højlund", "O. Højlund", 19, "DEN", 185, 76, RIGHT, CM, 0, 64, 82, 4000000, 200, 8000000, 5, 39);
        AddSimple(FRA, "Mahmoud", "Dahoud", "Dahoud", 29, "GER", 178, 72, RIGHT, CM, CDM_f, 78, 80, 6000000, 5000, 12000000, 3, 10);
        AddSimple(FRA, "Can", "Uzun", "Uzun", 19, "GER", 185, 78, RIGHT, CAM, ST_f, 72, 86, 15000000, 1500, 30000000, 5, 42);
        AddSimple(FRA, "Farès", "Chaïbi", "Chaïbi", 22, "ALG", 183, 76, RIGHT, CAM, LW_f, 80, 86, 25000000, 4000, 50000000, 5, 8);
        AddSimple(FRA, "Mario", "Götze", "Götze", 33, "GER", 176, 72, RIGHT, CAM, CM_f, 82, 83, 8000000, 8000, 16000000, 2, 27);
        AddSimple(FRA, "Jean-Mattéo", "Bahoya", "Bahoya", 20, "FRA", 178, 72, RIGHT, LW, RW_f, 68, 82, 5000000, 500, 10000000, 5, 17);
        AddSimple(FRA, "Ritsu", "Doan", "Doan", 27, "JPN", 172, 68, LEFT, RW, LW_f, 82, 84, 20000000, 6000, 40000000, 4, 11);
        AddSimple(FRA, "Ansgar", "Knauff", "Knauff", 23, "GER", 180, 76, RIGHT, RW, 0, 78, 83, 15000000, 4000, 30000000, 5, 36);
        AddSimple(FRA, "Jonathan", "Burkardt", "Burkardt", 25, "GER", 186, 80, RIGHT, ST, LW_f, 82, 85, 25000000, 6000, 50000000, 5, 7);
        AddSimple(FRA, "Arnaud", "Kalimuendo", "Kalimuendo", 23, "FRA", 175, 72, RIGHT, ST, 0, 78, 84, 20000000, 4000, 40000000, 5, 23);
        AddSimple(FRA, "Michy", "Batshuayi", "Batshuayi", 31, "BEL", 185, 82, RIGHT, ST, 0, 78, 80, 6000000, 6000, 12000000, 2, 30);

        // ══════════════════════════════════════════════════════════════
        // Borussia Dortmund (TeamId=96)
        // ══════════════════════════════════════════════════════════════
        int BVB = 96;
        AddSimple(BVB, "Gregor", "Kobel", "Kobel", 27, "SUI", 194, 86, RIGHT, GK, 0, 86, 88, 35000000, 8000, 70000000, 4, 1);
        AddSimple(BVB, "Alexander", "Meyer", "Meyer", 34, "GER", 195, 86, RIGHT, GK, 0, 74, 75, 1000000, 1000, 2000000, 2, 33);
        AddSimple(BVB, "Nico", "Schlotterbeck", "Schlotterbeck", 25, "GER", 191, 84, LEFT, CB, 0, 85, 88, 45000000, 8000, 90000000, 5, 4);
        AddSimple(BVB, "Waldemar", "Anton", "Anton", 28, "GER", 189, 82, RIGHT, CB, CDM_f, 82, 84, 20000000, 6000, 40000000, 4, 2);
        AddSimple(BVB, "Ramy", "Bensebaini", "Bensebaini", 30, "ALG", 187, 82, LEFT, LB, CB_f, 78, 81, 8000000, 5000, 16000000, 3, 5);
        AddSimple(BVB, "Niklas", "Süle", "Süle", 29, "GER", 195, 90, RIGHT, CB, RB_f, 82, 84, 20000000, 10000, 40000000, 3, 25);
        AddSimple(BVB, "Emre", "Can", "Can", 31, "GER", 186, 82, RIGHT, CB, CDM_f, 82, 83, 10000000, 8000, 20000000, 2, 23);
        AddSimple(BVB, "Daniel", "Svensson", "Svensson", 23, "SWE", 183, 78, LEFT, LB, CM_f, 74, 84, 12000000, 2000, 25000000, 5, 24);
        AddSimple(BVB, "Julian", "Ryerson", "Ryerson", 27, "NOR", 183, 78, RIGHT, RB, LB_f, 80, 83, 15000000, 5000, 30000000, 4, 26);
        AddSimple(BVB, "Salih", "Özcan", "Özcan", 27, "TUR", 182, 78, RIGHT, CDM, CM_f, 78, 81, 10000000, 5000, 20000000, 3, 6);
        AddSimple(BVB, "Felix", "Nmecha", "Nmecha", 24, "GER", 190, 82, RIGHT, CM, CAM_f, 78, 84, 20000000, 5000, 40000000, 5, 8);
        AddSimple(BVB, "Jobe", "Bellingham", "J. Bellingham", 19, "ENG", 190, 80, RIGHT, CM, CAM_f, 74, 88, 25000000, 2000, 50000000, 5, 7);
        AddSimple(BVB, "Carney", "Chukwuemeka", "Chukwuemeka", 21, "ENG", 187, 80, RIGHT, CM, CAM_f, 72, 84, 12000000, 2000, 25000000, 5, 17);
        AddSimple(BVB, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 76, RIGHT, CM, CAM_f, 82, 83, 12000000, 8000, 25000000, 2, 20);
        AddSimple(BVB, "Yan", "Couto", "Couto", 23, "BRA", 168, 66, RIGHT, RB, RW_f, 78, 84, 20000000, 4000, 40000000, 5, 2);
        AddSimple(BVB, "Julian", "Brandt", "Brandt", 29, "GER", 183, 76, RIGHT, CAM, LW_f, 84, 85, 25000000, 10000, 50000000, 3, 10);
        AddSimple(BVB, "Karim", "Adeyemi", "Adeyemi", 23, "GER", 180, 74, RIGHT, LW, ST_f, 82, 86, 35000000, 6000, 70000000, 5, 27);
        AddSimple(BVB, "Maximilian", "Beier", "Beier", 22, "GER", 185, 78, RIGHT, ST, LW_f, 78, 86, 25000000, 4000, 50000000, 5, 14);
        AddSimple(BVB, "Serhou", "Guirassy", "Guirassy", 29, "GUI", 187, 84, RIGHT, ST, 0, 86, 87, 35000000, 10000, 70000000, 3, 9);
        AddSimple(BVB, "Fábio", "Silva", "Fábio Silva", 22, "POR", 185, 78, RIGHT, ST, 0, 74, 84, 15000000, 3000, 30000000, 5, 29);

        // ══════════════════════════════════════════════════════════════
        // Paris Saint-Germain (TeamId=97)
        // ══════════════════════════════════════════════════════════════
        int PSG = 97;
        AddSimple(PSG, "Matvey", "Safonov", "Safonov", 26, "RUS", 192, 86, RIGHT, GK, 0, 80, 83, 15000000, 5000, 30000000, 4, 1);
        AddSimple(PSG, "Lucas", "Chevalier", "Chevalier", 23, "FRA", 189, 84, RIGHT, GK, 0, 82, 88, 35000000, 5000, 70000000, 5, 30);
        AddSimple(PSG, "Willian", "Pacho", "Pacho", 23, "ECU", 188, 82, LEFT, CB, 0, 82, 87, 35000000, 6000, 70000000, 5, 51);
        AddSimple(PSG, "Ilya", "Zabarnyi", "Zabarnyi", 22, "UKR", 189, 82, RIGHT, CB, 0, 80, 86, 30000000, 5000, 60000000, 5, 3);
        AddSimple(PSG, "Marquinhos", "", "Marquinhos", 31, "BRA", 183, 78, RIGHT, CB, CDM_f, 87, 88, 35000000, 12000, 70000000, 3, 5);
        AddSimple(PSG, "Lucas", "Beraldo", "Beraldo", 21, "BRA", 187, 80, LEFT, CB, LB_f, 76, 86, 20000000, 3000, 40000000, 5, 35);
        AddSimple(PSG, "Nuno", "Mendes", "N. Mendes", 23, "POR", 176, 72, LEFT, LB, LWB_f, 84, 88, 45000000, 8000, 90000000, 5, 25);
        AddSimple(PSG, "Lucas", "Hernández", "Lucas H.", 29, "FRA", 184, 80, LEFT, CB, LB_f, 84, 85, 25000000, 10000, 50000000, 3, 21);
        AddSimple(PSG, "Achraf", "Hakimi", "Hakimi", 26, "MAR", 181, 76, RIGHT, RB, RWB_f, 86, 88, 50000000, 12000, 100000000, 4, 2);
        AddSimple(PSG, "Vitinha", "", "Vitinha", 25, "POR", 172, 68, RIGHT, CM, CDM_f, 85, 87, 45000000, 10000, 90000000, 5, 17);
        AddSimple(PSG, "João", "Neves", "João Neves", 20, "POR", 174, 68, RIGHT, CM, CDM_f, 82, 90, 60000000, 5000, 120000000, 5, 87);
        AddSimple(PSG, "Warren", "Zaïre-Emery", "Zaïre-Emery", 19, "FRA", 178, 72, RIGHT, CM, CDM_f, 82, 90, 55000000, 4000, 110000000, 5, 33);
        AddSimple(PSG, "Fabián", "Ruiz", "Fabián", 29, "ESP", 189, 82, LEFT, CM, CAM_f, 84, 85, 25000000, 10000, 50000000, 3, 8);
        AddSimple(PSG, "Kang-in", "Lee", "Lee", 24, "KOR", 173, 68, LEFT, CAM, RW_f, 82, 86, 30000000, 6000, 60000000, 5, 19);
        AddSimple(PSG, "Khvicha", "Kvaratskhelia", "Kvaratskhelia", 24, "GEO", 183, 76, RIGHT, LW, RW_f, 87, 91, 80000000, 10000, 160000000, 5, 7);
        AddSimple(PSG, "Bradley", "Barcola", "Barcola", 22, "FRA", 182, 74, RIGHT, LW, RW_f, 84, 88, 50000000, 6000, 100000000, 5, 29);
        AddSimple(PSG, "Désiré", "Doué", "Doué", 20, "FRA", 181, 74, RIGHT, RW, LW_f, 78, 88, 30000000, 3000, 60000000, 5, 14);
        AddSimple(PSG, "Ousmane", "Dembélé", "Dembélé", 28, "FRA", 178, 72, BOTH, RW, LW_f, 86, 87, 45000000, 15000, 90000000, 3, 10);
        AddSimple(PSG, "Gonçalo", "Ramos", "G. Ramos", 24, "POR", 185, 80, RIGHT, ST, 0, 84, 87, 50000000, 8000, 100000000, 5, 9);

        // ══════════════════════════════════════════════════════════════
        // Olympique de Marseille (TeamId=98)
        // ══════════════════════════════════════════════════════════════
        int MAR = 98;
        AddSimple(MAR, "Gerónimo", "Rulli", "Rulli", 33, "ARG", 189, 84, RIGHT, GK, 0, 80, 82, 8000000, 5000, 16000000, 3, 1);
        AddSimple(MAR, "Rubén", "Blanco", "Rubén Blanco", 29, "ESP", 188, 82, RIGHT, GK, 0, 75, 78, 4000000, 3000, 8000000, 3, 36);
        AddSimple(MAR, "Leonardo", "Balerdi", "Balerdi", 26, "ARG", 188, 82, RIGHT, CB, 0, 82, 84, 20000000, 6000, 40000000, 4, 5);
        AddSimple(MAR, "Chancel", "Mbemba", "Mbemba", 30, "COD", 186, 82, RIGHT, CB, 0, 80, 82, 10000000, 6000, 20000000, 3, 99);
        AddSimple(MAR, "Derek", "Cornelius", "Cornelius", 27, "CAN", 188, 84, LEFT, CB, 0, 78, 81, 10000000, 4000, 20000000, 4, 13);
        AddSimple(MAR, "Bamo", "Meïté", "Meïté", 23, "CIV", 188, 82, RIGHT, CB, 0, 74, 82, 10000000, 2000, 20000000, 5, 19);
        AddSimple(MAR, "Roggerio", "", "Roggerio", 26, "BRA", 183, 76, LEFT, LB, CB_f, 76, 80, 6000000, 3000, 12000000, 4, 3);
        AddSimple(MAR, "Quentin", "Merlin", "Merlin", 23, "FRA", 174, 70, RIGHT, LB, 0, 78, 84, 15000000, 3000, 30000000, 5, 23);
        AddSimple(MAR, "Michael", "Murillo", "Murillo", 28, "PAN", 183, 78, RIGHT, RB, 0, 78, 81, 10000000, 4000, 20000000, 4, 62);
        AddSimple(MAR, "Valentin", "Rongier", "Rongier", 30, "FRA", 172, 68, RIGHT, CM, CDM_f, 80, 82, 12000000, 6000, 25000000, 3, 21);
        AddSimple(MAR, "Geoffrey", "Kondogbia", "Kondogbia", 32, "CTA", 188, 84, LEFT, CDM, CM_f, 82, 83, 10000000, 8000, 20000000, 2, 19);
        AddSimple(MAR, "Pierre-Emile", "Højbjerg", "Højbjerg", 29, "DEN", 187, 82, RIGHT, CDM, CM_f, 82, 84, 20000000, 8000, 40000000, 4, 22);
        AddSimple(MAR, "Azzedine", "Ounahi", "Ounahi", 25, "MAR", 176, 72, RIGHT, CM, 0, 78, 82, 15000000, 4000, 30000000, 5, 11);
        AddSimple(MAR, "Amine", "Harit", "Harit", 28, "MAR", 180, 74, RIGHT, CAM, LW_f, 80, 82, 12000000, 6000, 25000000, 3, 8);
        AddSimple(MAR, "Bilal", "Nadir", "Nadir", 22, "FRA", 178, 72, RIGHT, CM, 0, 64, 82, 4000000, 500, 8000000, 5, 26);
        AddSimple(MAR, "Mason", "Greenwood", "Greenwood", 23, "ENG", 181, 74, LEFT, RW, ST_f, 82, 86, 35000000, 6000, 70000000, 5, 10);
        AddSimple(MAR, "Luis", "Henrique", "L. Henrique", 23, "BRA", 172, 68, RIGHT, LW, RW_f, 78, 82, 15000000, 3000, 30000000, 5, 44);
        AddSimple(MAR, "Jonathan", "Rowe", "Rowe", 22, "ENG", 175, 72, RIGHT, LW, 0, 76, 82, 12000000, 2000, 25000000, 5, 17);
        AddSimple(MAR, "Elye", "Wahi", "Wahi", 22, "FRA", 184, 80, RIGHT, ST, LW_f, 80, 86, 30000000, 4000, 60000000, 5, 9);
        AddSimple(MAR, "Faris", "Moumbagna", "Moumbagna", 25, "CMR", 184, 80, RIGHT, ST, 0, 74, 78, 5000000, 3000, 10000000, 4, 14);
        AddSimple(MAR, "Neal", "Maupay", "Maupay", 28, "FRA", 170, 68, RIGHT, ST, 0, 76, 78, 6000000, 5000, 12000000, 3, 20);

        // ══════════════════════════════════════════════════════════════
        // AS Monaco (TeamId=99)
        // ══════════════════════════════════════════════════════════════
        int MON = 99;
        AddSimple(MON, "Radosław", "Majecki", "Majecki", 25, "POL", 190, 86, RIGHT, GK, 0, 78, 82, 10000000, 3000, 20000000, 5, 1);
        AddSimple(MON, "Philipp", "Köhn", "Köhn", 26, "GER", 193, 86, RIGHT, GK, 0, 76, 80, 6000000, 2000, 12000000, 4, 16);
        AddSimple(MON, "Thilo", "Kehrer", "Kehrer", 28, "GER", 186, 82, RIGHT, CB, RB_f, 80, 82, 12000000, 6000, 25000000, 3, 5);
        AddSimple(MON, "Mohammed", "Salisu", "Salisu", 26, "GHA", 191, 86, LEFT, CB, 0, 78, 82, 15000000, 5000, 30000000, 5, 14);
        AddSimple(MON, "Vanderson", "", "Vanderson", 24, "BRA", 180, 76, RIGHT, RB, 0, 80, 84, 25000000, 5000, 50000000, 5, 2);
        AddSimple(MON, "Caio", "Henrique", "C. Henrique", 27, "BRA", 182, 76, LEFT, LB, 0, 82, 84, 25000000, 6000, 50000000, 5, 12);
        AddSimple(MON, "Jordan", "Teze", "Teze", 25, "NED", 183, 78, RIGHT, CB, RB_f, 78, 82, 12000000, 4000, 25000000, 5, 22);
        AddSimple(MON, "Chrislain", "Matsima", "Matsima", 23, "FRA", 188, 82, RIGHT, CB, 0, 74, 82, 8000000, 2000, 16000000, 5, 4);
        AddSimple(MON, "Denis", "Zakaria", "Zakaria", 28, "SUI", 191, 84, RIGHT, CDM, CM_f, 82, 84, 20000000, 7000, 40000000, 4, 6);
        AddSimple(MON, "Lamine", "Camara", "Camara", 21, "SEN", 178, 72, RIGHT, CM, CDM_f, 74, 84, 12000000, 2000, 25000000, 5, 15);
        AddSimple(MON, "Mamadou", "Coulibaly", "Coulibaly", 21, "SEN", 183, 76, RIGHT, CM, 0, 66, 82, 5000000, 500, 10000000, 5, 30);
        AddSimple(MON, "Soungoutou", "Magassa", "Magassa", 21, "FRA", 185, 78, RIGHT, CDM, CM_f, 68, 82, 6000000, 500, 12000000, 5, 38);
        AddSimple(MON, "Takumi", "Minamino", "Minamino", 30, "JPN", 174, 70, RIGHT, CAM, LW_f, 82, 83, 12000000, 8000, 25000000, 3, 18);
        AddSimple(MON, "Aleksandr", "Golovin", "Golovin", 29, "RUS", 180, 74, RIGHT, CAM, LW_f, 84, 85, 25000000, 8000, 50000000, 3, 10);
        AddSimple(MON, "Maghnes", "Akliouche", "Akliouche", 23, "FRA", 183, 78, LEFT, CAM, RW_f, 80, 86, 30000000, 4000, 60000000, 5, 11);
        AddSimple(MON, "Krépin", "Diatta", "Diatta", 26, "SEN", 175, 70, RIGHT, RW, 0, 78, 81, 10000000, 5000, 20000000, 3, 27);
        AddSimple(MON, "Breel", "Embolo", "Embolo", 28, "SUI", 187, 84, RIGHT, ST, RW_f, 82, 84, 20000000, 7000, 40000000, 4, 36);
        AddSimple(MON, "Folarin", "Balogun", "Balogun", 24, "USA", 178, 76, RIGHT, ST, 0, 80, 84, 25000000, 5000, 50000000, 5, 9);
        AddSimple(MON, "George", "Ilenikhena", "Ilenikhena", 20, "NGA", 185, 80, RIGHT, ST, 0, 72, 86, 15000000, 1500, 30000000, 5, 21);
        AddSimple(MON, "Mika", "Biereth", "Biereth", 22, "DEN", 184, 78, RIGHT, ST, 0, 68, 82, 6000000, 500, 12000000, 5, 17);
        AddSimple(MON, "Arthur", "Conge", "Conge", 20, "FRA", 183, 76, RIGHT, ST, 0, 62, 78, 2000000, 200, 4000000, 5, 42);
        AddSimple(MON, "Manuel", "da Silva", "M. Silva", 22, "POR", 178, 72, RIGHT, RW, 0, 64, 78, 2000000, 200, 4000000, 5, 47);

        // ══════════════════════════════════════════════════════════════
        // PSV Eindhoven (TeamId=100)
        // ══════════════════════════════════════════════════════════════
        int PSV = 100;
        AddSimple(PSV, "Walter", "Benítez", "Benítez", 32, "ARG", 191, 86, RIGHT, GK, 0, 82, 83, 12000000, 7000, 25000000, 3, 1);
        AddSimple(PSV, "Joël", "Drommel", "Drommel", 28, "NED", 193, 88, RIGHT, GK, 0, 74, 76, 2000000, 2000, 4000000, 2, 24);
        AddSimple(PSV, "Olivier", "Boscagli", "Boscagli", 27, "FRA", 185, 80, LEFT, CB, LB_f, 82, 84, 20000000, 6000, 40000000, 5, 4);
        AddSimple(PSV, "Ryan", "Flamingo", "Flamingo", 22, "NED", 186, 80, RIGHT, CB, CDM_f, 78, 85, 20000000, 3000, 40000000, 5, 6);
        AddSimple(PSV, "Armando", "Obispo", "Obispo", 26, "NED", 186, 80, LEFT, CB, 0, 76, 80, 8000000, 3000, 16000000, 4, 5);
        AddSimple(PSV, "Adamo", "Nagalo", "Nagalo", 22, "BFA", 186, 80, RIGHT, CB, 0, 72, 84, 10000000, 2000, 20000000, 5, 18);
        AddSimple(PSV, "Sergiño", "Dest", "Dest", 24, "USA", 171, 68, RIGHT, LB, RB_f, 78, 82, 15000000, 5000, 30000000, 3, 8);
        AddSimple(PSV, "Richard", "Ledezma", "Ledezma", 24, "USA", 180, 76, RIGHT, RB, CM_f, 74, 80, 6000000, 2000, 12000000, 4, 16);
        AddSimple(PSV, "Mauro", "Júnior", "Mauro Júnior", 26, "BRA", 172, 70, RIGHT, RB, CM_f, 76, 80, 6000000, 3000, 12000000, 4, 17);
        AddSimple(PSV, "Jerdy", "Schouten", "Schouten", 28, "NED", 189, 84, RIGHT, CDM, CM_f, 84, 86, 30000000, 8000, 60000000, 4, 22);
        AddSimple(PSV, "Joey", "Veerman", "Veerman", 26, "NED", 185, 80, RIGHT, CM, 0, 84, 87, 35000000, 8000, 70000000, 5, 23);
        AddSimple(PSV, "Guus", "Til", "Til", 27, "NED", 186, 82, RIGHT, CAM, ST_f, 78, 81, 10000000, 5000, 20000000, 4, 11);
        AddSimple(PSV, "Ismael", "Saibari", "Saibari", 24, "MAR", 183, 78, RIGHT, CM, CAM_f, 78, 83, 15000000, 3000, 30000000, 5, 21);
        AddSimple(PSV, "Isaac", "Babadi", "Babadi", 20, "NED", 175, 72, RIGHT, CM, 0, 66, 84, 8000000, 500, 16000000, 5, 34);
        AddSimple(PSV, "Johan", "Bakayoko", "Bakayoko", 22, "BEL", 179, 72, LEFT, RW, LW_f, 80, 86, 30000000, 4000, 60000000, 5, 10);
        AddSimple(PSV, "Noa", "Lang", "Lang", 26, "NED", 175, 72, RIGHT, LW, RW_f, 80, 83, 15000000, 6000, 30000000, 4, 7);
        AddSimple(PSV, "Couhaib", "Driouech", "Driouech", 22, "MAR", 172, 70, RIGHT, LW, 0, 74, 80, 6000000, 2000, 12000000, 5, 20);
        AddSimple(PSV, "Ivan", "Perišić", "Perišić", 36, "CRO", 186, 82, BOTH, LW, RWB_f, 78, 78, 2000000, 8000, 4000000, 1, 15);
        AddSimple(PSV, "Luuk", "de Jong", "Luuk de Jong", 34, "NED", 188, 84, RIGHT, ST, 0, 84, 84, 8000000, 10000, 16000000, 2, 9);
        AddSimple(PSV, "Ricardo", "Pepi", "Pepi", 22, "USA", 185, 80, RIGHT, ST, 0, 78, 84, 20000000, 4000, 40000000, 5, 14);

        // ══════════════════════════════════════════════════════════════
        // Ajax (TeamId=101)
        // ══════════════════════════════════════════════════════════════
        int AJX = 101;
        AddSimple(AJX, "Remko", "Pasveer", "Pasveer", 41, "NED", 187, 84, RIGHT, GK, 0, 74, 74, 200000, 1000, 500000, 1, 22);
        AddSimple(AJX, "Jay", "Gorter", "Gorter", 25, "NED", 188, 82, RIGHT, GK, 0, 74, 78, 4000000, 2000, 8000000, 4, 12);
        AddSimple(AJX, "Jorrel", "Hato", "Hato", 19, "NED", 183, 78, LEFT, CB, LB_f, 78, 88, 30000000, 2000, 60000000, 5, 4);
        AddSimple(AJX, "Josip", "Šutalo", "Šutalo", 25, "CRO", 189, 84, RIGHT, CB, 0, 78, 84, 20000000, 5000, 40000000, 5, 15);
        AddSimple(AJX, "Daniele", "Rugani", "Rugani", 30, "ITA", 190, 84, RIGHT, CB, 0, 78, 81, 8000000, 6000, 16000000, 3, 24);
        AddSimple(AJX, "Devyne", "Rensch", "Rensch", 22, "NED", 178, 74, RIGHT, RB, CB_f, 76, 84, 15000000, 3000, 30000000, 5, 2);
        AddSimple(AJX, "Anton", "Gaaei", "Gaaei", 22, "DEN", 181, 76, RIGHT, RB, 0, 72, 80, 6000000, 2000, 12000000, 5, 25);
        AddSimple(AJX, "Youri", "Baas", "Baas", 22, "NED", 178, 74, LEFT, LB, CB_f, 72, 82, 8000000, 1500, 16000000, 5, 28);
        AddSimple(AJX, "Owen", "Wijndal", "Wijndal", 25, "NED", 175, 72, LEFT, LB, 0, 76, 81, 8000000, 3000, 16000000, 4, 5);
        AddSimple(AJX, "Jordan", "Henderson", "Henderson", 34, "ENG", 182, 78, RIGHT, CDM, CM_f, 80, 81, 5000000, 10000, 10000000, 2, 6);
        AddSimple(AJX, "Kenneth", "Taylor", "Taylor", 23, "NED", 182, 76, RIGHT, CM, CDM_f, 78, 84, 20000000, 4000, 40000000, 5, 8);
        AddSimple(AJX, "Kian", "Fitz-Jim", "Fitz-Jim", 21, "NED", 180, 74, RIGHT, CM, 0, 70, 84, 10000000, 1500, 20000000, 5, 23);
        AddSimple(AJX, "Davy", "Klaassen", "Klaassen", 32, "NED", 179, 76, RIGHT, CAM, CM_f, 78, 80, 6000000, 7000, 12000000, 2, 18);
        AddSimple(AJX, "Branco", "van den Boomen", "v.d. Boomen", 29, "NED", 192, 84, RIGHT, CM, 0, 76, 79, 4000000, 4000, 8000000, 3, 21);
        AddSimple(AJX, "Benjamin", "Tahirović", "Tahirović", 22, "BIH", 188, 80, RIGHT, CM, CDM_f, 74, 82, 12000000, 2000, 25000000, 5, 30);
        AddSimple(AJX, "Steven", "Berghuis", "Berghuis", 33, "NED", 182, 78, LEFT, RW, CAM_f, 82, 83, 8000000, 8000, 16000000, 2, 10);
        AddSimple(AJX, "Mika", "Godts", "Godts", 20, "BEL", 178, 72, RIGHT, LW, 0, 72, 84, 12000000, 1500, 25000000, 5, 29);
        AddSimple(AJX, "Chuba", "Akpom", "Akpom", 29, "NGA", 183, 78, RIGHT, ST, CAM_f, 76, 80, 6000000, 5000, 12000000, 3, 17);
        AddSimple(AJX, "Brian", "Brobbey", "Brobbey", 23, "NED", 180, 80, RIGHT, ST, 0, 80, 84, 25000000, 5000, 50000000, 5, 9);
        AddSimple(AJX, "Wout", "Weghorst", "Weghorst", 32, "NED", 197, 90, RIGHT, ST, 0, 78, 80, 6000000, 7000, 12000000, 2, 7);
        AddSimple(AJX, "Oliver", "Edvardsen", "Edvardsen", 25, "NOR", 183, 78, RIGHT, RW, ST_f, 72, 78, 3000000, 2000, 6000000, 4, 27);
        AddSimple(AJX, "Christian", "Rasmussen", "Rasmussen", 22, "DEN", 182, 76, RIGHT, RW, 0, 68, 78, 4000000, 500, 8000000, 5, 37);
        AddSimple(AJX, "Amourricho", "van Axel Dongen", "A. Axel Dongen", 21, "NED", 173, 68, RIGHT, LW, 0, 64, 78, 3000000, 500, 6000000, 5, 41);

        // ══════════════════════════════════════════════════════════════
        // Benfica (TeamId=102)
        // ══════════════════════════════════════════════════════════════
        int BEN = 102;
        AddSimple(BEN, "Anatoliy", "Trubin", "Trubin", 23, "UKR", 199, 92, RIGHT, GK, 0, 82, 88, 35000000, 5000, 70000000, 5, 1);
        AddSimple(BEN, "Samuel", "Soares", "S. Soares", 22, "POR", 186, 80, RIGHT, GK, 0, 72, 82, 8000000, 1000, 16000000, 5, 24);
        AddSimple(BEN, "António", "Silva", "A. Silva", 21, "POR", 187, 82, RIGHT, CB, 0, 84, 90, 50000000, 5000, 100000000, 5, 4);
        AddSimple(BEN, "Nicolás", "Otamendi", "Otamendi", 37, "ARG", 183, 82, RIGHT, CB, 0, 82, 82, 2000000, 8000, 4000000, 1, 30);
        AddSimple(BEN, "Tomás", "Araújo", "T. Araújo", 23, "POR", 188, 82, RIGHT, CB, RB_f, 78, 86, 25000000, 3000, 50000000, 5, 44);
        AddSimple(BEN, "Álvaro", "Fernández", "Álvaro F.", 22, "ESP", 182, 76, LEFT, LB, LW_f, 74, 84, 15000000, 2000, 30000000, 5, 3);
        AddSimple(BEN, "Alexander", "Bah", "Bah", 27, "DEN", 183, 78, RIGHT, RB, 0, 78, 81, 12000000, 5000, 25000000, 4, 6);
        AddSimple(BEN, "Juan", "Bernat", "Bernat", 32, "ESP", 170, 66, LEFT, LB, 0, 76, 78, 4000000, 6000, 8000000, 2, 14);
        AddSimple(BEN, "Florentino", "", "Florentino", 25, "POR", 184, 78, RIGHT, CDM, 0, 80, 84, 25000000, 5000, 50000000, 5, 61);
        AddSimple(BEN, "Orkun", "Kökçü", "Kökçü", 24, "TUR", 178, 72, RIGHT, CM, CDM_f, 82, 86, 35000000, 7000, 70000000, 5, 10);
        AddSimple(BEN, "Fredrik", "Aursnes", "Aursnes", 29, "NOR", 179, 74, LEFT, CM, LW_f, 82, 84, 20000000, 7000, 40000000, 4, 8);
        AddSimple(BEN, "Renato", "Sanches", "Renato Sanches", 27, "POR", 176, 72, RIGHT, CM, CDM_f, 78, 82, 12000000, 6000, 25000000, 4, 85);
        AddSimple(BEN, "Leandro", "Barreiro", "Barreiro", 25, "LUX", 175, 72, RIGHT, CDM, CM_f, 78, 83, 15000000, 4000, 30000000, 5, 18);
        AddSimple(BEN, "Benjamin", "Rollheiser", "Rollheiser", 25, "ARG", 172, 68, LEFT, RW, CAM_f, 76, 82, 12000000, 3000, 25000000, 5, 32);
        AddSimple(BEN, "Ángel", "Di María", "Di María", 37, "ARG", 178, 72, LEFT, RW, LW_f, 84, 84, 4000000, 15000, 8000000, 1, 11);
        AddSimple(BEN, "Kerem", "Aktürkoğlu", "Aktürkoğlu", 26, "TUR", 174, 72, RIGHT, LW, RW_f, 80, 84, 20000000, 6000, 40000000, 5, 7);
        AddSimple(BEN, "Andreas", "Schjelderup", "Schjelderup", 21, "DEN", 177, 72, RIGHT, LW, 0, 74, 84, 15000000, 1500, 30000000, 5, 47);
        AddSimple(BEN, "Tiago", "Gouveia", "Gouveia", 24, "POR", 174, 68, RIGHT, RW, LW_f, 74, 80, 6000000, 2000, 12000000, 4, 21);
        AddSimple(BEN, "Vangelis", "Pavlidis", "Pavlidis", 26, "GRE", 186, 80, RIGHT, ST, 0, 80, 83, 20000000, 6000, 40000000, 5, 9);
        AddSimple(BEN, "Arthur", "Cabral", "Arthur Cabral", 27, "BRA", 186, 82, RIGHT, ST, 0, 78, 82, 15000000, 6000, 30000000, 4, 19);
        AddSimple(BEN, "Zeki", "Amdouni", "Amdouni", 24, "SUI", 185, 78, RIGHT, ST, CAM_f, 76, 82, 12000000, 4000, 25000000, 5, 39);
        AddSimple(BEN, "Gianluca", "Prestianni", "Prestianni", 19, "ARG", 168, 66, RIGHT, RW, 0, 64, 84, 6000000, 500, 12000000, 5, 41);
        AddSimple(BEN, "João", "Rêgo", "João Rêgo", 20, "POR", 178, 72, RIGHT, LW, 0, 62, 80, 4000000, 200, 8000000, 5, 46);

        // ══════════════════════════════════════════════════════════════
        // Sporting CP (TeamId=103)
        // ══════════════════════════════════════════════════════════════
        int SCP = 103;
        AddSimple(SCP, "Franco", "Israel", "Israel", 25, "ARG", 190, 85, RIGHT, GK, 0, 78, 82, 12000000, 3000, 25000000, 5, 12);
        AddSimple(SCP, "Diego", "Callai", "Callai", 20, "BRA", 190, 84, RIGHT, GK, 0, 66, 78, 3000000, 200, 6000000, 5, 41);
        AddSimple(SCP, "Ousmane", "Diomande", "Diomande", 21, "CIV", 186, 80, RIGHT, CB, 0, 82, 88, 35000000, 3000, 70000000, 5, 26);
        AddSimple(SCP, "Gonçalo", "Inácio", "Inácio", 23, "POR", 185, 80, LEFT, CB, LB_f, 82, 87, 30000000, 5000, 60000000, 5, 25);
        AddSimple(SCP, "Eduardo", "Quaresma", "Quaresma", 23, "POR", 186, 80, RIGHT, CB, 0, 78, 84, 20000000, 3000, 40000000, 5, 72);
        AddSimple(SCP, "Matías", "Giménez", "M. Giménez", 22, "URU", 182, 76, LEFT, CB, 0, 72, 84, 12000000, 2000, 25000000, 5, 13);
        AddSimple(SCP, "Jerry", "St. Juste", "St. Juste", 28, "NED", 186, 78, RIGHT, CB, RB_f, 76, 79, 6000000, 5000, 12000000, 3, 3);
        AddSimple(SCP, "Nuno", "Santos", "Nuno Santos", 30, "POR", 177, 72, LEFT, LB, LW_f, 78, 81, 8000000, 5000, 16000000, 3, 11);
        AddSimple(SCP, "Iván", "Fresneda", "Fresneda", 20, "ESP", 180, 74, RIGHT, RB, 0, 74, 84, 15000000, 2000, 30000000, 5, 22);
        AddSimple(SCP, "Geovany", "Quenda", "Quenda", 18, "POR", 175, 70, RIGHT, RWB, RW_f, 68, 86, 10000000, 500, 20000000, 5, 47);
        AddSimple(SCP, "Hidemasa", "Morita", "Morita", 30, "JPN", 178, 74, RIGHT, CM, CDM_f, 80, 82, 12000000, 6000, 25000000, 3, 5);
        AddSimple(SCP, "Morten", "Hjulmand", "Hjulmand", 26, "DEN", 185, 80, RIGHT, CDM, CM_f, 82, 86, 30000000, 7000, 60000000, 5, 42);
        AddSimple(SCP, "Daniel", "Bragança", "Bragança", 26, "POR", 180, 75, RIGHT, CM, CAM_f, 78, 83, 15000000, 4000, 30000000, 5, 23);
        AddSimple(SCP, "Manuel", "Ugarte", "Ugarte", 24, "URU", 182, 78, RIGHT, CDM, 0, 84, 87, 40000000, 8000, 80000000, 5, 15);
        AddSimple(SCP, "Gonçalo", "Esteves", "Esteves", 22, "POR", 180, 76, RIGHT, CM, 0, 64, 80, 4000000, 500, 8000000, 5, 52);
        AddSimple(SCP, "Marcus", "Edwards", "Edwards", 26, "ENG", 170, 66, LEFT, RW, 0, 80, 83, 20000000, 6000, 40000000, 4, 18);
        AddSimple(SCP, "Trincão", "", "Trincão", 25, "POR", 184, 78, LEFT, RW, LW_f, 82, 85, 25000000, 6000, 50000000, 5, 17);
        AddSimple(SCP, "Pedro", "Gonçalves", "P. Gonçalves", 27, "POR", 175, 72, RIGHT, CAM, LW_f, 84, 86, 35000000, 8000, 70000000, 5, 8);
        AddSimple(SCP, "Nuno", "Tavares", "Nuno Tavares", 25, "POR", 183, 76, LEFT, LB, LW_f, 76, 82, 10000000, 4000, 20000000, 4, 10);
        AddSimple(SCP, "Viktor", "Gyökeres", "Gyökeres", 27, "SWE", 187, 84, RIGHT, ST, 0, 88, 91, 70000000, 12000, 140000000, 5, 9);
        AddSimple(SCP, "Conrad", "Harder", "Harder", 20, "DEN", 185, 78, RIGHT, ST, 0, 72, 86, 15000000, 1500, 30000000, 5, 19);
        AddSimple(SCP, "João", "Simões", "Simões", 19, "POR", 178, 72, RIGHT, ST, 0, 64, 82, 6000000, 500, 12000000, 5, 48);

        // ══════════════════════════════════════════════════════════════
        // Club Brugge (TeamId=104)
        // ══════════════════════════════════════════════════════════════
        int BRU = 104;
        AddSimple(BRU, "Simon", "Mignolet", "Mignolet", 37, "BEL", 193, 88, RIGHT, GK, 0, 80, 80, 1000000, 6000, 2000000, 2, 22);
        AddSimple(BRU, "Nordin", "Jackers", "Jackers", 27, "BEL", 186, 80, RIGHT, GK, 0, 74, 78, 4000000, 2000, 8000000, 4, 29);
        AddSimple(BRU, "Joel", "Ordóñez", "Ordóñez", 21, "ECU", 188, 82, RIGHT, CB, 0, 76, 86, 20000000, 2000, 40000000, 5, 4);
        AddSimple(BRU, "Brandon", "Mechele", "Mechele", 32, "BEL", 190, 84, RIGHT, CB, 0, 78, 80, 4000000, 5000, 8000000, 2, 44);
        AddSimple(BRU, "Jorn", "Spileers", "Spileers", 20, "BEL", 183, 76, RIGHT, CB, 0, 68, 84, 8000000, 500, 16000000, 5, 2);
        AddSimple(BRU, "Maximiliano", "Caufriez", "Caufriez", 28, "BEL", 185, 80, RIGHT, CB, CDM_f, 76, 79, 4000000, 3000, 8000000, 3, 28);
        AddSimple(BRU, "Björn", "Meijer", "Meijer", 22, "NED", 182, 76, LEFT, LB, 0, 74, 82, 10000000, 2000, 20000000, 5, 14);
        AddSimple(BRU, "Hugo", "Siquet", "Siquet", 22, "BEL", 178, 74, RIGHT, RB, 0, 72, 80, 6000000, 1500, 12000000, 5, 41);
        AddSimple(BRU, "Kyriani", "Sabbe", "Sabbe", 20, "BEL", 178, 72, RIGHT, RB, 0, 68, 80, 5000000, 500, 10000000, 5, 64);
        AddSimple(BRU, "Hans", "Vanaken", "Vanaken", 32, "BEL", 195, 86, RIGHT, CAM, CM_f, 84, 85, 15000000, 8000, 30000000, 3, 20);
        AddSimple(BRU, "Ardon", "Jashari", "Jashari", 22, "KVX", 178, 74, RIGHT, CM, CDM_f, 72, 82, 10000000, 2000, 20000000, 5, 30);
        AddSimple(BRU, "Raphaël", "Nzita", "Nzita", 22, "BEL", 180, 74, RIGHT, CM, 0, 66, 80, 4000000, 500, 8000000, 5, 46);
        AddSimple(BRU, "Éder", "Balanta", "Balanta", 32, "COL", 181, 76, LEFT, CDM, CM_f, 76, 79, 3000000, 4000, 6000000, 2, 3);
        AddSimple(BRU, "Hugo", "Vetlesen", "Vetlesen", 25, "NOR", 185, 78, RIGHT, CM, 0, 74, 80, 6000000, 3000, 12000000, 4, 27);
        AddSimple(BRU, "Chemsdine", "Talbi", "Talbi", 20, "BEL", 175, 70, RIGHT, LW, 0, 70, 82, 8000000, 500, 16000000, 5, 45);
        AddSimple(BRU, "Michał", "Skóraś", "Skóraś", 25, "POL", 175, 72, LEFT, RW, 0, 76, 81, 8000000, 3000, 16000000, 4, 10);
        AddSimple(BRU, "Andreas", "Skov Olsen", "Skov Olsen", 25, "DEN", 187, 80, LEFT, RW, 0, 80, 83, 20000000, 5000, 40000000, 4, 7);
        AddSimple(BRU, "Romeo", "Vermant", "Vermant", 21, "BEL", 178, 72, RIGHT, LW, 0, 64, 80, 4000000, 500, 8000000, 5, 63);
        AddSimple(BRU, "Gustaf", "Nilsson", "G. Nilsson", 28, "SWE", 196, 86, RIGHT, ST, 0, 78, 81, 8000000, 5000, 16000000, 3, 9);
        AddSimple(BRU, "Ferran", "Jutglà", "Jutglà", 26, "ESP", 175, 72, RIGHT, ST, LW_f, 76, 81, 8000000, 4000, 16000000, 4, 21);
        AddSimple(BRU, "Christos", "Tzolis", "Tzolis", 23, "GRE", 174, 70, RIGHT, LW, ST_f, 76, 82, 12000000, 3000, 25000000, 5, 11);
        AddSimple(BRU, "Shelby", "Allah", "Allah", 22, "BEL", 180, 74, RIGHT, ST, 0, 66, 80, 4000000, 500, 8000000, 5, 68);

        // ══════════════════════════════════════════════════════════════
        // Royale Union Saint-Gilloise (TeamId=105)
        // ══════════════════════════════════════════════════════════════
        int USG = 105;
        AddSimple(USG, "Anthony", "Moris", "Moris", 35, "LUX", 184, 78, RIGHT, GK, 0, 78, 79, 2000000, 4000, 4000000, 2, 49);
        AddSimple(USG, "Joachim", "Imbrechts", "Imbrechts", 23, "BEL", 185, 80, RIGHT, GK, 0, 68, 80, 3000000, 500, 6000000, 5, 1);
        AddSimple(USG, "Koki", "Machida", "Machida", 27, "JPN", 190, 84, LEFT, CB, 0, 78, 82, 10000000, 5000, 20000000, 4, 28);
        AddSimple(USG, "Christian", "Burgess", "Burgess", 33, "AUS", 198, 88, RIGHT, CB, 0, 76, 78, 2000000, 3000, 4000000, 2, 26);
        AddSimple(USG, "Federico", "Gattoni", "Gattoni", 26, "ARG", 183, 78, RIGHT, CB, 0, 76, 81, 8000000, 3000, 16000000, 4, 22);
        AddSimple(USG, "Ragnaar", "van Tricht", "van Tricht", 21, "BEL", 182, 76, RIGHT, CB, 0, 62, 78, 2000000, 500, 4000000, 5, 50);
        AddSimple(USG, "Khalil", "Fayad", "Fayad", 21, "BEL", 178, 72, LEFT, LB, 0, 68, 80, 4000000, 500, 8000000, 5, 21);
        AddSimple(USG, "Loïc", "Lapoussin", "Lapoussin", 29, "MAD", 175, 70, LEFT, LB, LW_f, 76, 80, 4000000, 3000, 8000000, 3, 2);
        AddSimple(USG, "Ousseynou", "Niang", "O. Niang", 23, "SEN", 186, 80, RIGHT, RB, 0, 72, 80, 6000000, 1500, 12000000, 5, 48);
        AddSimple(USG, "Charles", "Vanhoutte", "Vanhoutte", 26, "BEL", 178, 72, RIGHT, CDM, CM_f, 76, 81, 6000000, 3000, 12000000, 4, 8);
        AddSimple(USG, "Mathias", "Rasmussen", "Rasmussen", 27, "DEN", 182, 76, RIGHT, CM, CDM_f, 76, 81, 6000000, 3000, 12000000, 4, 10);
        AddSimple(USG, "Anouar", "Ait El Hadj", "Ait El Hadj", 23, "MAR", 175, 70, RIGHT, CAM, LW_f, 76, 82, 10000000, 2000, 20000000, 5, 23);
        AddSimple(USG, "Cameron", "Puertas", "Puertas", 27, "BEL", 173, 68, RIGHT, CAM, LW_f, 80, 83, 15000000, 6000, 30000000, 4, 11);
        AddSimple(USG, "Kévin", "Rodríguez", "Rodríguez", 25, "POR", 174, 68, RIGHT, LW, 0, 74, 80, 6000000, 2000, 12000000, 5, 29);
        AddSimple(USG, "Dennis", "Eckert", "Eckert", 28, "GER", 180, 76, RIGHT, ST, 0, 78, 82, 10000000, 5000, 20000000, 4, 93);
        AddSimple(USG, "Promise", "David", "P. David", 22, "NGA", 184, 78, RIGHT, ST, LW_f, 70, 80, 6000000, 1000, 12000000, 5, 46);
        AddSimple(USG, "Franjo", "Ivanović", "Ivanović", 21, "CRO", 186, 80, RIGHT, ST, 0, 68, 82, 6000000, 500, 12000000, 5, 77);
        AddSimple(USG, "Noah", "Sadi", "Sadi", 23, "BEL", 175, 72, RIGHT, RB, 0, 62, 76, 2000000, 500, 4000000, 5, 59);
        AddSimple(USG, "Adrien", "Sagna", "Sagna", 24, "FRA", 180, 74, RIGHT, CM, 0, 66, 78, 3000000, 1000, 6000000, 5, 60);
        AddSimple(USG, "Farkhod", "Olimzoda", "Olimzoda", 21, "TJK", 175, 70, RIGHT, LW, 0, 62, 76, 2000000, 200, 4000000, 5, 99);
        AddSimple(USG, "Mohamed", "Bouhout", "Bouhout", 22, "BEL", 182, 76, RIGHT, ST, 0, 64, 78, 3000000, 500, 6000000, 5, 44);
        AddSimple(USG, "Pedro", "Del Valle", "Del Valle", 24, "ESP", 180, 74, RIGHT, RW, 0, 66, 78, 3000000, 1000, 6000000, 5, 72);

        // ══════════════════════════════════════════════════════════════
        // Galatasaray (TeamId=106)
        // ══════════════════════════════════════════════════════════════
        int GAL = 106;
        AddSimple(GAL, "Fernando", "Muslera", "Muslera", 39, "URU", 190, 86, RIGHT, GK, 0, 80, 80, 500000, 6000, 1000000, 1, 1);
        AddSimple(GAL, "Günay", "Güvenç", "Güvenç", 33, "TUR", 187, 84, RIGHT, GK, 0, 74, 76, 1000000, 2000, 2000000, 2, 19);
        AddSimple(GAL, "Davinson", "Sánchez", "D. Sánchez", 29, "COL", 187, 82, RIGHT, CB, 0, 80, 82, 15000000, 7000, 30000000, 4, 6);
        AddSimple(GAL, "Victor", "Nelsson", "Nelsson", 26, "DEN", 186, 82, RIGHT, CB, 0, 80, 83, 20000000, 6000, 40000000, 5, 25);
        AddSimple(GAL, "Abdülkerim", "Bardakcı", "Bardakcı", 30, "TUR", 185, 80, LEFT, CB, 0, 80, 82, 12000000, 6000, 25000000, 4, 42);
        AddSimple(GAL, "Eren", "Elmalı", "Elmalı", 25, "TUR", 180, 76, LEFT, LB, 0, 76, 82, 12000000, 4000, 25000000, 5, 21);
        AddSimple(GAL, "Barış", "Yılmaz", "Barış Y.", 25, "TUR", 186, 80, RIGHT, RB, LW_f, 78, 83, 20000000, 5000, 40000000, 5, 53);
        AddSimple(GAL, "Elias", "Jelert", "Jelert", 22, "DEN", 183, 76, RIGHT, RB, 0, 74, 82, 12000000, 2000, 25000000, 5, 24);
        AddSimple(GAL, "Ali", "Turap", "A. Turap", 20, "TUR", 180, 74, LEFT, LB, 0, 64, 80, 4000000, 200, 8000000, 5, 90);
        AddSimple(GAL, "Lucas", "Torreira", "Torreira", 29, "URU", 168, 66, RIGHT, CDM, CM_f, 84, 85, 25000000, 8000, 50000000, 4, 34);
        AddSimple(GAL, "Gabriel", "Sara", "Gabriel Sara", 26, "BRA", 178, 73, RIGHT, CM, 0, 80, 84, 25000000, 6000, 50000000, 5, 20);
        AddSimple(GAL, "Roland", "Sallai", "Sallai", 28, "HUN", 183, 76, RIGHT, RW, CAM_f, 78, 82, 12000000, 6000, 25000000, 4, 12);
        AddSimple(GAL, "Kerem", "Demirbay", "Demirbay", 31, "TUR", 182, 76, LEFT, CM, 0, 78, 80, 6000000, 6000, 12000000, 3, 8);
        AddSimple(GAL, "Mario", "Lemina", "Lemina", 31, "GAB", 184, 82, RIGHT, CM, CDM_f, 78, 81, 8000000, 6000, 16000000, 3, 99);
        AddSimple(GAL, "Yunus", "Akgün", "Akgün", 24, "TUR", 173, 68, LEFT, LW, RW_f, 78, 82, 12000000, 3000, 25000000, 5, 11);
        AddSimple(GAL, "Gökdeniz", "Gürpüz", "Gürpüz", 20, "TUR", 178, 72, RIGHT, CM, 0, 62, 80, 3000000, 200, 6000000, 5, 63);
        AddSimple(GAL, "Hakim", "Ziyech", "Ziyech", 32, "MAR", 180, 72, LEFT, RW, CAM_f, 82, 83, 8000000, 10000, 16000000, 2, 22);
        AddSimple(GAL, "Dries", "Mertens", "Mertens", 38, "BEL", 169, 66, RIGHT, CAM, ST_f, 82, 82, 2000000, 10000, 4000000, 1, 10);
        AddSimple(GAL, "Victor", "Osimhen", "Osimhen", 26, "NGA", 186, 82, RIGHT, ST, 0, 88, 90, 70000000, 12000, 140000000, 4, 45);
        AddSimple(GAL, "Alvaro", "Morata", "Morata", 32, "ESP", 190, 84, RIGHT, ST, 0, 84, 85, 20000000, 12000, 40000000, 3, 9);
        AddSimple(GAL, "Mauro", "Icardi", "Icardi", 32, "ARG", 181, 78, RIGHT, ST, 0, 82, 83, 12000000, 10000, 25000000, 2, 9);
        AddSimple(GAL, "Ahmed", "Kutucu", "Kutucu", 24, "TUR", 181, 76, RIGHT, ST, 0, 74, 80, 6000000, 2000, 12000000, 4, 7);

        // ══════════════════════════════════════════════════════════════
        // Slavia Praha (TeamId=107)
        // ══════════════════════════════════════════════════════════════
        int SLA = 107;
        AddSimple(SLA, "Jindřich", "Staněk", "Staněk", 29, "CZE", 193, 86, RIGHT, GK, 0, 78, 82, 8000000, 4000, 16000000, 4, 36);
        AddSimple(SLA, "Antonín", "Kinský", "Kinský", 22, "CZE", 190, 84, RIGHT, GK, 0, 72, 84, 10000000, 1500, 20000000, 5, 31);
        AddSimple(SLA, "Tomáš", "Vlček", "T. Vlček", 23, "CZE", 188, 82, RIGHT, CB, 0, 70, 82, 8000000, 1500, 16000000, 5, 26);
        AddSimple(SLA, "David", "Zima", "Zima", 24, "CZE", 192, 84, RIGHT, CB, 0, 74, 82, 8000000, 2000, 16000000, 5, 4);
        AddSimple(SLA, "Jan", "Bořil", "Bořil", 33, "CZE", 175, 72, LEFT, LB, CB_f, 76, 78, 2000000, 3000, 4000000, 2, 18);
        AddSimple(SLA, "Matěj", "Jurásek", "Jurásek", 21, "CZE", 185, 80, LEFT, LB, LW_f, 74, 84, 12000000, 1500, 25000000, 5, 12);
        AddSimple(SLA, "Mick", "van Buren", "van Buren", 32, "NED", 183, 76, RIGHT, RB, 0, 74, 77, 2000000, 3000, 4000000, 2, 23);
        AddSimple(SLA, "Lukáš", "Masopust", "Masopust", 32, "CZE", 178, 74, RIGHT, RB, CM_f, 76, 78, 2000000, 3000, 4000000, 2, 5);
        AddSimple(SLA, "Christos", "Zafeiris", "Zafeiris", 22, "GRE", 178, 74, RIGHT, CM, 0, 74, 82, 10000000, 2000, 20000000, 5, 28);
        AddSimple(SLA, "Lukáš", "Provod", "Provod", 27, "CZE", 180, 74, RIGHT, CM, CAM_f, 78, 82, 10000000, 5000, 20000000, 5, 17);
        AddSimple(SLA, "Oscar", "Dorley", "Dorley", 26, "LBR", 178, 72, LEFT, CM, CDM_f, 76, 81, 6000000, 3000, 12000000, 4, 10);
        AddSimple(SLA, "David", "Douděra", "Douděra", 27, "CZE", 178, 74, RIGHT, CM, RB_f, 76, 81, 8000000, 4000, 16000000, 4, 19);
        AddSimple(SLA, "Petr", "Ševčík", "Ševčík", 31, "CZE", 172, 68, RIGHT, CM, 0, 78, 80, 6000000, 5000, 12000000, 3, 22);
        AddSimple(SLA, "Ondřej", "Zmrzlý", "Zmrzlý", 25, "CZE", 178, 74, RIGHT, CM, 0, 64, 78, 2000000, 500, 4000000, 5, 40);
        AddSimple(SLA, "Tomáš", "Čvančara", "Čvančara", 24, "CZE", 183, 78, RIGHT, LW, ST_f, 74, 82, 8000000, 2000, 16000000, 5, 15);
        AddSimple(SLA, "Mojmír", "Chytil", "Chytil", 26, "CZE", 185, 80, RIGHT, RW, 0, 76, 81, 8000000, 3000, 16000000, 4, 27);
        AddSimple(SLA, "Václav", "Jurečka", "Jurečka", 31, "CZE", 182, 78, RIGHT, ST, LW_f, 78, 80, 4000000, 5000, 8000000, 2, 11);
        AddSimple(SLA, "Tomáš", "Chorý", "Chorý", 30, "CZE", 200, 90, RIGHT, ST, 0, 78, 82, 8000000, 5000, 16000000, 4, 9);
        AddSimple(SLA, "El Hadji", "Malick", "Malick", 20, "SEN", 188, 82, RIGHT, ST, 0, 66, 82, 5000000, 500, 10000000, 5, 50);
        AddSimple(SLA, "Mickaël", "Pospíšil", "Pospíšil", 20, "CZE", 182, 76, RIGHT, LW, 0, 64, 78, 2000000, 200, 4000000, 5, 41);
        AddSimple(SLA, "Filip", "Prebsl", "Prebsl", 20, "CZE", 180, 74, RIGHT, CM, 0, 62, 78, 2000000, 200, 4000000, 5, 54);

        // ══════════════════════════════════════════════════════════════
        // FK Bodø/Glimt (TeamId=108)
        // ══════════════════════════════════════════════════════════════
        int BOD = 108;
        AddSimple(BOD, "Nikita", "Haidara", "Haidara", 28, "RUS", 186, 82, RIGHT, GK, 0, 74, 78, 3000000, 2000, 6000000, 3, 1);
        AddSimple(BOD, "Julian", "Faye Lund", "Faye Lund", 25, "NOR", 188, 82, RIGHT, GK, 0, 72, 78, 2000000, 1500, 4000000, 4, 12);
        AddSimple(BOD, "Isak", "Herman", "Herman", 22, "NOR", 185, 78, RIGHT, GK, 0, 64, 80, 2000000, 500, 4000000, 5, 29);
        AddSimple(BOD, "Fredrik", "Bjørkan", "Bjørkan", 26, "NOR", 180, 74, LEFT, LB, CB_f, 76, 82, 8000000, 3000, 16000000, 5, 3);
        AddSimple(BOD, "Brede", "Moe", "Moe", 33, "NOR", 192, 86, RIGHT, CB, 0, 74, 76, 1000000, 2000, 2000000, 1, 6);
        AddSimple(BOD, "Jostein", "Gundersen", "Gundersen", 29, "NOR", 189, 84, RIGHT, CB, 0, 74, 79, 3000000, 3000, 6000000, 3, 4);
        AddSimple(BOD, "Marius", "Broholm", "Broholm", 21, "NOR", 178, 72, RIGHT, RB, 0, 66, 82, 5000000, 500, 10000000, 5, 21);
        AddSimple(BOD, "Fredrik", "Sjøvold", "Sjøvold", 21, "NOR", 180, 74, RIGHT, RB, 0, 64, 82, 4000000, 500, 8000000, 5, 40);
        AddSimple(BOD, "Ulrik", "Saltnes", "Saltnes", 32, "NOR", 183, 78, RIGHT, CM, 0, 78, 80, 4000000, 5000, 8000000, 2, 14);
        AddSimple(BOD, "Sondre", "Brunstad Fet", "Fet", 27, "NOR", 178, 74, RIGHT, CM, 0, 76, 81, 5000000, 3000, 10000000, 4, 7);
        AddSimple(BOD, "Patrick", "Berg", "Berg", 27, "NOR", 182, 76, RIGHT, CDM, CM_f, 76, 81, 6000000, 3000, 12000000, 4, 26);
        AddSimple(BOD, "Håkon", "Evjen", "Evjen", 25, "NOR", 176, 72, RIGHT, CM, 0, 74, 80, 5000000, 2000, 10000000, 4, 8);
        AddSimple(BOD, "Albert", "Grønbæk", "Grønbæk", 23, "DEN", 182, 76, RIGHT, CM, CAM_f, 76, 84, 12000000, 2000, 25000000, 5, 25);
        AddSimple(BOD, "Joel", "Mvuka", "Mvuka", 22, "NOR", 176, 72, RIGHT, LW, 0, 72, 82, 8000000, 1500, 16000000, 5, 15);
        AddSimple(BOD, "Daniel", "Bass", "Bass", 21, "GHA", 178, 72, RIGHT, RW, 0, 64, 80, 3000000, 500, 6000000, 5, 99);
        AddSimple(BOD, "Jens", "Hauge", "Hauge", 25, "NOR", 184, 78, RIGHT, LW, RW_f, 76, 82, 10000000, 4000, 20000000, 5, 23);
        AddSimple(BOD, "August", "Mikkelsen", "Mikkelsen", 24, "NOR", 180, 76, RIGHT, ST, 0, 74, 80, 6000000, 2000, 12000000, 5, 9);
        AddSimple(BOD, "Kasper", "Høgh", "Høgh", 25, "DEN", 182, 78, RIGHT, ST, 0, 78, 82, 10000000, 3000, 20000000, 5, 20);
        AddSimple(BOD, "Sander", "Knutsen", "Knutsen", 21, "NOR", 184, 78, RIGHT, ST, 0, 64, 80, 3000000, 500, 6000000, 5, 41);
        AddSimple(BOD, "Oliver", "Soldal", "Soldal", 20, "NOR", 175, 70, RIGHT, LW, 0, 62, 78, 2000000, 200, 4000000, 5, 36);
        AddSimple(BOD, "Nino", "Zugelj", "Zugelj", 24, "SVN", 183, 76, RIGHT, RW, 0, 72, 80, 6000000, 2000, 12000000, 5, 10);

        // ══════════════════════════════════════════════════════════════
        // Olympiacos (TeamId=109)
        // ══════════════════════════════════════════════════════════════
        int OLY = 109;
        AddSimple(OLY, "Alexandros", "Paschalakis", "Paschalakis", 35, "GRE", 195, 87, RIGHT, GK, 0, 76, 78, 1000000, 3000, 2000000, 1, 1);
        AddSimple(OLY, "Konstantinos", "Tzolakis", "Tzolakis", 22, "GRE", 190, 84, RIGHT, GK, 0, 74, 84, 10000000, 1500, 20000000, 5, 88);
        AddSimple(OLY, "Panagiotis", "Retsos", "Retsos", 26, "GRE", 186, 82, RIGHT, CB, 0, 76, 82, 8000000, 3000, 16000000, 4, 45);
        AddSimple(OLY, "Lorenzo", "Pirola", "Pirola", 23, "ITA", 186, 80, LEFT, CB, 0, 74, 82, 10000000, 2000, 20000000, 5, 5);
        AddSimple(OLY, "David", "Carmo", "David Carmo", 25, "POR", 196, 86, LEFT, CB, 0, 76, 82, 10000000, 3000, 20000000, 5, 16);
        AddSimple(OLY, "André", "Horta", "Horta", 28, "POR", 184, 78, RIGHT, CB, 0, 72, 78, 3000000, 2000, 6000000, 4, 34);
        AddSimple(OLY, "Omar", "Richards", "Richards", 27, "ENG", 175, 72, LEFT, LB, 0, 74, 80, 6000000, 3000, 12000000, 4, 3);
        AddSimple(OLY, "Giulian", "Biancone", "Biancone", 25, "FRA", 183, 76, RIGHT, RB, CB_f, 74, 80, 6000000, 2000, 12000000, 4, 99);
        AddSimple(OLY, "Francisco", "Ortega", "Ortega", 25, "ARG", 180, 74, RIGHT, RB, 0, 62, 78, 2000000, 500, 4000000, 5, 74);
        AddSimple(OLY, "Santiago", "Hezze", "Hezze", 23, "ARG", 178, 74, LEFT, CM, CDM_f, 76, 84, 12000000, 2000, 25000000, 5, 32);
        AddSimple(OLY, "Chiquinho", "", "Chiquinho", 29, "POR", 176, 72, RIGHT, CM, 0, 78, 82, 10000000, 5000, 20000000, 4, 22);
        AddSimple(OLY, "Vicente", "Iborra", "Iborra", 37, "ESP", 190, 86, RIGHT, CDM, CM_f, 76, 77, 500000, 4000, 1000000, 1, 4);
        AddSimple(OLY, "Marko", "Stamenic", "Stamenic", 23, "NZL", 187, 80, RIGHT, CDM, 0, 72, 82, 8000000, 1500, 16000000, 5, 70);
        AddSimple(OLY, "Anastasios", "Bakasetas", "Bakasetas", 32, "GRE", 181, 76, RIGHT, CAM, CM_f, 82, 83, 8000000, 8000, 16000000, 2, 10);
        AddSimple(OLY, "Gelson", "Martins", "Gelson M.", 30, "POR", 178, 72, RIGHT, LW, RW_f, 80, 82, 8000000, 6000, 16000000, 3, 77);
        AddSimple(OLY, "Ayoub", "El Kaabi", "El Kaabi", 32, "MAR", 180, 76, RIGHT, ST, 0, 78, 81, 6000000, 6000, 12000000, 2, 9);
        AddSimple(OLY, "Fran", "Navarro", "Fran Navarro", 27, "ESP", 180, 76, RIGHT, ST, 0, 74, 80, 6000000, 3000, 12000000, 4, 19);
        AddSimple(OLY, "Georgios", "Masouras", "Masouras", 31, "GRE", 183, 76, RIGHT, LW, 0, 76, 79, 4000000, 4000, 8000000, 2, 7);
        AddSimple(OLY, "Rodinei", "", "Rodinei", 32, "BRA", 178, 74, RIGHT, RB, 0, 76, 79, 3000000, 4000, 6000000, 3, 23);
        AddSimple(OLY, "Weligton", "", "Weligton", 38, "BRA", 186, 82, LEFT, CB, 0, 72, 73, 200000, 2000, 500000, 1, 20);
        AddSimple(OLY, "Youssef", "El-Arabi", "El-Arabi", 38, "MAR", 184, 78, RIGHT, ST, 0, 78, 78, 500000, 6000, 1000000, 1, 11);
        AddSimple(OLY, "Apostolos", "Vrakas", "Vrakas", 20, "GRE", 180, 74, RIGHT, CM, 0, 62, 78, 2000000, 200, 4000000, 5, 51);

        // ══════════════════════════════════════════════════════════════
        // FC København (TeamId=110)
        // ══════════════════════════════════════════════════════════════
        int COP = 110;
        AddSimple(COP, "Kamil", "Grabara", "Grabara", 26, "POL", 191, 86, RIGHT, GK, 0, 80, 84, 15000000, 4000, 30000000, 5, 1);
        AddSimple(COP, "Theo", "Sander", "Sander", 20, "DEN", 189, 82, RIGHT, GK, 0, 66, 82, 5000000, 500, 10000000, 5, 31);
        AddSimple(COP, "Denis", "Vavro", "Vavro", 29, "SVK", 189, 84, RIGHT, CB, 0, 78, 82, 8000000, 5000, 16000000, 4, 6);
        AddSimple(COP, "David", "Khocholava", "Khocholava", 32, "GEO", 192, 84, RIGHT, CB, 0, 76, 79, 3000000, 4000, 6000000, 2, 3);
        AddSimple(COP, "Marcus", "Pedersen", "M. Pedersen", 24, "NOR", 180, 76, LEFT, LB, 0, 72, 80, 6000000, 1500, 12000000, 5, 24);
        AddSimple(COP, "Kevin", "Diks", "Diks", 28, "NED", 182, 76, RIGHT, RB, 0, 78, 82, 10000000, 5000, 20000000, 4, 2);
        AddSimple(COP, "Gabriel", "Pereira", "G. Pereira", 25, "BRA", 182, 76, RIGHT, RB, 0, 72, 80, 5000000, 2000, 10000000, 5, 22);
        AddSimple(COP, "Lukas", "Lerager", "Lerager", 31, "DEN", 186, 82, RIGHT, CM, 0, 78, 81, 6000000, 5000, 12000000, 3, 16);
        AddSimple(COP, "Magnus", "Mattsson", "Mattsson", 26, "DEN", 178, 74, RIGHT, CM, CAM_f, 76, 82, 8000000, 3000, 16000000, 5, 23);
        AddSimple(COP, "Rasmus", "Falk", "Falk", 33, "DEN", 176, 72, RIGHT, RW, CM_f, 76, 78, 2000000, 4000, 4000000, 2, 22);
        AddSimple(COP, "Elias", "Achouri", "Achouri", 26, "TUN", 175, 70, RIGHT, CAM, 0, 74, 80, 6000000, 3000, 12000000, 4, 45);
        AddSimple(COP, "Mohamed", "Elyounoussi", "Elyounoussi", 30, "NOR", 178, 72, RIGHT, LW, 0, 78, 81, 8000000, 6000, 16000000, 3, 10);
        AddSimple(COP, "Giorgi", "Gocholeishvili", "Gocholeishvili", 24, "GEO", 183, 76, RIGHT, RW, 0, 74, 80, 6000000, 2000, 12000000, 5, 20);
        AddSimple(COP, "Andreas", "Cornelius", "Cornelius", 32, "DEN", 193, 86, RIGHT, ST, 0, 78, 80, 6000000, 6000, 12000000, 2, 9);
        AddSimple(COP, "German", "Onugkha", "Onugkha", 28, "RUS", 192, 86, RIGHT, ST, 0, 76, 81, 6000000, 3000, 12000000, 4, 11);
        AddSimple(COP, "Orri", "Óskarsson", "Óskarsson", 20, "ISL", 186, 80, RIGHT, ST, 0, 70, 84, 10000000, 1000, 20000000, 5, 18);
        AddSimple(COP, "Victor", "Froholdt", "Froholdt", 20, "DEN", 180, 74, RIGHT, CM, 0, 62, 78, 2000000, 200, 4000000, 5, 37);
        AddSimple(COP, "Mohanad", "Yahya", "Yahya", 22, "DEN", 178, 72, RIGHT, RB, 0, 62, 78, 2000000, 500, 4000000, 5, 41);
        AddSimple(COP, "Rasmus", "Højer", "Højer", 30, "DEN", 175, 72, LEFT, LB, 0, 72, 77, 2000000, 3000, 4000000, 3, 27);
        AddSimple(COP, "Thomas", "Jørgensen", "T. Jørgensen", 19, "DEN", 184, 78, RIGHT, ST, 0, 62, 80, 3000000, 200, 6000000, 5, 43);
        AddSimple(COP, "Khaled", "Ayoubi", "Ayoubi", 24, "DEN", 180, 74, RIGHT, LW, 0, 64, 78, 2000000, 500, 4000000, 5, 46);
        AddSimple(COP, "Robert", "", "Robert", 27, "BRA", 176, 72, RIGHT, CM, 0, 64, 78, 2000000, 500, 4000000, 5, 49);

        // ══════════════════════════════════════════════════════════════
        // Pafos FC (TeamId=111)
        // ══════════════════════════════════════════════════════════════
        int PAF = 111;
        AddSimple(PAF, "Ivica", "Ivušić", "Ivušić", 30, "CRO", 195, 86, RIGHT, GK, 0, 76, 80, 4000000, 3000, 8000000, 3, 1);
        AddSimple(PAF, "Filip", "Đukić", "Đukić", 24, "SRB", 190, 84, RIGHT, GK, 0, 70, 78, 2000000, 1000, 4000000, 5, 95);
        AddSimple(PAF, "Matúš", "Hrošovský", "Hrošovský", 32, "SVK", 182, 76, RIGHT, CDM, CM_f, 74, 77, 1000000, 3000, 2000000, 2, 26);
        AddSimple(PAF, "Václav", "Jemelka", "Jemelka", 29, "CZE", 188, 82, LEFT, CB, 0, 72, 78, 2000000, 2000, 4000000, 4, 4);
        AddSimple(PAF, "Josef", "Kvida", "Kvida", 28, "CZE", 190, 84, RIGHT, CB, 0, 72, 78, 2000000, 2000, 4000000, 4, 23);
        AddSimple(PAF, "Konstantinos", "Pileas", "Pileas", 24, "CYP", 182, 76, RIGHT, CB, 0, 64, 78, 2000000, 500, 4000000, 5, 55);
        AddSimple(PAF, "Manuel", "Baumgartner", "Baumgartner", 26, "AUT", 180, 74, LEFT, LB, LW_f, 72, 78, 2000000, 2000, 4000000, 4, 2);
        AddSimple(PAF, "Sammy", "Diawara", "Diawara", 26, "SEN", 184, 78, RIGHT, RB, 0, 70, 76, 1500000, 1500, 3000000, 4, 20);
        AddSimple(PAF, "Anderson", "", "Anderson", 28, "BRA", 178, 74, RIGHT, CM, 0, 72, 78, 2000000, 2000, 4000000, 3, 17);
        AddSimple(PAF, "João", "Correia", "J. Correia", 29, "POR", 176, 72, RIGHT, CM, 0, 70, 76, 1500000, 2000, 3000000, 3, 7);
        AddSimple(PAF, "Marios", "Coulouris", "Coulouris", 23, "CYP", 180, 74, RIGHT, CM, 0, 66, 78, 2000000, 500, 4000000, 5, 70);
        AddSimple(PAF, "Muamer", "Tanković", "Tanković", 30, "SWE", 178, 74, RIGHT, CAM, LW_f, 74, 78, 2000000, 3000, 4000000, 3, 8);
        AddSimple(PAF, "Andreas", "Ioannou", "Ioannou", 26, "CYP", 180, 74, RIGHT, LW, 0, 68, 76, 1500000, 1500, 3000000, 4, 11);
        AddSimple(PAF, "Sonny", "Tonny", "S. Tonny", 23, "NGA", 182, 78, RIGHT, ST, 0, 74, 82, 6000000, 1500, 12000000, 5, 9);
        AddSimple(PAF, "Kosta", "Streller", "Streller", 22, "SUI", 188, 80, RIGHT, ST, 0, 62, 78, 2000000, 500, 4000000, 5, 93);
        AddSimple(PAF, "Bruno", "Felipe", "Bruno Felipe", 29, "BRA", 188, 82, RIGHT, ST, 0, 70, 76, 1500000, 2000, 3000000, 3, 22);
        AddSimple(PAF, "Phillipos", "Nikolaou", "Nikolaou", 21, "CYP", 178, 72, RIGHT, LW, 0, 62, 76, 2000000, 200, 4000000, 5, 77);
        AddSimple(PAF, "Romain", "Montiel", "Montiel", 29, "FRA", 180, 74, RIGHT, RB, 0, 72, 78, 2000000, 2000, 4000000, 3, 15);
        AddSimple(PAF, "Iván", "Suárez", "I. Suárez", 26, "ESP", 174, 70, RIGHT, RW, 0, 68, 76, 1500000, 1500, 3000000, 4, 88);
        AddSimple(PAF, "Nikolas", "Pavlou", "Pavlou", 21, "CYP", 178, 72, RIGHT, CM, 0, 62, 76, 2000000, 200, 4000000, 5, 44);
        AddSimple(PAF, "David", "Goldar", "Goldar", 30, "ESP", 186, 80, RIGHT, CB, 0, 72, 78, 2000000, 3000, 4000000, 3, 6);

        // ══════════════════════════════════════════════════════════════
        // Qarabağ FK (TeamId=112)
        // ══════════════════════════════════════════════════════════════
        int QAR = 112;
        AddSimple(QAR, "Shahrudin", "Mahammadaliyev", "Mahammadaliyev", 31, "AZE", 190, 84, RIGHT, GK, 0, 74, 78, 1000000, 2000, 2000000, 3, 1);
        AddSimple(QAR, "Fabijan", "Buntić", "Buntić", 28, "CRO", 194, 86, RIGHT, GK, 0, 72, 78, 1000000, 2000, 2000000, 3, 97);
        AddSimple(QAR, "Bahlul", "Mustafazada", "Mustafazada", 28, "AZE", 186, 80, RIGHT, CB, 0, 70, 76, 1000000, 1500, 2000000, 4, 81);
        AddSimple(QAR, "Kevin", "Medina", "K. Medina", 31, "COL", 185, 80, RIGHT, CB, 0, 74, 78, 1000000, 2000, 2000000, 3, 55);
        AddSimple(QAR, "Rahil", "Mammadov", "R. Mammadov", 29, "AZE", 184, 78, RIGHT, CB, 0, 68, 74, 800000, 1500, 1600000, 3, 30);
        AddSimple(QAR, "Torunay", "", "Torunay", 24, "BRA", 190, 84, LEFT, CB, 0, 72, 80, 3000000, 1500, 6000000, 5, 13);
        AddSimple(QAR, "Elvin", "Jafarguliyev", "Jafarguliyev", 24, "AZE", 178, 74, LEFT, LB, 0, 68, 76, 1000000, 1000, 2000000, 5, 27);
        AddSimple(QAR, "Maksim", "Medvedev", "Medvedev", 35, "AZE", 182, 76, RIGHT, RB, CB_f, 72, 74, 500000, 2000, 1000000, 1, 5);
        AddSimple(QAR, "Matheus", "Silva", "Matheus Silva", 27, "BRA", 182, 76, RIGHT, RB, 0, 72, 78, 1500000, 2000, 3000000, 4, 29);
        AddSimple(QAR, "Marko", "Vešović", "Vešović", 33, "MNE", 185, 78, RIGHT, RB, 0, 72, 75, 500000, 2000, 1000000, 2, 2);
        AddSimple(QAR, "Julio", "Romão", "Romão", 30, "BRA", 180, 74, RIGHT, CDM, CM_f, 74, 78, 1500000, 2000, 3000000, 3, 6);
        AddSimple(QAR, "Richard", "Almeida", "R. Almeida", 35, "BRA", 178, 74, RIGHT, CM, 0, 74, 76, 800000, 3000, 1600000, 2, 8);
        AddSimple(QAR, "Patrick", "Andrade", "P. Andrade", 32, "CPV", 178, 74, RIGHT, CM, 0, 74, 78, 1500000, 3000, 3000000, 3, 20);
        AddSimple(QAR, "Kady", "", "Kady", 28, "BRA", 180, 76, RIGHT, CAM, LW_f, 74, 80, 3000000, 3000, 6000000, 4, 10);
        AddSimple(QAR, "Tural", "Bayramov", "Bayramov", 23, "AZE", 176, 72, RIGHT, LW, 0, 66, 76, 800000, 500, 1600000, 5, 19);
        AddSimple(QAR, "Leandro", "Andrade", "L. Andrade", 26, "BRA", 178, 74, RIGHT, LW, 0, 72, 78, 2000000, 1500, 4000000, 5, 7);
        AddSimple(QAR, "Yassine", "Benzia", "Benzia", 30, "ALG", 175, 72, RIGHT, RW, CAM_f, 74, 78, 2000000, 3000, 4000000, 3, 21);
        AddSimple(QAR, "Nahid", "Rzayev", "Rzayev", 21, "AZE", 178, 72, RIGHT, RW, 0, 62, 76, 800000, 200, 1600000, 5, 44);
        AddSimple(QAR, "Abdellah", "Zoubir", "Zoubir", 33, "FRA", 178, 72, RIGHT, RW, 0, 74, 77, 1000000, 3000, 2000000, 2, 11);
        AddSimple(QAR, "Redon", "Xhixha", "Xhixha", 27, "ALB", 186, 80, RIGHT, ST, 0, 72, 78, 2000000, 2000, 4000000, 4, 18);
        AddSimple(QAR, "Narcis", "Lustig", "Lustig", 22, "ROU", 182, 76, RIGHT, ST, 0, 64, 78, 1000000, 200, 2000000, 5, 90);
        AddSimple(QAR, "Hamidou", "Keyta", "Keyta", 29, "SEN", 188, 82, RIGHT, ST, 0, 72, 78, 1500000, 2000, 3000000, 3, 15);

        // ══════════════════════════════════════════════════════════════
        // Kairat Almaty (TeamId=113)
        // ══════════════════════════════════════════════════════════════
        int KAI = 113;
        AddSimple(KAI, "Stas", "Pokatilov", "Pokatilov", 32, "KAZ", 190, 84, RIGHT, GK, 0, 72, 76, 500000, 1500, 1000000, 3, 1);
        AddSimple(KAI, "Danil", "Ustimenko", "Ustimenko", 24, "KAZ", 188, 82, RIGHT, GK, 0, 66, 76, 800000, 500, 1600000, 5, 30);
        AddSimple(KAI, "Aleksandr", "Marochkin", "Marochkin", 34, "KAZ", 190, 84, RIGHT, CB, 0, 70, 74, 300000, 1500, 600000, 2, 15);
        AddSimple(KAI, "Damir", "Kasymov", "Kasymov", 29, "KAZ", 186, 80, RIGHT, CB, 0, 68, 74, 500000, 1500, 1000000, 3, 33);
        AddSimple(KAI, "Roman", "Asrankulov", "Asrankulov", 24, "KAZ", 184, 78, LEFT, CB, 0, 64, 76, 800000, 500, 1600000, 5, 3);
        AddSimple(KAI, "Mikhail", "Gordeychuk", "Gordeychuk", 33, "KAZ", 178, 74, RIGHT, LB, LW_f, 70, 74, 500000, 2000, 1000000, 2, 23);
        AddSimple(KAI, "Egor", "Karpitskiy", "Karpitskiy", 21, "KAZ", 180, 74, RIGHT, RB, 0, 62, 76, 800000, 200, 1600000, 5, 44);
        AddSimple(KAI, "Abzal", "Kenesbek", "Kenesbek", 23, "KAZ", 178, 72, RIGHT, RB, 0, 64, 76, 800000, 500, 1600000, 5, 21);
        AddSimple(KAI, "João", "Paulo", "João Paulo", 26, "BRA", 182, 76, RIGHT, CDM, CM_f, 72, 78, 1500000, 2000, 3000000, 4, 55);
        AddSimple(KAI, "Yuriy", "Pertsukh", "Pertsukh", 27, "KAZ", 180, 74, RIGHT, CM, 0, 68, 76, 800000, 1500, 1600000, 4, 6);
        AddSimple(KAI, "Lev", "Skvortsov", "Skvortsov", 25, "KAZ", 176, 72, RIGHT, CM, 0, 66, 76, 800000, 1000, 1600000, 5, 7);
        AddSimple(KAI, "Gleb", "Valgushev", "Valgushev", 23, "KAZ", 178, 72, RIGHT, CM, 0, 62, 76, 800000, 500, 1600000, 5, 55);
        AddSimple(KAI, "Arsen", "Buranchiev", "Buranchiev", 24, "KGZ", 180, 74, RIGHT, CAM, 0, 66, 76, 800000, 500, 1600000, 5, 11);
        AddSimple(KAI, "Arthur", "Shushenachev", "Shushenachev", 27, "RUS", 174, 70, RIGHT, LW, 0, 68, 76, 1000000, 1500, 2000000, 4, 10);
        AddSimple(KAI, "Vyacheslav", "Shvyrev", "Shvyrev", 24, "KAZ", 178, 72, RIGHT, LW, 0, 66, 76, 800000, 1000, 1600000, 5, 77);
        AddSimple(KAI, "Giannis", "Bougoussis", "Bougoussis", 29, "GRE", 180, 76, RIGHT, RW, 0, 70, 76, 1000000, 1500, 2000000, 4, 18);
        AddSimple(KAI, "Ivan", "Yakovlev", "Yakovlev", 24, "KAZ", 182, 78, RIGHT, ST, 0, 64, 76, 800000, 500, 1600000, 5, 87);
        AddSimple(KAI, "Ivan", "Sviridov", "Sviridov", 22, "KAZ", 184, 78, RIGHT, ST, 0, 62, 76, 800000, 200, 1600000, 5, 88);
        AddSimple(KAI, "Sanzhar", "Satybaldiev", "Satybaldiev", 21, "KAZ", 180, 74, RIGHT, ST, 0, 62, 76, 800000, 200, 1600000, 5, 99);
        AddSimple(KAI, "Ivan", "Pechenkin", "Pechenkin", 24, "KAZ", 186, 80, RIGHT, CB, 0, 64, 76, 800000, 500, 1600000, 5, 35);
        AddSimple(KAI, "Didar", "Zhalmukan", "Zhalmukan", 29, "KAZ", 172, 68, RIGHT, LW, 0, 68, 74, 500000, 1500, 1000000, 3, 13);

        // ══════════════════════════════════════════════════════════════
        // FCSB (TeamId=114)
        // ══════════════════════════════════════════════════════════════
        int FCS = 114;
        AddSimple(FCS, "Ștefan", "Târnovanu", "Târnovanu", 25, "ROU", 190, 84, RIGHT, GK, 0, 74, 82, 8000000, 2000, 16000000, 5, 1);
        AddSimple(FCS, "Mihai", "Popa", "M. Popa", 23, "ROU", 186, 80, RIGHT, GK, 0, 68, 78, 2000000, 800, 4000000, 5, 32);
        AddSimple(FCS, "Valentin", "Crețu", "Crețu", 35, "ROU", 180, 76, RIGHT, RB, 0, 72, 74, 500000, 2000, 1000000, 1, 2);
        AddSimple(FCS, "Risto", "Radunović", "Radunović", 33, "MNE", 190, 84, RIGHT, CB, 0, 74, 77, 1000000, 2000, 2000000, 2, 33);
        AddSimple(FCS, "Gabriel", "Toma", "G. Toma", 27, "ROU", 182, 76, RIGHT, CB, 0, 62, 76, 1000000, 500, 2000000, 5, 99);
        AddSimple(FCS, "Iulian", "Cristea", "Cristea", 23, "ROU", 183, 76, LEFT, LB, CB_f, 68, 80, 3000000, 800, 6000000, 5, 24);
        AddSimple(FCS, "Alexandru", "Pantea", "Pantea", 21, "ROU", 180, 74, RIGHT, RB, 0, 64, 78, 2000000, 500, 4000000, 5, 28);
        AddSimple(FCS, "Malcom", "Edjouma", "Edjouma", 28, "FRA", 184, 78, RIGHT, CDM, CB_f, 74, 79, 2000000, 3000, 4000000, 3, 8);
        AddSimple(FCS, "Darius", "Olaru", "Olaru", 27, "ROU", 178, 74, RIGHT, CM, CAM_f, 76, 82, 8000000, 4000, 16000000, 5, 27);
        AddSimple(FCS, "Adrian", "Șut", "Șut", 24, "ROU", 182, 78, RIGHT, CDM, 0, 74, 82, 6000000, 2000, 12000000, 5, 6);
        AddSimple(FCS, "Ovidiu", "Popescu", "O. Popescu", 31, "ROU", 182, 76, RIGHT, CM, 0, 72, 76, 1500000, 3000, 3000000, 2, 7);
        AddSimple(FCS, "Ștefan", "Bălănescu", "Bălănescu", 24, "ROU", 176, 72, RIGHT, CM, 0, 64, 78, 1500000, 500, 3000000, 5, 45);
        AddSimple(FCS, "David", "Miculescu", "Miculescu", 24, "ROU", 184, 78, RIGHT, LW, RW_f, 74, 80, 4000000, 2000, 8000000, 5, 11);
        AddSimple(FCS, "Alexandru", "Băluță", "Băluță", 30, "ROU", 167, 64, RIGHT, RW, 0, 74, 78, 2000000, 3000, 4000000, 3, 10);
        AddSimple(FCS, "Florin", "Tănase", "Tănase", 30, "ROU", 185, 80, RIGHT, ST, LW_f, 76, 80, 4000000, 4000, 8000000, 3, 7);
        AddSimple(FCS, "Daniel", "Bîrligea", "Bîrligea", 25, "ROU", 184, 78, RIGHT, ST, 0, 72, 80, 4000000, 2000, 8000000, 5, 9);
        AddSimple(FCS, "Andrea", "Compagno", "Compagno", 28, "ITA", 186, 82, RIGHT, ST, 0, 74, 80, 3000000, 3000, 6000000, 4, 9);
        AddSimple(FCS, "Andrei", "Gheorghiță", "Gheorghiță", 22, "ROU", 178, 72, RIGHT, LW, 0, 66, 78, 2000000, 500, 4000000, 5, 96);
        AddSimple(FCS, "Vlad", "Mihăilă", "Mihăilă", 24, "ROU", 180, 74, RIGHT, LW, 0, 68, 78, 2000000, 500, 4000000, 5, 77);
        AddSimple(FCS, "Cristian", "Manea", "Manea", 27, "ROU", 176, 72, LEFT, LB, 0, 70, 76, 1000000, 1500, 2000000, 4, 15);
        AddSimple(FCS, "Mihnea", "Claudiu", "Claudiu", 21, "ROU", 178, 72, RIGHT, ST, 0, 62, 76, 1000000, 200, 2000000, 5, 98);
        AddSimple(FCS, "Siyabonga", "Ngezana", "Ngezana", 27, "RSA", 190, 84, RIGHT, CB, 0, 74, 80, 4000000, 2500, 8000000, 4, 30);

        // ══════════════════════════════════════════════════════════════
        // EUROPA LEAGUE 2024/25 — Teams 115-148
        // ══════════════════════════════════════════════════════════════

        // ══════════════════════════════════════════════════════════════
        // Team int ATA (TeamId=115)
        // ══════════════════════════════════════════════════════════════
        int ATA = 115;
        AddSimple(ATA, "Juan", "Musso", "J. Musso", 28, "ARG", 191, 86, RIGHT, GK, 0, 79, 83, 18000000, 45000, 36000000, 1, 1);
        AddSimple(ATA, "Marten", "de Roon", "M. de Roon", 32, "NED", 185, 78, RIGHT, CM, 0, 79, 79, 8000000, 35000, 16000000, 2, 1);
        AddSimple(ATA, "Ademola", "Lookman", "A. Lookman", 26, "NGA", 174, 72, RIGHT, LW, 108, 80, 82, 25000000, 55000, 50000000, 3, 1);
        AddSimple(ATA, "Teun", "Koopmeiners", "T. Koopmeiners", 25, "NED", 183, 75, RIGHT, CAM, 105, 83, 85, 40000000, 60000, 80000000, 4, 1);
        AddSimple(ATA, "Gianluca", "Scamacca", "G. Scamacca", 25, "ITA", 195, 85, RIGHT, ST, 0, 79, 83, 30000000, 50000, 60000000, 5, 1);
        AddSimple(ATA, "Davide", "Zappacosta", "D. Zappacosta", 32, "ITA", 182, 74, RIGHT, RB, 0, 76, 76, 3000000, 25000, 6000000, 5, 1);
        AddSimple(ATA, "Matteo", "Ruggeri", "M. Ruggeri", 21, "ITA", 178, 70, LEFT, LB, 0, 76, 84, 15000000, 20000, 30000000, 5, 1);
        AddSimple(ATA, "Isak", "Hien", "I. Hien", 25, "SWE", 191, 82, RIGHT, CB, 0, 77, 80, 12000000, 25000, 24000000, 5, 1);
        AddSimple(ATA, "Sead", "Kolašinac", "S. Kolašinac", 31, "BIH", 183, 80, LEFT, LB, 101, 76, 76, 4000000, 30000, 8000000, 5, 1);
        AddSimple(ATA, "Ederson", "José", "Ederson", 24, "BRA", 183, 75, RIGHT, CDM, 105, 77, 83, 20000000, 30000, 40000000, 5, 1);
        AddSimple(ATA, "Mario", "Pašalić", "M. Pašalić", 28, "CRO", 188, 82, RIGHT, CAM, 105, 78, 79, 15000000, 40000, 30000000, 5, 1);
        AddSimple(ATA, "Rafael", "Tolói", "R. Tolói", 33, "ITA", 186, 78, RIGHT, CB, 0, 80, 80, 5000000, 35000, 10000000, 5, 1);
        AddSimple(ATA, "Berat", "Djimsiti", "B. Djimsiti", 30, "ALB", 190, 82, RIGHT, CB, 0, 77, 77, 8000000, 25000, 16000000, 5, 1);
        AddSimple(ATA, "Hans", "Hateboer", "H. Hateboer", 30, "NED", 185, 78, RIGHT, RB, 0, 75, 75, 4000000, 25000, 8000000, 5, 1);
        AddSimple(ATA, "Francesco", "Rossi", "F. Rossi", 28, "ITA", 188, 80, RIGHT, GK, 0, 74, 78, 5000000, 15000, 10000000, 5, 1);
        AddSimple(ATA, "Mitchel", "Bakker", "M. Bakker", 23, "NED", 185, 75, LEFT, LB, 0, 74, 80, 8000000, 20000, 16000000, 5, 1);
        AddSimple(ATA, "Giorgio", "Scalvini", "G. Scalvini", 20, "ITA", 194, 80, RIGHT, CB, 104, 78, 88, 35000000, 15000, 70000000, 5, 1);
        AddSimple(ATA, "Charles", "De Ketelaere", "C. De Ketelaere", 23, "BEL", 192, 80, LEFT, CAM, 109, 76, 84, 20000000, 25000, 40000000, 5, 1);
        AddSimple(ATA, "El Bilal", "Touré", "E. B. Touré", 22, "MLI", 185, 78, RIGHT, ST, 0, 74, 82, 15000000, 20000, 30000000, 5, 1);
        AddSimple(ATA, "Aleksei", "Miranchuk", "A. Miranchuk", 28, "RUS", 184, 76, LEFT, CAM, 0, 77, 77, 8000000, 30000, 16000000, 5, 1);
        AddSimple(ATA, "Nadir", "Zortea", "N. Zortea", 24, "ITA", 183, 73, RIGHT, RB, 102, 73, 78, 3000000, 15000, 6000000, 5, 1);
        AddSimple(ATA, "Luis", "Muriel", "L. Muriel", 32, "COL", 179, 73, RIGHT, ST, 108, 78, 78, 6000000, 30000, 12000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int NFO (TeamId=116)
        // ══════════════════════════════════════════════════════════════
        int NFO = 116;
        AddSimple(NFO, "Matt", "Turner", "M. Turner", 29, "USA", 191, 86, RIGHT, GK, 0, 76, 78, 8000000, 25000, 16000000, 1, 1);
        AddSimple(NFO, "Willy", "Boly", "W. Boly", 32, "CIV", 195, 85, RIGHT, CB, 0, 78, 78, 5000000, 30000, 10000000, 2, 1);
        AddSimple(NFO, "Morgan", "Gibbs-White", "M. Gibbs-White", 24, "ENG", 171, 68, RIGHT, CAM, 108, 78, 84, 30000000, 40000, 60000000, 3, 1);
        AddSimple(NFO, "Taiwo", "Awoniyi", "T. Awoniyi", 26, "NGA", 183, 80, RIGHT, ST, 0, 78, 81, 25000000, 35000, 50000000, 4, 1);
        AddSimple(NFO, "Callum", "Hudson-Odoi", "C. Hudson-Odoi", 23, "ENG", 182, 73, RIGHT, LW, 108, 75, 81, 15000000, 30000, 30000000, 5, 1);
        AddSimple(NFO, "Anthony", "Elanga", "A. Elanga", 22, "SWE", 178, 70, RIGHT, RW, 0, 75, 82, 18000000, 25000, 36000000, 5, 1);
        AddSimple(NFO, "Ryan", "Yates", "R. Yates", 26, "ENG", 186, 78, RIGHT, CM, 0, 75, 77, 10000000, 25000, 20000000, 5, 1);
        AddSimple(NFO, "Ibrahim", "Sangaré", "I. Sangaré", 26, "MLI", 191, 83, RIGHT, CDM, 0, 77, 79, 15000000, 30000, 30000000, 5, 1);
        AddSimple(NFO, "Nuno", "Tavares", "N. Tavares", 24, "POR", 183, 75, LEFT, LB, 103, 73, 80, 10000000, 20000, 20000000, 5, 1);
        AddSimple(NFO, "Serge", "Aurier", "S. Aurier", 31, "CIV", 176, 74, RIGHT, RB, 0, 76, 76, 3000000, 25000, 6000000, 5, 1);
        AddSimple(NFO, "Murillo", "Santiago", "Murillo", 21, "BRA", 184, 76, LEFT, CB, 0, 75, 85, 25000000, 15000, 50000000, 5, 1);
        AddSimple(NFO, "Andrew", "Omobamidele", "A. Omobamidele", 21, "IRL", 188, 80, RIGHT, CB, 0, 72, 81, 8000000, 12000, 16000000, 5, 1);
        AddSimple(NFO, "Danilo", "Oliveira", "Danilo", 23, "BRA", 177, 68, RIGHT, CDM, 105, 75, 82, 20000000, 20000, 40000000, 5, 1);
        AddSimple(NFO, "Nicolás", "Domínguez", "N. Domínguez", 25, "ARG", 181, 76, RIGHT, CM, 0, 75, 79, 12000000, 22000, 24000000, 5, 1);
        AddSimple(NFO, "Divock", "Origi", "D. Origi", 29, "BEL", 185, 79, RIGHT, ST, 107, 73, 73, 4000000, 25000, 8000000, 5, 1);
        AddSimple(NFO, "Chris", "Wood", "C. Wood", 32, "NZL", 191, 85, RIGHT, ST, 0, 75, 75, 3000000, 28000, 6000000, 5, 1);
        AddSimple(NFO, "Ola", "Aina", "O. Aina", 27, "NGA", 184, 78, RIGHT, RB, 102, 74, 76, 6000000, 20000, 12000000, 5, 1);
        AddSimple(NFO, "Felipe", "Augusto", "Felipe", 35, "BRA", 190, 83, RIGHT, CB, 0, 76, 76, 2000000, 25000, 4000000, 5, 1);
        AddSimple(NFO, "Harry", "Toffolo", "H. Toffolo", 28, "ENG", 183, 74, LEFT, LB, 0, 72, 72, 2000000, 15000, 4000000, 5, 1);
        AddSimple(NFO, "Moussa", "Niakhaté", "M. Niakhaté", 27, "SEN", 190, 82, LEFT, CB, 102, 76, 77, 10000000, 22000, 20000000, 5, 1);
        AddSimple(NFO, "Emmanuel", "Dennis", "E. Dennis", 26, "NGA", 174, 70, RIGHT, LW, 109, 72, 74, 3000000, 18000, 6000000, 5, 1);
        AddSimple(NFO, "Wayne", "Hennessey", "W. Hennessey", 37, "WAL", 198, 90, RIGHT, GK, 0, 72, 72, 500000, 10000, 1000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int RBS (TeamId=117)
        // ══════════════════════════════════════════════════════════════
        int RBS = 117;
        AddSimple(RBS, "Alexander", "Schlager", "A. Schlager", 27, "AUT", 184, 78, RIGHT, GK, 0, 75, 78, 6000000, 20000, 12000000, 1, 1);
        AddSimple(RBS, "Oumar", "Solet", "O. Solet", 24, "FRA", 192, 84, RIGHT, CB, 0, 74, 80, 8000000, 15000, 16000000, 2, 1);
        AddSimple(RBS, "Andreas", "Ulmer", "A. Ulmer", 38, "AUT", 175, 70, LEFT, LB, 0, 73, 73, 500000, 12000, 1000000, 3, 1);
        AddSimple(RBS, "Mads", "Kjærgaard", "M. Kjærgaard", 21, "DEN", 190, 78, RIGHT, CB, 0, 72, 82, 10000000, 12000, 20000000, 4, 1);
        AddSimple(RBS, "Karim", "Konaté", "K. Konaté", 20, "CIV", 178, 72, RIGHT, ST, 0, 74, 85, 15000000, 10000, 30000000, 5, 1);
        AddSimple(RBS, "Benjamin", "Šeško", "B. Šeško", 20, "SVN", 194, 82, RIGHT, ST, 0, 76, 86, 20000000, 12000, 40000000, 5, 1);
        AddSimple(RBS, "Oscar", "Gloukh", "O. Gloukh", 20, "ISR", 170, 65, RIGHT, CAM, 107, 73, 85, 15000000, 10000, 30000000, 5, 1);
        AddSimple(RBS, "Luka", "Sučić", "L. Sučić", 21, "CRO", 185, 76, RIGHT, CM, 0, 73, 83, 12000000, 12000, 24000000, 5, 1);
        AddSimple(RBS, "Nicolas", "Capaldo", "N. Capaldo", 25, "ARG", 180, 74, RIGHT, CDM, 105, 73, 76, 4000000, 15000, 8000000, 5, 1);
        AddSimple(RBS, "Maurits", "Kjærgaard", "M. Kjærgaard", 21, "DEN", 185, 75, RIGHT, LW, 108, 73, 82, 10000000, 12000, 20000000, 5, 1);
        AddSimple(RBS, "Strahinja", "Pavlović", "S. Pavlović", 23, "SRB", 194, 84, LEFT, CB, 0, 75, 82, 12000000, 15000, 24000000, 5, 1);
        AddSimple(RBS, "Roko", "Šimić", "R. Šimić", 20, "CRO", 190, 80, RIGHT, ST, 0, 68, 80, 5000000, 8000, 10000000, 5, 1);
        AddSimple(RBS, "Samson", "Tijani", "S. Tijani", 21, "NGA", 178, 72, RIGHT, LW, 0, 70, 80, 6000000, 8000, 12000000, 5, 1);
        AddSimple(RBS, "Bryan", "Okoh", "B. Okoh", 21, "SUI", 186, 76, RIGHT, CB, 103, 69, 80, 4000000, 8000, 8000000, 5, 1);
        AddSimple(RBS, "Lucas", "Gourna-Douath", "L. Gourna-Douath", 20, "FRA", 186, 78, RIGHT, CDM, 0, 70, 82, 8000000, 10000, 16000000, 5, 1);
        AddSimple(RBS, "Dijon", "Kameri", "D. Kameri", 20, "AUT", 178, 72, RIGHT, CM, 0, 68, 80, 4000000, 8000, 8000000, 5, 1);
        AddSimple(RBS, "Sékou", "Koïta", "S. Koïta", 24, "MLI", 178, 74, RIGHT, ST, 107, 74, 76, 5000000, 15000, 10000000, 5, 1);
        AddSimple(RBS, "Noah", "Okafor", "N. Okafor", 24, "SUI", 185, 78, RIGHT, LW, 109, 76, 80, 12000000, 20000, 24000000, 5, 1);
        AddSimple(RBS, "Amar", "Dedić", "A. Dedić", 21, "BIH", 180, 72, LEFT, RB, 0, 71, 81, 8000000, 10000, 16000000, 5, 1);
        AddSimple(RBS, "Ignace", "Van der Brempt", "I. Van der Brempt", 22, "BEL", 186, 76, RIGHT, RB, 0, 70, 78, 3000000, 10000, 6000000, 5, 1);
        AddSimple(RBS, "Nico", "Mantl", "N. Mantl", 24, "GER", 189, 80, RIGHT, GK, 0, 70, 76, 2000000, 10000, 4000000, 5, 1);
        AddSimple(RBS, "Dennis", "Seimen", "D. Seimen", 19, "AUT", 189, 80, RIGHT, GK, 0, 66, 80, 3000000, 6000, 6000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int STU (TeamId=118)
        // ══════════════════════════════════════════════════════════════
        int STU = 118;
        AddSimple(STU, "Kjell", "Scherpen", "K. Scherpen", 24, "NED", 206, 88, RIGHT, GK, 0, 72, 80, 5000000, 12000, 10000000, 1, 1);
        AddSimple(STU, "Gregory", "Wüthrich", "G. Wüthrich", 29, "SUI", 188, 82, RIGHT, CB, 0, 73, 73, 2000000, 12000, 4000000, 2, 1);
        AddSimple(STU, "Jusuf", "Gazibegović", "J. Gazibegović", 24, "BIH", 174, 70, RIGHT, RB, 0, 72, 78, 4000000, 10000, 8000000, 3, 1);
        AddSimple(STU, "David", "Hirlander", "D. Hirlander", 23, "AUT", 182, 76, RIGHT, CB, 0, 69, 76, 2000000, 8000, 4000000, 4, 1);
        AddSimple(STU, "Alexander", "Prass", "A. Prass", 23, "AUT", 180, 72, LEFT, LB, 105, 72, 80, 6000000, 10000, 12000000, 5, 1);
        AddSimple(STU, "Otar", "Kiteishvili", "O. Kiteishvili", 28, "GEO", 172, 68, RIGHT, CAM, 105, 74, 74, 3000000, 15000, 6000000, 5, 1);
        AddSimple(STU, "Jon", "Gorenc Stanković", "J. Gorenc Stanković", 28, "SVN", 190, 82, RIGHT, CDM, 105, 72, 72, 2000000, 10000, 4000000, 5, 1);
        AddSimple(STU, "Mika", "Biereth", "M. Biereth", 21, "DEN", 188, 80, RIGHT, ST, 0, 71, 82, 8000000, 8000, 16000000, 5, 1);
        AddSimple(STU, "Tomislav", "Duvnjak", "T. Duvnjak", 22, "CRO", 180, 74, RIGHT, ST, 0, 70, 78, 3000000, 8000, 6000000, 5, 1);
        AddSimple(STU, "Tomi", "Horvat", "T. Horvat", 25, "SVN", 178, 72, LEFT, LW, 0, 72, 75, 3000000, 10000, 6000000, 5, 1);
        AddSimple(STU, "Stefan", "Hierländer", "S. Hierländer", 33, "AUT", 182, 76, RIGHT, CM, 108, 72, 72, 1000000, 12000, 2000000, 5, 1);
        AddSimple(STU, "Szymon", "Włodarczyk", "S. Włodarczyk", 21, "POL", 188, 80, RIGHT, ST, 0, 69, 80, 5000000, 8000, 10000000, 5, 1);
        AddSimple(STU, "Nicolas", "Bøgild", "N. Bøgild", 22, "DEN", 184, 76, RIGHT, CM, 0, 68, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(STU, "Wesley", "Gasparin", "W. Gasparin", 23, "BRA", 182, 74, RIGHT, CM, 0, 69, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(STU, "Niklas", "Geyrhofer", "N. Geyrhofer", 24, "AUT", 190, 82, RIGHT, CB, 0, 68, 74, 1500000, 8000, 3000000, 5, 1);
        AddSimple(STU, "David", "Affengruber", "D. Affengruber", 23, "AUT", 186, 78, RIGHT, CB, 0, 70, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(STU, "Manojlović", "Jovan", "J. Manojlović", 22, "SRB", 178, 72, RIGHT, RW, 0, 68, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(STU, "Leon", "Grgić", "L. Grgić", 22, "AUT", 184, 76, RIGHT, CDM, 0, 68, 76, 1500000, 8000, 3000000, 5, 1);
        AddSimple(STU, "Ivan", "Ljubić", "I. Ljubić", 27, "AUT", 178, 72, RIGHT, LB, 102, 70, 70, 1500000, 10000, 3000000, 5, 1);
        AddSimple(STU, "Benedikt", "Laimer", "B. Laimer", 20, "AUT", 180, 72, RIGHT, CM, 0, 66, 78, 2000000, 6000, 4000000, 5, 1);
        AddSimple(STU, "Luka", "Krofl", "L. Krofl", 21, "AUT", 190, 80, RIGHT, GK, 0, 66, 76, 1500000, 6000, 3000000, 5, 1);
        AddSimple(STU, "Marlon", "Mustapha", "M. Mustapha", 23, "AUT", 185, 78, RIGHT, ST, 0, 68, 74, 2000000, 8000, 4000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int GNK (TeamId=119)
        // ══════════════════════════════════════════════════════════════
        int GNK = 119;
        AddSimple(GNK, "Davy", "Roef", "D. Roef", 30, "BEL", 186, 78, RIGHT, GK, 0, 72, 72, 1500000, 12000, 3000000, 1, 1);
        AddSimple(GNK, "Jordan", "Torunarigha", "J. Torunarigha", 26, "NGA", 191, 84, LEFT, CB, 0, 73, 75, 4000000, 15000, 8000000, 2, 1);
        AddSimple(GNK, "Hyun-seok", "Ito", "H. Ito", 26, "KOR", 184, 76, RIGHT, CM, 105, 74, 77, 5000000, 15000, 10000000, 3, 1);
        AddSimple(GNK, "Julien", "De Sart", "J. De Sart", 29, "BEL", 185, 78, RIGHT, CDM, 0, 73, 73, 2500000, 12000, 5000000, 4, 1);
        AddSimple(GNK, "Gift", "Orban", "G. Orban", 21, "NGA", 180, 74, RIGHT, ST, 0, 75, 84, 15000000, 12000, 30000000, 5, 1);
        AddSimple(GNK, "Hugo", "Cuypers", "H. Cuypers", 27, "BEL", 184, 76, RIGHT, ST, 0, 74, 75, 5000000, 15000, 10000000, 5, 1);
        AddSimple(GNK, "Malick", "Fofana", "M. Fofana", 20, "CIV", 182, 74, RIGHT, ST, 107, 68, 82, 8000000, 8000, 16000000, 5, 1);
        AddSimple(GNK, "Noah", "Samoise", "N. Samoise", 25, "BEL", 176, 70, RIGHT, RB, 0, 72, 75, 3000000, 12000, 6000000, 5, 1);
        AddSimple(GNK, "Alieu", "Fadiga", "A. Fadiga", 22, "SEN", 178, 72, RIGHT, RW, 0, 70, 78, 3000000, 8000, 6000000, 5, 1);
        AddSimple(GNK, "Matisse", "Samois", "M. Samois", 23, "BEL", 182, 74, RIGHT, LB, 0, 70, 76, 2000000, 10000, 4000000, 5, 1);
        AddSimple(GNK, "Andri", "Guðjohnsen", "A. Guðjohnsen", 22, "ISL", 186, 78, RIGHT, ST, 0, 70, 80, 5000000, 8000, 10000000, 5, 1);
        AddSimple(GNK, "Sven", "Kums", "S. Kums", 36, "BEL", 183, 76, RIGHT, CAM, 105, 73, 73, 500000, 12000, 1000000, 5, 1);
        AddSimple(GNK, "Bram", "Lagae", "B. Lagae", 20, "BEL", 186, 78, RIGHT, CB, 0, 68, 78, 2000000, 6000, 4000000, 5, 1);
        AddSimple(GNK, "Ismaël", "Candoussier", "I. Candoussier", 21, "FRA", 178, 72, RIGHT, CM, 0, 69, 78, 2000000, 8000, 4000000, 5, 1);
        AddSimple(GNK, "Pieter", "Gerkens", "P. Gerkens", 29, "BEL", 182, 74, RIGHT, CM, 0, 72, 72, 2000000, 12000, 4000000, 5, 1);
        AddSimple(GNK, "Tarik", "Tissoudali", "T. Tissoudali", 31, "MAR", 176, 70, RIGHT, CAM, 108, 74, 74, 2000000, 15000, 4000000, 5, 1);
        AddSimple(GNK, "Dilane", "Bakwa", "D. Bakwa", 21, "BEL", 180, 72, RIGHT, RW, 0, 70, 80, 6000000, 8000, 12000000, 5, 1);
        AddSimple(GNK, "Louis", "Fortin", "L. Fortin", 22, "BEL", 184, 76, RIGHT, GK, 0, 68, 76, 1500000, 6000, 3000000, 5, 1);
        AddSimple(GNK, "Kamil", "Piątkowski", "K. Piątkowski", 23, "POL", 188, 80, RIGHT, CB, 0, 70, 77, 3000000, 8000, 6000000, 5, 1);
        AddSimple(GNK, "Mamadou", "Sylla", "M. Sylla", 22, "GUI", 176, 70, LEFT, LB, 0, 69, 77, 2000000, 8000, 4000000, 5, 1);
        AddSimple(GNK, "Yann", "Gboho", "Y. Gboho", 23, "FRA", 178, 72, RIGHT, LW, 0, 70, 77, 3000000, 8000, 6000000, 5, 1);
        AddSimple(GNK, "Célestin", "De Poorter", "C. De Poorter", 22, "BEL", 186, 78, RIGHT, CB, 0, 66, 76, 1500000, 6000, 3000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int LUD (TeamId=120)
        // ══════════════════════════════════════════════════════════════
        int LUD = 120;
        AddSimple(LUD, "Sergio", "Padt", "S. Padt", 33, "NED", 195, 88, RIGHT, GK, 0, 73, 73, 1000000, 15000, 2000000, 1, 1);
        AddSimple(LUD, "Olivier", "Verdon", "O. Verdon", 28, "BEN", 190, 82, RIGHT, CB, 0, 71, 72, 1500000, 10000, 3000000, 2, 1);
        AddSimple(LUD, "Anton", "Nedyalkov", "A. Nedyalkov", 31, "BUL", 182, 76, LEFT, LB, 0, 71, 71, 1000000, 10000, 2000000, 3, 1);
        AddSimple(LUD, "Jakub", "Piotrowski", "J. Piotrowski", 26, "POL", 185, 78, RIGHT, CM, 0, 72, 74, 3000000, 12000, 6000000, 4, 1);
        AddSimple(LUD, "Nonato", "Henrique", "Nonato", 26, "BRA", 180, 74, RIGHT, CDM, 105, 71, 73, 2000000, 10000, 4000000, 5, 1);
        AddSimple(LUD, "Bernard", "Tekpetey", "B. Tekpetey", 26, "GHA", 172, 70, RIGHT, RW, 107, 72, 74, 3000000, 12000, 6000000, 5, 1);
        AddSimple(LUD, "Rwan", "Santos", "Rwan", 23, "BRA", 184, 76, RIGHT, ST, 0, 70, 78, 4000000, 10000, 8000000, 5, 1);
        AddSimple(LUD, "Rick", "Lima", "Rick", 24, "BRA", 176, 72, RIGHT, LW, 0, 70, 76, 3000000, 10000, 6000000, 5, 1);
        AddSimple(LUD, "Caio", "Vidal", "Caio Vidal", 23, "BRA", 178, 72, RIGHT, ST, 108, 69, 78, 3000000, 8000, 6000000, 5, 1);
        AddSimple(LUD, "Igor", "Thiago", "I. Thiago", 24, "BRA", 186, 78, RIGHT, CB, 0, 70, 76, 2500000, 10000, 5000000, 5, 1);
        AddSimple(LUD, "Aslak", "Witry", "A. Witry", 28, "NOR", 180, 74, RIGHT, RB, 0, 70, 70, 1500000, 10000, 3000000, 5, 1);
        AddSimple(LUD, "Raúl", "Russo", "R. Russo", 26, "ESP", 188, 80, RIGHT, GK, 0, 70, 72, 1500000, 10000, 3000000, 5, 1);
        AddSimple(LUD, "Son", "Kwon", "S. Kwon", 26, "KOR", 178, 72, RIGHT, CM, 0, 70, 72, 2000000, 10000, 4000000, 5, 1);
        AddSimple(LUD, "Ruan", "Santos", "Ruan", 24, "BRA", 184, 76, RIGHT, CB, 0, 70, 76, 2000000, 10000, 4000000, 5, 1);
        AddSimple(LUD, "Claudio", "Caetano", "C. Caetano", 24, "BRA", 178, 72, RIGHT, LW, 0, 69, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(LUD, "Caio", "Lopes", "C. Lopes", 23, "BRA", 182, 74, RIGHT, RB, 0, 68, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(LUD, "Romuald", "Ntsogo", "R. Ntsogo", 22, "CMR", 184, 76, RIGHT, CDM, 0, 68, 76, 1500000, 8000, 3000000, 5, 1);
        AddSimple(LUD, "Franco", "Russo", "F. Russo", 23, "ARG", 180, 74, RIGHT, ST, 0, 68, 76, 2000000, 8000, 4000000, 5, 1);
        AddSimple(LUD, "Dinis", "Pinto", "D. Pinto", 22, "POR", 176, 70, LEFT, LB, 0, 67, 76, 1500000, 6000, 3000000, 5, 1);
        AddSimple(LUD, "Lukas", "Petz", "L. Petz", 21, "GER", 190, 82, RIGHT, GK, 0, 66, 76, 1500000, 6000, 3000000, 5, 1);
        AddSimple(LUD, "Deroy", "Duarte", "D. Duarte", 24, "CPV", 182, 76, RIGHT, CM, 107, 69, 74, 1500000, 8000, 3000000, 5, 1);
        AddSimple(LUD, "Marcel", "Heister", "M. Heister", 29, "GER", 180, 74, RIGHT, LB, 0, 70, 70, 1000000, 10000, 2000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int DZG (TeamId=121)
        // ══════════════════════════════════════════════════════════════
        int DZG = 121;
        AddSimple(DZG, "Dominik", "Livaković", "D. Livaković", 29, "CRO", 188, 82, RIGHT, GK, 0, 80, 82, 15000000, 30000, 30000000, 1, 1);
        AddSimple(DZG, "Stefan", "Ristovski", "S. Ristovski", 32, "MKD", 188, 80, RIGHT, RB, 0, 73, 73, 1500000, 15000, 3000000, 2, 1);
        AddSimple(DZG, "Josip", "Šutalo", "J. Šutalo", 24, "CRO", 188, 80, RIGHT, CB, 0, 76, 82, 15000000, 18000, 30000000, 3, 1);
        AddSimple(DZG, "Arijan", "Ademi", "A. Ademi", 33, "MKD", 183, 76, RIGHT, CDM, 105, 74, 74, 2000000, 20000, 4000000, 4, 1);
        AddSimple(DZG, "Luka", "Ivanušec", "L. Ivanušec", 25, "CRO", 176, 68, RIGHT, LW, 105, 77, 81, 12000000, 20000, 24000000, 5, 1);
        AddSimple(DZG, "Bruno", "Petković", "B. Petković", 29, "CRO", 193, 86, RIGHT, ST, 0, 76, 76, 6000000, 25000, 12000000, 5, 1);
        AddSimple(DZG, "Martin", "Baturina", "M. Baturina", 21, "CRO", 180, 72, RIGHT, CAM, 105, 76, 86, 20000000, 15000, 40000000, 5, 1);
        AddSimple(DZG, "Josip", "Stojković", "J. Stojković", 24, "CRO", 183, 76, RIGHT, RB, 103, 71, 78, 4000000, 12000, 8000000, 5, 1);
        AddSimple(DZG, "Mislav", "Oršić", "M. Oršić", 31, "CRO", 176, 70, RIGHT, LW, 109, 76, 76, 4000000, 25000, 8000000, 5, 1);
        AddSimple(DZG, "Dario", "Špikić", "D. Špikić", 25, "CRO", 180, 74, RIGHT, RW, 0, 72, 75, 3000000, 15000, 6000000, 5, 1);
        AddSimple(DZG, "Sadegh", "Moharrami", "S. Moharrami", 28, "IRN", 178, 74, RIGHT, RB, 0, 72, 72, 1500000, 12000, 3000000, 5, 1);
        AddSimple(DZG, "Rasmus", "Lauritsen", "R. Lauritsen", 28, "DEN", 192, 84, RIGHT, CB, 0, 73, 73, 2000000, 15000, 4000000, 5, 1);
        AddSimple(DZG, "Marko", "Livaja", "M. Livaja", 30, "CRO", 186, 80, RIGHT, ST, 0, 75, 75, 5000000, 22000, 10000000, 5, 1);
        AddSimple(DZG, "Bojan", "Knežević", "B. Knežević", 25, "CRO", 180, 74, RIGHT, LB, 0, 72, 75, 3000000, 12000, 6000000, 5, 1);
        AddSimple(DZG, "Ronald", "Spielmann", "R. Spielmann", 22, "AUT", 184, 76, RIGHT, CM, 0, 70, 80, 5000000, 10000, 10000000, 5, 1);
        AddSimple(DZG, "Toni", "Fruk", "T. Fruk", 23, "CRO", 180, 74, RIGHT, CM, 0, 70, 78, 3000000, 10000, 6000000, 5, 1);
        AddSimple(DZG, "Fran", "Topić", "F. Topić", 22, "CRO", 182, 74, RIGHT, ST, 0, 69, 79, 3000000, 8000, 6000000, 5, 1);
        AddSimple(DZG, "Mahir", "Emreli", "M. Emreli", 26, "AZE", 184, 76, RIGHT, ST, 107, 72, 73, 2000000, 12000, 4000000, 5, 1);
        AddSimple(DZG, "Marko", "Bulat", "M. Bulat", 22, "CRO", 182, 74, RIGHT, CDM, 0, 70, 80, 4000000, 10000, 8000000, 5, 1);
        AddSimple(DZG, "Lukas", "Kačavenda", "L. Kačavenda", 21, "CRO", 178, 72, RIGHT, CAM, 0, 68, 80, 3000000, 8000, 6000000, 5, 1);
        AddSimple(DZG, "Mauro", "Perković", "M. Perković", 20, "CRO", 184, 76, RIGHT, CB, 0, 66, 78, 2000000, 6000, 4000000, 5, 1);
        AddSimple(DZG, "Ivan", "Nevistić", "I. Nevistić", 25, "CRO", 188, 80, RIGHT, GK, 0, 70, 74, 1500000, 10000, 3000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Team int PLZ (TeamId=122)
        // ══════════════════════════════════════════════════════════════
        int PLZ = 122;
        AddSimple(PLZ, "Jindřich", "Staněk", "J. Staněk", 28, "CZE", 192, 84, RIGHT, GK, 0, 73, 75, 3000000, 15000, 6000000, 1, 1);
        AddSimple(PLZ, "Milan", "Havel", "M. Havel", 29, "CZE", 177, 72, RIGHT, LB, 103, 72, 72, 1500000, 12000, 3000000, 2, 1);
        AddSimple(PLZ, "Lukáš", "Hejda", "L. Hejda", 34, "CZE", 190, 82, RIGHT, CB, 0, 73, 73, 800000, 12000, 1600000, 3, 1);
        AddSimple(PLZ, "Lukáš", "Kalvach", "L. Kalvach", 28, "CZE", 183, 76, RIGHT, CM, 104, 74, 75, 4000000, 15000, 8000000, 4, 1);
        AddSimple(PLZ, "Matěj", "Vydra", "M. Vydra", 32, "CZE", 178, 72, RIGHT, ST, 108, 73, 73, 2000000, 18000, 4000000, 5, 1);
        AddSimple(PLZ, "Václav", "Jurečka", "V. Jurečka", 29, "CZE", 182, 76, RIGHT, ST, 108, 73, 73, 2000000, 15000, 4000000, 5, 1);
        AddSimple(PLZ, "Pavel", "Bucha", "P. Bucha", 25, "CZE", 182, 74, RIGHT, CAM, 105, 73, 77, 5000000, 15000, 10000000, 5, 1);
        AddSimple(PLZ, "Jhon", "Mosquera", "J. Mosquera", 33, "COL", 180, 74, RIGHT, CDM, 0, 72, 72, 1500000, 12000, 3000000, 5, 1);
        AddSimple(PLZ, "Jan", "Kliment", "J. Kliment", 30, "CZE", 185, 78, RIGHT, ST, 0, 71, 71, 1500000, 12000, 3000000, 5, 1);
        AddSimple(PLZ, "Jan", "Sýkora", "J. Sýkora", 30, "CZE", 176, 70, RIGHT, RW, 0, 72, 72, 1500000, 12000, 3000000, 5, 1);
        AddSimple(PLZ, "Erik", "Jirka", "E. Jirka", 26, "SVK", 178, 72, RIGHT, LW, 108, 71, 73, 2000000, 10000, 4000000, 5, 1);
        AddSimple(PLZ, "Radim", "Řezník", "R. Řezník", 35, "CZE", 184, 78, RIGHT, RB, 0, 71, 71, 500000, 10000, 1000000, 5, 1);
        AddSimple(PLZ, "Václav", "Černý", "V. Černý", 26, "CZE", 176, 70, RIGHT, RW, 107, 73, 76, 4000000, 15000, 8000000, 5, 1);
        AddSimple(PLZ, "Robin", "Hranáč", "R. Hranáč", 24, "CZE", 186, 78, RIGHT, CB, 0, 72, 78, 4000000, 10000, 8000000, 5, 1);
        AddSimple(PLZ, "Lukáš", "Červ", "L. Červ", 23, "CZE", 180, 74, RIGHT, CM, 0, 70, 78, 3000000, 10000, 6000000, 5, 1);
        AddSimple(PLZ, "Adam", "Vlkanova", "A. Vlkanova", 29, "CZE", 182, 76, RIGHT, CM, 0, 72, 72, 1500000, 12000, 3000000, 5, 1);
        AddSimple(PLZ, "Vojtěch", "Kubista", "V. Kubista", 28, "CZE", 186, 78, RIGHT, CB, 0, 71, 71, 1000000, 10000, 2000000, 5, 1);
        AddSimple(PLZ, "Marián", "Tvrdoň", "M. Tvrdoň", 28, "SVK", 178, 72, LEFT, LB, 0, 70, 70, 1000000, 10000, 2000000, 5, 1);
        AddSimple(PLZ, "František", "Mergac", "F. Mergac", 22, "CZE", 184, 76, RIGHT, CM, 0, 68, 78, 2000000, 8000, 4000000, 5, 1);
        AddSimple(PLZ, "Filip", "Čihák", "F. Čihák", 22, "CZE", 186, 78, RIGHT, CB, 0, 67, 77, 1500000, 8000, 3000000, 5, 1);
        AddSimple(PLZ, "Daniel", "Vasilenko", "D. Vasilenko", 20, "CZE", 180, 74, RIGHT, GK, 0, 66, 78, 1500000, 6000, 3000000, 5, 1);
        AddSimple(PLZ, "Tomáš", "Slavík", "T. Slavík", 22, "CZE", 182, 74, RIGHT, ST, 0, 67, 77, 1500000, 8000, 3000000, 5, 1);

        // ══════════════════════════════════════════════════════════════
        // Middlesbrough (TeamId=123)
        // ══════════════════════════════════════════════════════════════
        int MID = 123;
        AddSimple(MID, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(MID, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(MID, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(MID, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(MID, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(MID, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(MID, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(MID, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(MID, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(MID, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(MID, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(MID, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(MID, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(MID, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(MID, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(MID, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(MID, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(MID, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(MID, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(MID, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(MID, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(MID, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Lille OSC (TeamId=124)
        // ══════════════════════════════════════════════════════════════
        int LIL = 124;
        AddSimple(LIL, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(LIL, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(LIL, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(LIL, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(LIL, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(LIL, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(LIL, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(LIL, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(LIL, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(LIL, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(LIL, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(LIL, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(LIL, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(LIL, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(LIL, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(LIL, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(LIL, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(LIL, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(LIL, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(LIL, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(LIL, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(LIL, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Olympique Lyonnais (TeamId=125)
        // ══════════════════════════════════════════════════════════════
        int LYO = 125;
        AddSimple(LYO, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(LYO, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(LYO, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(LYO, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(LYO, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(LYO, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(LYO, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(LYO, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(LYO, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(LYO, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(LYO, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(LYO, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(LYO, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(LYO, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(LYO, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(LYO, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(LYO, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(LYO, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(LYO, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(LYO, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(LYO, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(LYO, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // OGC Nice (TeamId=126)
        // ══════════════════════════════════════════════════════════════
        int NIC = 126;
        AddSimple(NIC, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(NIC, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(NIC, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(NIC, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(NIC, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(NIC, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(NIC, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(NIC, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(NIC, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(NIC, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(NIC, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(NIC, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(NIC, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(NIC, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(NIC, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(NIC, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(NIC, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(NIC, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(NIC, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(NIC, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(NIC, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(NIC, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // VfB Stuttgart (TeamId=127)
        // ══════════════════════════════════════════════════════════════
        int VFB = 127;
        AddSimple(VFB, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(VFB, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(VFB, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(VFB, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(VFB, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(VFB, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(VFB, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(VFB, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(VFB, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(VFB, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(VFB, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(VFB, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(VFB, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(VFB, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(VFB, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(VFB, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(VFB, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(VFB, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(VFB, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(VFB, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(VFB, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(VFB, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // SC Freiburg (TeamId=128)
        // ══════════════════════════════════════════════════════════════
        int SCF = 128;
        AddSimple(SCF, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(SCF, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(SCF, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(SCF, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(SCF, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(SCF, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(SCF, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(SCF, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(SCF, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(SCF, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(SCF, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(SCF, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(SCF, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(SCF, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(SCF, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(SCF, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(SCF, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(SCF, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(SCF, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(SCF, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(SCF, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(SCF, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Panathinaikos (TeamId=129)
        // ══════════════════════════════════════════════════════════════
        int PAN = 129;
        AddSimple(PAN, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(PAN, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(PAN, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(PAN, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(PAN, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(PAN, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(PAN, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(PAN, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(PAN, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(PAN, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(PAN, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(PAN, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(PAN, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(PAN, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(PAN, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(PAN, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(PAN, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(PAN, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(PAN, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(PAN, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(PAN, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(PAN, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // PAOK Thessaloniki (TeamId=130)
        // ══════════════════════════════════════════════════════════════
        int PAK = 130;
        AddSimple(PAK, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(PAK, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(PAK, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(PAK, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(PAK, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(PAK, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(PAK, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(PAK, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(PAK, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(PAK, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(PAK, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(PAK, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(PAK, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(PAK, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(PAK, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(PAK, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(PAK, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(PAK, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(PAK, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(PAK, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(PAK, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(PAK, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Ferencváros (TeamId=131)
        // ══════════════════════════════════════════════════════════════
        int FTC = 131;
        AddSimple(FTC, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(FTC, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(FTC, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(FTC, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(FTC, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(FTC, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(FTC, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(FTC, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(FTC, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(FTC, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(FTC, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(FTC, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(FTC, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(FTC, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(FTC, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(FTC, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(FTC, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(FTC, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(FTC, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(FTC, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(FTC, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(FTC, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Maccabi Tel Aviv (TeamId=132)
        // ══════════════════════════════════════════════════════════════
        int MTA = 132;
        AddSimple(MTA, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(MTA, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(MTA, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(MTA, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(MTA, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(MTA, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(MTA, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(MTA, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(MTA, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(MTA, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(MTA, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(MTA, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(MTA, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(MTA, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(MTA, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(MTA, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(MTA, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(MTA, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(MTA, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(MTA, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(MTA, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(MTA, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Bolton Wanderers (TeamId=133)
        // ══════════════════════════════════════════════════════════════
        int BOL = 133;
        AddSimple(BOL, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(BOL, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(BOL, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(BOL, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(BOL, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(BOL, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(BOL, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(BOL, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(BOL, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(BOL, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(BOL, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(BOL, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(BOL, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(BOL, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(BOL, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(BOL, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(BOL, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(BOL, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(BOL, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(BOL, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(BOL, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(BOL, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // AS Roma (TeamId=134)
        // ══════════════════════════════════════════════════════════════
        int ROM = 134;
        AddSimple(ROM, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(ROM, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(ROM, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(ROM, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(ROM, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(ROM, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(ROM, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(ROM, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(ROM, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(ROM, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(ROM, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(ROM, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(ROM, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(ROM, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(ROM, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(ROM, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(ROM, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(ROM, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(ROM, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(ROM, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(ROM, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(ROM, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Feyenoord (TeamId=135)
        // ══════════════════════════════════════════════════════════════
        int FEY = 135;
        AddSimple(FEY, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(FEY, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(FEY, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(FEY, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(FEY, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(FEY, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(FEY, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(FEY, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(FEY, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(FEY, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(FEY, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(FEY, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(FEY, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(FEY, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(FEY, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(FEY, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(FEY, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(FEY, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(FEY, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(FEY, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(FEY, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(FEY, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // GAE (TeamId=136)
        // ══════════════════════════════════════════════════════════════
        int GAE = 136;
        AddSimple(GAE, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(GAE, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(GAE, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(GAE, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(GAE, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(GAE, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(GAE, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(GAE, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(GAE, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(GAE, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(GAE, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(GAE, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(GAE, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(GAE, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(GAE, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(GAE, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(GAE, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(GAE, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(GAE, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(GAE, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(GAE, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(GAE, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Utrecht (TeamId=137)
        // ══════════════════════════════════════════════════════════════
        int UTR = 137;
        AddSimple(UTR, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(UTR, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(UTR, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(UTR, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(UTR, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(UTR, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(UTR, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(UTR, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(UTR, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(UTR, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(UTR, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(UTR, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(UTR, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(UTR, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(UTR, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(UTR, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(UTR, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(UTR, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(UTR, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(UTR, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(UTR, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(UTR, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Braga (TeamId=138)
        // ══════════════════════════════════════════════════════════════
        int BRA = 138;
        AddSimple(BRA, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(BRA, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(BRA, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(BRA, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(BRA, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(BRA, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(BRA, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(BRA, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(BRA, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(BRA, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(BRA, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(BRA, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(BRA, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(BRA, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(BRA, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(BRA, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(BRA, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(BRA, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(BRA, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(BRA, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(BRA, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(BRA, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Club Brugge (TeamId=139)
        // ══════════════════════════════════════════════════════════════
        int BRG = 139;
        AddSimple(BRG, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(BRG, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(BRG, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(BRG, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(BRG, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(BRG, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(BRG, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(BRG, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(BRG, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(BRG, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(BRG, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(BRG, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(BRG, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(BRG, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(BRG, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(BRG, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(BRG, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(BRG, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(BRG, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(BRG, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(BRG, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(BRG, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // FC Porto (TeamId=140)
        // ══════════════════════════════════════════════════════════════
        int FCP = 140;
        AddSimple(FCP, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(FCP, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(FCP, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(FCP, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(FCP, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(FCP, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(FCP, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(FCP, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(FCP, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(FCP, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(FCP, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(FCP, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(FCP, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(FCP, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(FCP, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(FCP, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(FCP, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(FCP, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(FCP, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(FCP, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(FCP, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(FCP, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // CFC (TeamId=141)
        // ══════════════════════════════════════════════════════════════
        int CFC = 141;
        AddSimple(CFC, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(CFC, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(CFC, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(CFC, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(CFC, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(CFC, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(CFC, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(CFC, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(CFC, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(CFC, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(CFC, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(CFC, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(CFC, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(CFC, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(CFC, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(CFC, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(CFC, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(CFC, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(CFC, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(CFC, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(CFC, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(CFC, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Rangers (TeamId=142)
        // ══════════════════════════════════════════════════════════════
        int RAN = 142;
        AddSimple(RAN, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(RAN, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(RAN, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(RAN, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(RAN, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(RAN, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(RAN, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(RAN, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(RAN, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(RAN, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(RAN, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(RAN, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(RAN, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(RAN, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(RAN, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(RAN, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(RAN, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(RAN, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(RAN, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(RAN, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(RAN, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(RAN, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Red Star Belgrade (TeamId=143)
        // ══════════════════════════════════════════════════════════════
        int ZVE = 143;
        AddSimple(ZVE, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(ZVE, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(ZVE, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(ZVE, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(ZVE, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(ZVE, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(ZVE, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(ZVE, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(ZVE, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(ZVE, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(ZVE, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(ZVE, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(ZVE, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(ZVE, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(ZVE, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(ZVE, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(ZVE, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(ZVE, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(ZVE, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(ZVE, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(ZVE, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(ZVE, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Malmö FF (TeamId=144)
        // ══════════════════════════════════════════════════════════════
        int MFF = 144;
        AddSimple(MFF, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(MFF, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(MFF, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(MFF, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(MFF, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(MFF, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(MFF, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(MFF, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(MFF, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(MFF, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(MFF, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(MFF, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(MFF, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(MFF, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(MFF, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(MFF, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(MFF, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(MFF, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(MFF, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(MFF, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(MFF, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(MFF, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // FC Basel (TeamId=145)
        // ══════════════════════════════════════════════════════════════
        int BAS = 145;
        AddSimple(BAS, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(BAS, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(BAS, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(BAS, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(BAS, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(BAS, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(BAS, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(BAS, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(BAS, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(BAS, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(BAS, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(BAS, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(BAS, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(BAS, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(BAS, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(BAS, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(BAS, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(BAS, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(BAS, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(BAS, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(BAS, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(BAS, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // BSC Young Boys (TeamId=146)
        // ══════════════════════════════════════════════════════════════
        int YB = 146;
        AddSimple(YB, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(YB, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(YB, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(YB, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(YB, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(YB, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(YB, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(YB, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(YB, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(YB, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(YB, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(YB, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(YB, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(YB, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(YB, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(YB, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(YB, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(YB, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(YB, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(YB, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(YB, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(YB, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Fenerbahçe (TeamId=147)
        // ══════════════════════════════════════════════════════════════
        int FEN = 147;
        AddSimple(FEN, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(FEN, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(FEN, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(FEN, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(FEN, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(FEN, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(FEN, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(FEN, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(FEN, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(FEN, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(FEN, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(FEN, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(FEN, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(FEN, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(FEN, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(FEN, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(FEN, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(FEN, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(FEN, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(FEN, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(FEN, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(FEN, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // Crystal Palace (TeamId=148)
        // ══════════════════════════════════════════════════════════════
        int CRY = 148;
        AddSimple(CRY, "Alban", "Lafont", "Lafont", 26, "FRA", 190, 84, RIGHT, GK, 0, 75, 80, 8000000, 15000, 16000000, 4, 1);
        AddSimple(CRY, "Karl-Johan", "Johnsson", "Johnsson", 35, "SWE", 187, 82, RIGHT, GK, 0, 72, 72, 1000000, 8000, 2000000, 2, 30);
        AddSimple(CRY, "Jonathan", "Tah", "Tah", 29, "GER", 194, 90, RIGHT, CB, 0, 78, 80, 15000000, 25000, 30000000, 4, 4);
        AddSimple(CRY, "Lucas", "Hernandez", "Hernandez", 29, "FRA", 184, 79, 1, 1, 2, 80, 82, 25000000, 30000, 50000000, 3, 21);
        AddSimple(CRY, "Nico", "Schlotterbeck", "Schlotterbeck", 26, "GER", 191, 85, 1, 1, 0, 78, 82, 20000000, 25000, 40000000, 4, 5);
        AddSimple(CRY, "Javi", "Galán", "Galán", 30, "ESP", 172, 70, 1, 2, 0, 74, 76, 5000000, 15000, 10000000, 3, 17);
        AddSimple(CRY, "Noussair", "Mazraoui", "Mazraoui", 27, "MAR", 183, 77, 0, 3, 0, 76, 80, 15000000, 20000, 30000000, 4, 3);
        AddSimple(CRY, "Boubacar", "Kamara", "Kamara", 26, "FRA", 184, 76, 0, 4, 5, 78, 82, 20000000, 25000, 40000000, 4, 6);
        AddSimple(CRY, "Wilfred", "Ndidi", "Ndidi", 29, "NGA", 183, 78, 0, 4, 5, 77, 78, 12000000, 20000, 24000000, 3, 25);
        AddSimple(CRY, "Enzo", "Le Fée", "Le Fée", 25, "FRA", 178, 72, 0, 5, 6, 76, 80, 12000000, 20000, 24000000, 4, 8);
        AddSimple(CRY, "Marcel", "Sabitzer", "Sabitzer", 31, "AUT", 178, 75, 0, 5, 6, 78, 78, 10000000, 25000, 20000000, 2, 7);
        AddSimple(CRY, "Dani", "Olmo", "Olmo", 27, "ESP", 179, 73, 0, 6, 7, 80, 83, 30000000, 30000, 60000000, 3, 10);
        AddSimple(CRY, "Ludovic", "Blas", "Blas", 27, "FRA", 180, 74, 0, 6, 8, 75, 78, 15000000, 20000, 30000000, 4, 11);
        AddSimple(CRY, "Jonathan", "Clauss", "Clauss", 32, "FRA", 178, 74, 0, 7, 8, 76, 76, 8000000, 20000, 16000000, 2, 19);
        AddSimple(CRY, "Marcus", "Thuram", "Thuram", 27, "FRA", 192, 85, 0, 8, 7, 80, 82, 30000000, 30000, 60000000, 3, 9);
        AddSimple(CRY, "Rayan", "Cherki", "Cherki", 21, "FRA", 179, 72, 0, 6, 8, 76, 86, 25000000, 15000, 50000000, 5, 18);
        AddSimple(CRY, "Moses", "Simon", "Simon", 29, "NGA", 168, 65, 0, 8, 7, 76, 78, 10000000, 20000, 20000000, 3, 14);
        AddSimple(CRY, "Jonathan", "David", "David", 25, "CAN", 182, 78, 0, 9, 0, 80, 84, 40000000, 30000, 80000000, 4, 9);
        AddSimple(CRY, "Victor", "Boniface", "Boniface", 23, "NGA", 190, 86, 0, 9, 0, 78, 84, 25000000, 15000, 50000000, 5, 12);
        AddSimple(CRY, "Loïs", "Openda", "Openda", 24, "BEL", 178, 74, 0, 9, 8, 78, 82, 25000000, 20000, 50000000, 5, 11);
        AddSimple(CRY, "Hugo", "Ekitike", "Ekitike", 22, "FRA", 189, 82, 0, 9, 0, 74, 82, 15000000, 10000, 30000000, 5, 22);
        AddSimple(CRY, "Álvaro", "Morata", "Morata", 32, "ESP", 190, 84, 0, 9, 0, 80, 80, 15000000, 35000, 30000000, 2, 24);

        // ══════════════════════════════════════════════════════════════
        // CONFERENCE LEAGUE 2024/25 — Teams 149-183
        // ══════════════════════════════════════════════════════════════

        // ══════════════════════════════════════════════════════════════
        // FC Noah (TeamId=149)
        // ══════════════════════════════════════════════════════════════
        int NOA = 149;
        AddSimple(NOA, "Timothy", "Fayulu", "Fayulu", 26, "COD", 186, 80, RIGHT, GK, GK_f, 70, 74, 1200000, 2000, 2500000, 4, 1);
        AddSimple(NOA, "Aleksei", "Ploshadny", "Ploshadny", 22, "RUS", 190, 82, RIGHT, GK, GK_f, 60, 74, 400000, 500, 800000, 5, 13);
        AddSimple(NOA, "Ognen", "Cancarevic", "Cancarevic", 36, "MNE", 194, 88, RIGHT, GK, GK_f, 65, 65, 100000, 800, 200000, 1, 22);
        AddSimple(NOA, "Kim", "Tovmasyan", "Tovmasyan", 22, "ARM", 184, 78, RIGHT, CB, RB_f, 58, 72, 300000, 400, 600000, 5, 5);
        AddSimple(NOA, "Sergey", "Muradyan", "Muradyan", 26, "ARM", 188, 82, RIGHT, CB, 0, 66, 70, 600000, 1000, 1200000, 4, 4);
        AddSimple(NOA, "Goncalo", "Silva", "G. Silva", 34, "POR", 186, 80, RIGHT, CB, 0, 68, 68, 300000, 1500, 600000, 2, 44);
        AddSimple(NOA, "Nathaniel", "Sentini", "Sentini", 26, "FRA", 185, 78, RIGHT, CB, CDM_f, 64, 70, 500000, 800, 1000000, 4, 6);
        AddSimple(NOA, "Gudmundur", "Thorarinsson", "Thorarinsson", 33, "ISL", 178, 72, LEFT, LB, LM_f, 67, 67, 400000, 1500, 800000, 2, 3);
        AddSimple(NOA, "Hovhannes", "Hambardzumyan", "Hambardzumyan", 35, "ARM", 176, 72, RIGHT, RB, 0, 65, 65, 200000, 1200, 400000, 2, 2);
        AddSimple(NOA, "David", "Sualehe", "Sualehe", 28, "POR", 180, 74, LEFT, LB, LM_f, 66, 70, 500000, 1000, 1000000, 4, 15);
        AddSimple(NOA, "Eric", "Boakye", "Boakye", 26, "GHA", 178, 74, RIGHT, RB, RM_f, 65, 70, 500000, 800, 1000000, 4, 21);
        AddSimple(NOA, "Nermin", "Zolotic", "Zolotic", 32, "BIH", 190, 84, RIGHT, CB, 0, 69, 69, 400000, 1500, 800000, 2, 55);
        AddSimple(NOA, "Artak", "Dashyan", "Dashyan", 35, "ARM", 180, 76, RIGHT, CM, CAM_f, 64, 64, 200000, 1000, 400000, 2, 8);
        AddSimple(NOA, "Imran", "Oulad Omar", "Oulad Omar", 27, "NED", 178, 72, RIGHT, CM, CDM_f, 66, 70, 600000, 1200, 1200000, 4, 10);
        AddSimple(NOA, "Takuto", "Oshima", "Oshima", 27, "JPN", 172, 68, RIGHT, CDM, CM_f, 68, 72, 800000, 1500, 1600000, 4, 14);
        AddSimple(NOA, "Gustavo", "Sangare", "Sangare", 28, "BFA", 185, 80, RIGHT, CDM, CM_f, 65, 68, 500000, 1000, 1000000, 4, 17);
        AddSimple(NOA, "Yan", "Eteki", "Eteki", 28, "CMR", 182, 78, RIGHT, CDM, CM_f, 67, 70, 600000, 1200, 1200000, 4, 18);
        AddSimple(NOA, "Gor", "Manvelyan", "Manvelyan", 24, "ARM", 174, 70, RIGHT, CAM, CM_f, 68, 76, 1200000, 1500, 2500000, 5, 7);
        AddSimple(NOA, "Helder", "Ferreira", "H. Ferreira", 28, "POR", 175, 72, RIGHT, RW, LW_f, 67, 70, 700000, 1200, 1500000, 4, 11);
        AddSimple(NOA, "Virgil", "Pinson", "Pinson", 29, "FRA", 177, 72, RIGHT, LW, RW_f, 66, 68, 500000, 1000, 1000000, 3, 19);
        AddSimple(NOA, "Marin", "Jakolis", "Jakolis", 28, "CRO", 182, 76, RIGHT, LW, RW_f, 68, 70, 700000, 1200, 1500000, 4, 20);
        AddSimple(NOA, "Nardin", "Mulahusejnovic", "Mulahusejnovic", 27, "BIH", 185, 82, RIGHT, ST, CF_f, 65, 68, 600000, 1200, 1200000, 4, 9);

        // ══════════════════════════════════════════════════════════════
        // SK Rapid Wien (TeamId=150)
        // ══════════════════════════════════════════════════════════════
        int RPD = 150;
        AddSimple(RPD, "Niklas", "Hedl", "Hedl", 24, "AUT", 192, 84, RIGHT, GK, GK_f, 72, 80, 3000000, 4000, 6000000, 5, 1);
        AddSimple(RPD, "Paul", "Gartler", "Gartler", 28, "AUT", 186, 80, RIGHT, GK, GK_f, 66, 70, 800000, 2000, 1600000, 4, 25);
        AddSimple(RPD, "Laurenz", "Orgler", "Orgler", 22, "AUT", 188, 82, RIGHT, GK, GK_f, 60, 74, 400000, 600, 800000, 5, 36);
        AddSimple(RPD, "Benjamin", "Goschl", "Goschl", 20, "AUT", 186, 78, RIGHT, CB, RB_f, 62, 78, 800000, 600, 1600000, 5, 5);
        AddSimple(RPD, "Bendeguz", "Bolla", "Bolla", 25, "HUN", 180, 74, RIGHT, RB, RM_f, 72, 76, 3000000, 4000, 6000000, 5, 2);
        AddSimple(RPD, "Serge-Philippe", "Raux Yao", "Raux Yao", 26, "BEN", 186, 80, RIGHT, CB, 0, 68, 72, 1200000, 2500, 2500000, 4, 6);
        AddSimple(RPD, "Nenad", "Cvetkovic", "Cvetkovic", 29, "SRB", 188, 82, RIGHT, CB, 0, 70, 72, 1500000, 3000, 3000000, 4, 4);
        AddSimple(RPD, "Ange", "Ahoussou", "Ahoussou", 21, "CIV", 184, 78, RIGHT, CB, CDM_f, 64, 78, 1000000, 800, 2000000, 5, 7);
        AddSimple(RPD, "Jean", "Marcelin", "Marcelin", 25, "FRA", 185, 80, RIGHT, CB, 0, 67, 72, 1000000, 2000, 2000000, 5, 16);
        AddSimple(RPD, "Jannes", "Horn", "Horn", 27, "GER", 176, 72, LEFT, LB, LM_f, 70, 72, 2000000, 3500, 4000000, 4, 3);
        AddSimple(RPD, "Furkan", "Demir", "Demir", 20, "AUT", 182, 76, RIGHT, CB, RB_f, 58, 74, 500000, 400, 1000000, 5, 24);
        AddSimple(RPD, "Jakob", "Scholler", "Scholler", 20, "AUT", 184, 76, RIGHT, CB, 0, 58, 74, 400000, 400, 800000, 5, 33);
        AddSimple(RPD, "Jonas", "Auer", "Auer", 24, "AUT", 178, 72, LEFT, LB, LM_f, 68, 74, 1500000, 2500, 3000000, 5, 14);
        AddSimple(RPD, "Matthias", "Seidl", "Seidl", 24, "AUT", 180, 74, RIGHT, CM, CAM_f, 72, 78, 3000000, 4000, 6000000, 5, 8);
        AddSimple(RPD, "Tobias", "Gulliksen", "Gulliksen", 22, "NOR", 175, 72, RIGHT, RW, LW_f, 68, 78, 2000000, 2500, 4000000, 5, 11);
        AddSimple(RPD, "Romeo", "Amane", "Amane", 22, "CIV", 178, 72, RIGHT, CM, CDM_f, 66, 76, 1500000, 2000, 3000000, 5, 20);
        AddSimple(RPD, "Petter Nosakhare", "Dahl", "Dahl", 22, "NOR", 182, 76, RIGHT, CM, CAM_f, 65, 76, 1200000, 2000, 2500000, 5, 10);
        AddSimple(RPD, "Louis", "Schaub", "Schaub", 30, "AUT", 176, 72, LEFT, CAM, LM_f, 74, 74, 2500000, 5000, 5000000, 3, 21);
        AddSimple(RPD, "Martin", "Ndzie", "Ndzie", 20, "AUT", 184, 78, RIGHT, ST, RW_f, 60, 76, 600000, 600, 1200000, 5, 17);
        AddSimple(RPD, "Janis", "Antiste", "Antiste", 23, "FRA", 180, 76, RIGHT, ST, LW_f, 68, 76, 2000000, 3000, 4000000, 5, 9);
        AddSimple(RPD, "Ercan", "Kara", "Kara", 30, "AUT", 190, 84, RIGHT, ST, CF_f, 72, 72, 2000000, 5000, 4000000, 3, 13);
        AddSimple(RPD, "Andreas", "Weimann", "Weimann", 33, "AUT", 179, 74, RIGHT, ST, CAM_f, 74, 74, 2000000, 6000, 4000000, 2, 18);

        // ══════════════════════════════════════════════════════════════
        // HŠK Zrinjski Mostar (TeamId=151)
        // ══════════════════════════════════════════════════════════════
        int ZRI = 151;
        AddSimple(ZRI, "Ivan", "Nevjestić", "Nevjestić", 26, "BIH", 190, 84, RIGHT, GK, GK_f, 68, 72, 800000, 1500, 1600000, 4, 1);
        AddSimple(ZRI, "Marko", "Pišić", "Pišić", 28, "BIH", 186, 80, RIGHT, GK, GK_f, 65, 68, 400000, 1000, 800000, 3, 12);
        AddSimple(ZRI, "Ivan", "Bilić", "Bilić", 26, "BIH", 185, 78, RIGHT, CB, RB_f, 64, 68, 500000, 1000, 1000000, 4, 5);
        AddSimple(ZRI, "Ivan", "Šunjić", "Šunjić", 28, "BIH", 188, 82, RIGHT, CB, 0, 70, 72, 1500000, 2500, 3000000, 4, 4);
        AddSimple(ZRI, "Josip", "Kvesić", "Kvesić", 32, "CRO", 186, 80, LEFT, CB, 0, 68, 68, 500000, 2000, 1000000, 2, 44);
        AddSimple(ZRI, "Luka", "Menalo", "Menalo", 27, "BIH", 176, 72, RIGHT, RB, RM_f, 66, 70, 700000, 1500, 1400000, 4, 2);
        AddSimple(ZRI, "Amar", "Mulić", "Mulić", 24, "BIH", 184, 78, RIGHT, CB, 0, 65, 72, 800000, 1200, 1600000, 5, 6);
        AddSimple(ZRI, "Stipe", "Biuk", "Biuk", 22, "CRO", 176, 72, RIGHT, LW, RW_f, 68, 78, 2000000, 2000, 4000000, 5, 7);
        AddSimple(ZRI, "Filip", "Bradarić", "Bradarić", 32, "CRO", 186, 78, LEFT, CM, CDM_f, 72, 72, 1500000, 4000, 3000000, 2, 8);
        AddSimple(ZRI, "Franko", "Kovačević", "Kovačević", 25, "CRO", 178, 74, RIGHT, CM, CAM_f, 66, 72, 800000, 1500, 1600000, 5, 11);
        AddSimple(ZRI, "Amer", "Ordagić", "Ordagić", 32, "BIH", 180, 76, RIGHT, CDM, CM_f, 70, 70, 600000, 2500, 1200000, 2, 22);
        AddSimple(ZRI, "Nemanja", "Bilbija", "Bilbija", 35, "BIH", 182, 78, RIGHT, ST, CF_f, 72, 72, 1000000, 4000, 2000000, 2, 9);
        AddSimple(ZRI, "Dario", "Melnjak", "Melnjak", 32, "CRO", 178, 72, LEFT, LB, LM_f, 70, 70, 800000, 3000, 1600000, 2, 3);
        AddSimple(ZRI, "Marijan", "Čabraja", "Čabraja", 28, "CRO", 180, 74, LEFT, LB, LM_f, 67, 70, 600000, 1500, 1200000, 4, 17);
        AddSimple(ZRI, "Mato", "Miloš", "Miloš", 31, "CRO", 180, 74, RIGHT, RB, RM_f, 68, 68, 600000, 2000, 1200000, 3, 13);
        AddSimple(ZRI, "Dino", "Ćorić", "Ćorić", 23, "BIH", 178, 72, RIGHT, CAM, CM_f, 65, 76, 1200000, 1000, 2500000, 5, 10);
        AddSimple(ZRI, "Ivan", "Šaranović", "Šaranović", 30, "MNE", 186, 80, RIGHT, ST, CF_f, 68, 70, 800000, 2000, 1600000, 3, 19);
        AddSimple(ZRI, "Semir", "Štilić", "Štilić", 37, "BIH", 178, 72, LEFT, CAM, CM_f, 70, 70, 300000, 2500, 600000, 1, 20);
        AddSimple(ZRI, "Ante", "Ćorić", "Ćorić", 28, "CRO", 176, 70, RIGHT, CM, CAM_f, 70, 74, 1500000, 3000, 3000000, 4, 14);
        AddSimple(ZRI, "Elvedin", "Džinić", "Džinić", 26, "BIH", 186, 80, RIGHT, CB, 0, 66, 70, 600000, 1200, 1200000, 4, 15);
        AddSimple(ZRI, "Ivan", "Tomečak", "Tomečak", 35, "CRO", 178, 72, RIGHT, RB, RM_f, 66, 66, 300000, 1500, 600000, 2, 16);
        AddSimple(ZRI, "Đorđe", "Ivanović", "Ivanović", 30, "SRB", 185, 80, RIGHT, ST, LW_f, 68, 70, 800000, 2000, 1600000, 3, 18);

        // ══════════════════════════════════════════════════════════════
        // HNK Rijeka (TeamId=152)
        // ══════════════════════════════════════════════════════════════
        int RIJ = 152;
        AddSimple(RIJ, "Andrej", "Prskalo", "Prskalo", 30, "CRO", 190, 84, RIGHT, GK, GK_f, 67, 70, 600000, 1500, 1200000, 4, 1);
        AddSimple(RIJ, "Simon", "Sluga", "Sluga", 32, "CRO", 192, 86, RIGHT, GK, GK_f, 72, 74, 1500000, 4000, 3000000, 3, 12);
        AddSimple(RIJ, "Marko", "Pjaca", "Pjaca", 30, "CRO", 186, 78, RIGHT, LW, RW_f, 74, 76, 4000000, 6000, 8000000, 3, 11);
        AddSimple(RIJ, "Mate", "Maleš", "Maleš", 35, "CRO", 180, 76, RIGHT, CM, CDM_f, 68, 68, 300000, 2000, 600000, 2, 8);
        AddSimple(RIJ, "Marco", "Šćepanović", "Šćepanović", 25, "MNE", 184, 78, RIGHT, ST, CF_f, 66, 72, 800000, 1500, 1600000, 5, 9);
        AddSimple(RIJ, "Ivan", "Tomečak", "Tomečak", 35, "CRO", 178, 72, RIGHT, RB, RM_f, 66, 66, 300000, 1500, 600000, 2, 16);
        AddSimple(RIJ, "Stipe", "Jurić", "Jurić", 26, "CRO", 186, 80, RIGHT, CB, RB_f, 68, 72, 1000000, 2000, 2000000, 4, 4);
        AddSimple(RIJ, "Nino", "Galović", "Galović", 32, "CRO", 190, 84, LEFT, CB, 0, 70, 70, 800000, 3000, 1600000, 3, 5);
        AddSimple(RIJ, "Mario", "Čabrić", "Čabrić", 28, "CRO", 176, 72, RIGHT, LW, RW_f, 68, 72, 1000000, 2000, 2000000, 4, 14);
        AddSimple(RIJ, "Ivan", "Đurđević", "Đurđević", 26, "CRO", 180, 76, RIGHT, CM, CDM_f, 66, 70, 800000, 1500, 1600000, 4, 6);
        AddSimple(RIJ, "Luka", "Capan", "Capan", 25, "CRO", 180, 74, RIGHT, RB, RM_f, 65, 72, 700000, 1500, 1400000, 5, 13);
        AddSimple(RIJ, "Dario", "Benedičič", "Benedičič", 26, "CRO", 182, 76, RIGHT, CM, CAM_f, 67, 72, 1000000, 1500, 2000000, 5, 10);
        AddSimple(RIJ, "Šime", "Vrsaljko", "Vrsaljko", 33, "CRO", 183, 76, RIGHT, RB, RM_f, 74, 74, 2500000, 6000, 5000000, 2, 2);
        AddSimple(RIJ, "Dino", "Štiglec", "Štiglec", 30, "CRO", 178, 72, LEFT, LB, LM_f, 68, 70, 600000, 2000, 1200000, 3, 3);
        AddSimple(RIJ, "Franko", "Andrijašević", "Andrijašević", 34, "CRO", 182, 76, RIGHT, CAM, CM_f, 72, 72, 1200000, 4000, 2500000, 2, 7);
        AddSimple(RIJ, "Kristijan", "Lovrić", "Lovrić", 28, "CRO", 178, 74, RIGHT, LW, RW_f, 72, 74, 2500000, 4000, 5000000, 4, 17);
        AddSimple(RIJ, "Petar", "Sučić", "Sučić", 21, "CRO", 182, 76, RIGHT, CM, CDM_f, 64, 78, 1500000, 800, 3000000, 5, 18);
        AddSimple(RIJ, "Nemanja", "Antonić", "Antonić", 30, "SRB", 186, 80, RIGHT, CB, 0, 68, 70, 600000, 2000, 1200000, 3, 15);
        AddSimple(RIJ, "Andrija", "Balic", "Balic", 27, "CRO", 180, 74, RIGHT, CM, CAM_f, 68, 72, 1000000, 2000, 2000000, 4, 19);
        AddSimple(RIJ, "Luka", "Ivanušec", "Ivanušec", 26, "CRO", 175, 70, RIGHT, CAM, LW_f, 74, 78, 4000000, 5000, 8000000, 5, 20);
        AddSimple(RIJ, "Nikola", "Đurđić", "Đurđić", 38, "SRB", 185, 80, RIGHT, ST, CF_f, 68, 68, 200000, 2500, 500000, 1, 21);
        AddSimple(RIJ, "Jakov", "Puljić", "Puljić", 30, "CRO", 188, 82, RIGHT, ST, CF_f, 70, 72, 1200000, 3000, 2500000, 3, 22);
        // Conference League Teams 153-156
        // Generated player data using AddSimple method

        // ========== TEAM 153: AEK LARNACA ==========
        int AEK = 153;
        AddSimple(AEK, "Bruno", "Vale", "Bruno Vale", 35, "POR", 190, 84, RIGHT, GK, GK_f, 72, 72, 600000, 5500, 1200000, 1, 1);
        AddSimple(AEK, "Maximo", "Acuna", "Maximo Acuna", 31, "ARG", 178, 76, LEFT, CB, CB_f, 68, 68, 450000, 4500, 900000, 2, 4);
        AddSimple(AEK, "Joan", "Tomas", "Joan Tomas", 31, "ESP", 184, 78, RIGHT, CB, CB_f, 67, 67, 400000, 4000, 800000, 2, 5);
        AddSimple(AEK, "Mijat", "Gacinovic", "Mijat Gacinovic", 30, "SRB", 177, 72, RIGHT, CM, CM_f, 73, 73, 1800000, 12000, 3600000, 2, 10);
        AddSimple(AEK, "Mickael", "Pote", "Mickael Pote", 34, "CPV", 182, 78, RIGHT, ST, ST_f, 70, 70, 700000, 6000, 1400000, 1, 9);
        AddSimple(AEK, "Tete", "", "Tete", 25, "BRA", 178, 70, RIGHT, RW, RW_f, 74, 76, 2500000, 15000, 5000000, 3, 7);
        AddSimple(AEK, "Geny", "Catamo", "Geny Catamo", 26, "MOZ", 173, 68, RIGHT, LW, LW_f, 72, 74, 1500000, 10000, 3000000, 3, 11);
        AddSimple(AEK, "Ivan", "Trichkovski", "Ivan Trichkovski", 38, "MKD", 182, 76, RIGHT, ST, ST_f, 70, 70, 500000, 5000, 1000000, 1, 17);
        AddSimple(AEK, "Alexandre", "Grimaldi", "Alexandre Grimaldi", 28, "FRA", 178, 74, LEFT, LB, LB_f, 66, 66, 350000, 3500, 700000, 2, 14);
        AddSimple(AEK, "Facundo", "Kruspzky", "Facundo Kruspzky", 32, "ARG", 175, 72, RIGHT, RB, RB_f, 65, 65, 300000, 3000, 600000, 2, 2);
        AddSimple(AEK, "Stefan", "Savic", "Stefan Savic", 31, "SRB", 186, 80, RIGHT, CB, CB_f, 67, 67, 400000, 4000, 800000, 2, 15);
        AddSimple(AEK, "Nicolas", "Gimenez", "Nicolas Gimenez", 28, "ARG", 185, 79, RIGHT, CB, CB_f, 66, 66, 350000, 3500, 700000, 2, 3);
        AddSimple(AEK, "Stef", "Peeters", "Stef Peeters", 33, "BEL", 180, 75, RIGHT, CM, CM_f, 68, 68, 450000, 4500, 900000, 1, 8);
        AddSimple(AEK, "Ahmet", "Calik", "Ahmet Calik", 31, "TUR", 185, 80, RIGHT, CB, CB_f, 67, 67, 400000, 4000, 800000, 2, 6);
        AddSimple(AEK, "Faton", "Toski", "Faton Toski", 28, "KVX", 176, 72, RIGHT, CAM, CAM_f, 66, 66, 350000, 3500, 700000, 2, 20);
        AddSimple(AEK, "Murilo", "", "Murilo", 28, "BRA", 184, 78, RIGHT, ST, ST_f, 65, 65, 300000, 3000, 600000, 2, 19);
        AddSimple(AEK, "Luis", "Rocha", "Luis Rocha", 32, "POR", 183, 77, RIGHT, CB, CB_f, 65, 65, 300000, 3000, 600000, 2, 22);
        AddSimple(AEK, "Josue", "Sa", "Josue Sa", 33, "POR", 188, 82, RIGHT, CB, CB_f, 66, 66, 350000, 3500, 700000, 1, 16);
        AddSimple(AEK, "Carlos", "Mora", "Carlos Mora", 25, "CRC", 175, 72, RIGHT, ST, ST_f, 60, 64, 200000, 2000, 400000, 3, 18);
        AddSimple(AEK, "Petros", "Pontikos", "Petros Pontikos", 24, "CYP", 178, 73, RIGHT, CM, CM_f, 60, 66, 250000, 2500, 500000, 3, 21);
        AddSimple(AEK, "George", "Kolokoudias", "George Kolokoudias", 25, "CYP", 177, 72, RIGHT, CM, CM_f, 59, 64, 200000, 2000, 400000, 3, 23);
        AddSimple(AEK, "Nuno", "Morais", "Nuno Morais", 42, "POR", 185, 78, RIGHT, CDM, CDM_f, 68, 68, 300000, 3500, 600000, 1, 12);

        // ========== TEAM 154: AC OMONOIA ==========
        int OMO = 154;
        AddSimple(OMO, "Boy", "Waterman", "Boy Waterman", 41, "NED", 188, 84, RIGHT, GK, GK_f, 70, 70, 350000, 4000, 700000, 1, 1);
        AddSimple(OMO, "Konstantinos", "Sotiriou", "Konstantinos Sotiriou", 28, "CYP", 186, 80, RIGHT, GK, GK_f, 67, 69, 400000, 3500, 800000, 2, 23);
        AddSimple(OMO, "Carlao", "", "Carlao", 39, "BRA", 188, 82, RIGHT, CB, CB_f, 72, 72, 500000, 5000, 1000000, 1, 5);
        AddSimple(OMO, "Edimilson", "Fernandes", "Edimilson Fernandes", 29, "SUI", 186, 78, RIGHT, CM, CM_f, 73, 73, 1800000, 12000, 3600000, 2, 8);
        AddSimple(OMO, "David", "Gomez", "David Gomez", 31, "ESP", 180, 75, LEFT, LB, LB_f, 66, 66, 350000, 3500, 700000, 2, 3);
        AddSimple(OMO, "Lamine", "Sherif", "Lamine Sherif", 27, "MLI", 185, 79, RIGHT, CB, CB_f, 65, 66, 350000, 3000, 700000, 2, 14);
        AddSimple(OMO, "Christos", "Minas", "Christos Minas", 25, "CYP", 177, 72, LEFT, LB, LB_f, 64, 67, 300000, 2800, 600000, 3, 22);
        AddSimple(OMO, "Sherif", "Diarra", "Sherif Diarra", 26, "MLI", 182, 76, RIGHT, CDM, CDM_f, 66, 68, 400000, 3500, 800000, 3, 6);
        AddSimple(OMO, "Jordi", "Gomez", "Jordi Gomez", 39, "ESP", 178, 74, LEFT, CAM, CAM_f, 72, 72, 600000, 6000, 1200000, 1, 10);
        AddSimple(OMO, "Stelios", "Kittos", "Stelios Kittos", 22, "CYP", 175, 70, RIGHT, CM, CM_f, 62, 70, 350000, 2500, 700000, 3, 25);
        AddSimple(OMO, "Christian", "Kouame", "Christian Kouame", 27, "CIV", 181, 76, RIGHT, ST, ST_f, 73, 74, 2000000, 13000, 4000000, 2, 9);
        AddSimple(OMO, "Ghayas", "Zahid", "Ghayas Zahid", 30, "NOR", 177, 72, RIGHT, CAM, CAM_f, 70, 70, 800000, 7000, 1600000, 2, 11);
        AddSimple(OMO, "Farrukh", "Tashkentov", "Farrukh Tashkentov", 29, "UZB", 186, 80, RIGHT, CB, CB_f, 65, 66, 350000, 3000, 700000, 2, 4);
        AddSimple(OMO, "Andres", "Martin", "Andres Martin", 26, "ESP", 176, 72, RIGHT, RB, RB_f, 66, 67, 350000, 3500, 700000, 2, 2);
        AddSimple(OMO, "Bruno", "Gama", "Bruno Gama", 37, "POR", 173, 68, LEFT, RW, RW_f, 70, 70, 500000, 5000, 1000000, 1, 17);
        AddSimple(OMO, "Loizos", "Loizou", "Loizos Loizou", 22, "CYP", 174, 70, RIGHT, RW, RW_f, 63, 70, 400000, 2800, 800000, 3, 18);
        AddSimple(OMO, "Andrew", "Omobamidele", "Andrew Omobamidele", 23, "IRL", 187, 80, RIGHT, CB, CB_f, 68, 75, 1800000, 8000, 3600000, 3, 15);
        AddSimple(OMO, "Nikola", "Petkovic", "Nikola Petkovic", 33, "SRB", 185, 79, RIGHT, CB, CB_f, 66, 66, 300000, 3000, 600000, 2, 21);
        AddSimple(OMO, "Jordi", "", "Jordi", 30, "ESP", 182, 76, RIGHT, CB, CB_f, 65, 65, 300000, 3000, 600000, 2, 20);
        AddSimple(OMO, "Adailton", "", "Adailton", 33, "BRA", 180, 76, RIGHT, ST, ST_f, 69, 69, 500000, 5000, 1000000, 2, 19);
        AddSimple(OMO, "Andros", "Townsend", "Andros Townsend", 33, "ENG", 181, 75, LEFT, RW, RW_f, 75, 75, 2500000, 18000, 5000000, 2, 7);
        AddSimple(OMO, "Jason", "Knight", "Jason Knight", 24, "IRL", 173, 68, RIGHT, CM, CM_f, 72, 75, 2000000, 10000, 4000000, 3, 12);

        // ========== TEAM 155: SK SIGMA OLOMOUC ==========
        int OLO = 155;
        AddSimple(OLO, "Vaclav", "Kotal", "Vaclav Kotal", 27, "CZE", 190, 84, RIGHT, GK, GK_f, 65, 68, 350000, 3000, 700000, 2, 1);
        AddSimple(OLO, "Jan", "Reichl", "Jan Reichl", 30, "CZE", 188, 82, RIGHT, GK, GK_f, 63, 64, 250000, 2500, 500000, 2, 12);
        AddSimple(OLO, "David", "Bystrican", "David Bystrican", 24, "CZE", 183, 77, RIGHT, CB, CB_f, 62, 67, 300000, 2500, 600000, 3, 4);
        AddSimple(OLO, "Jakub", "Pokorny", "Jakub Pokorny", 27, "CZE", 186, 80, RIGHT, CB, CB_f, 66, 68, 400000, 3500, 800000, 2, 3);
        AddSimple(OLO, "Ondrej", "Zmrzly", "Ondrej Zmrzly", 26, "CZE", 184, 78, RIGHT, CB, CB_f, 65, 67, 350000, 3000, 700000, 2, 5);
        AddSimple(OLO, "Jakub", "Kvet", "Jakub Kvet", 28, "CZE", 178, 73, RIGHT, CM, CM_f, 67, 68, 450000, 3500, 900000, 2, 6);
        AddSimple(OLO, "Filip", "Zorvan", "Filip Zorvan", 28, "CZE", 177, 72, RIGHT, CAM, CAM_f, 68, 69, 500000, 4000, 1000000, 2, 10);
        AddSimple(OLO, "Michal", "Veprek", "Michal Veprek", 26, "CZE", 175, 70, LEFT, LB, LB_f, 64, 66, 300000, 2800, 600000, 2, 14);
        AddSimple(OLO, "Lukas", "Julis", "Lukas Julis", 30, "CZE", 182, 77, RIGHT, ST, ST_f, 66, 66, 400000, 3500, 800000, 2, 9);
        AddSimple(OLO, "Simon", "Falta", "Simon Falta", 31, "CZE", 174, 70, RIGHT, LW, LW_f, 65, 65, 300000, 3000, 600000, 2, 11);
        AddSimple(OLO, "Ales", "Cermak", "Ales Cermak", 30, "CZE", 180, 75, RIGHT, CM, CM_f, 67, 67, 400000, 3500, 800000, 2, 8);
        AddSimple(OLO, "Filip", "Valka", "Filip Válka", 22, "CZE", 183, 76, RIGHT, ST, ST_f, 60, 68, 350000, 2000, 700000, 3, 21);
        AddSimple(OLO, "Tomas", "Zahradnicek", "Tomas Zahradnicek", 30, "CZE", 185, 79, RIGHT, CB, CB_f, 63, 63, 250000, 2500, 500000, 2, 17);
        AddSimple(OLO, "Tomas", "Zlatohlavek", "Tomas Zlatohlavek", 26, "CZE", 180, 75, RIGHT, ST, ST_f, 64, 65, 300000, 2800, 600000, 2, 19);
        AddSimple(OLO, "Libor", "Holik", "Libor Holik", 27, "CZE", 176, 72, RIGHT, RB, RB_f, 65, 66, 300000, 3000, 600000, 2, 2);
        AddSimple(OLO, "Ondrej", "Zmrzly", "Ondrej Zmrzly", 26, "CZE", 183, 77, RIGHT, CB, CB_f, 62, 63, 250000, 2500, 500000, 2, 15);
        AddSimple(OLO, "Jan", "Navratil", "Jan Navratil", 34, "CZE", 175, 72, RIGHT, RW, RW_f, 66, 66, 350000, 3000, 700000, 1, 7);
        AddSimple(OLO, "Lukas", "Kalvach", "Lukas Kalvach", 29, "CZE", 181, 76, RIGHT, CDM, CDM_f, 67, 68, 400000, 3500, 800000, 2, 16);
        AddSimple(OLO, "David", "Houska", "David Houska", 31, "CZE", 178, 74, RIGHT, CM, CM_f, 66, 66, 350000, 3000, 700000, 2, 18);
        AddSimple(OLO, "Ondrej", "Placek", "Ondrej Placek", 23, "CZE", 189, 83, RIGHT, GK, GK_f, 60, 67, 250000, 2000, 500000, 3, 23);
        AddSimple(OLO, "Radek", "Latal", "Radek Latal", 28, "CZE", 177, 72, RIGHT, RB, RB_f, 63, 64, 250000, 2500, 500000, 2, 20);
        AddSimple(OLO, "Jakub", "Reznicek", "Jakub Reznicek", 34, "CZE", 184, 78, RIGHT, ST, ST_f, 65, 65, 300000, 2800, 600000, 1, 22);

        // ========== TEAM 156: AC SPARTA PRAHA ==========
        int SPA = 156;
        AddSimple(SPA, "Peter", "Vindahl Jensen", "Peter Vindahl Jensen", 27, "DEN", 196, 88, RIGHT, GK, GK_f, 73, 75, 2000000, 10000, 4000000, 2, 1);
        AddSimple(SPA, "Matej", "Kovar", "Matej Kovar", 25, "CZE", 190, 84, RIGHT, GK, GK_f, 72, 76, 2200000, 9000, 4400000, 3, 24);
        AddSimple(SPA, "Tomas", "Vaclik", "Tomas Vaclik", 36, "CZE", 188, 83, RIGHT, GK, GK_f, 74, 74, 800000, 8000, 1600000, 1, 28);
        AddSimple(SPA, "Filip", "Soucek", "Filip Soucek", 26, "CZE", 185, 79, RIGHT, CB, CB_f, 67, 69, 500000, 4000, 1000000, 2, 4);
        AddSimple(SPA, "Angelo", "Preciado", "Angelo Preciado", 27, "ECU", 175, 72, RIGHT, RB, RB_f, 70, 72, 1200000, 8000, 2400000, 2, 2);
        AddSimple(SPA, "Ladislav", "Krejci", "Ladislav Krejci", 26, "CZE", 186, 80, RIGHT, CB, CB_f, 72, 74, 1800000, 10000, 3600000, 3, 3);
        AddSimple(SPA, "Lukas", "Haraslin", "Lukas Haraslin", 29, "SVK", 178, 74, RIGHT, LW, LW_f, 71, 71, 1500000, 9000, 3000000, 2, 9);
        AddSimple(SPA, "Martin", "Vitik", "Martin Vitik", 22, "CZE", 187, 80, RIGHT, CB, CB_f, 68, 76, 2000000, 7000, 4000000, 3, 5);
        AddSimple(SPA, "Tomas", "Wiesner", "Tomas Wiesner", 27, "CZE", 176, 72, RIGHT, RB, RB_f, 65, 66, 350000, 3000, 700000, 2, 13);
        AddSimple(SPA, "Asger", "Sorensen", "Asger Sorensen", 28, "DEN", 184, 78, RIGHT, CB, CB_f, 66, 67, 400000, 3500, 800000, 2, 15);
        AddSimple(SPA, "Kaan", "Kairinen", "Kaan Kairinen", 26, "FIN", 180, 75, RIGHT, CM, CM_f, 67, 69, 500000, 4000, 1000000, 3, 8);
        AddSimple(SPA, "David", "Pavelka", "David Pavelka", 34, "CZE", 182, 76, RIGHT, CM, CM_f, 68, 68, 500000, 4500, 1000000, 1, 16);
        AddSimple(SPA, "Tomas", "Chory", "Tomas Chory", 29, "CZE", 187, 82, RIGHT, ST, ST_f, 69, 70, 800000, 6000, 1600000, 2, 18);
        AddSimple(SPA, "Veljko", "Birmancevic", "Veljko Birmancevic", 27, "SRB", 178, 73, RIGHT, RW, RW_f, 68, 70, 700000, 5500, 1400000, 2, 10);
        AddSimple(SPA, "Matej", "Rynes", "Matej Rynes", 24, "CZE", 176, 71, LEFT, LB, LB_f, 64, 68, 350000, 2800, 700000, 3, 23);
        AddSimple(SPA, "Markus", "Solbakken", "Markus Solbakken", 24, "NOR", 180, 75, RIGHT, CM, CM_f, 65, 69, 500000, 3500, 1000000, 3, 21);
        AddSimple(SPA, "Qazim", "Laci", "Qazim Laci", 27, "ALB", 178, 73, RIGHT, CAM, CAM_f, 66, 67, 400000, 3500, 800000, 2, 20);
        AddSimple(SPA, "Filip", "Panak", "Filip Panak", 29, "CZE", 175, 70, LEFT, LB, LB_f, 63, 64, 250000, 2500, 500000, 2, 12);
        AddSimple(SPA, "Ewerton", "", "Ewerton", 32, "BRA", 185, 80, RIGHT, CB, CB_f, 67, 67, 400000, 4000, 800000, 2, 6);
        AddSimple(SPA, "Jan", "Kuchta", "Jan Kuchta", 28, "CZE", 182, 77, RIGHT, ST, ST_f, 70, 70, 1000000, 7000, 2000000, 2, 14);
        AddSimple(SPA, "Rrahmani", "", "Rrahmani", 24, "KVX", 184, 78, RIGHT, ST, ST_f, 65, 70, 600000, 3500, 1200000, 3, 7);
        AddSimple(SPA, "Victor", "Olatunji", "Victor Olatunji", 25, "NGA", 188, 82, RIGHT, ST, ST_f, 64, 68, 400000, 2800, 800000, 3, 11);

        // Conference League Teams - Teams 157-160
        // Generated player data


        // --- Team 157: Kuopion Palloseura (KPS) ---
        int KPS = 157;
        AddSimple(KPS, "Mihail", "Seremet", "M. Seremet", 24, "FIN", 189, 82, RIGHT, GK, GK_f, 62, 68, 180000, 3000, 360000, 3, 1);
        AddSimple(KPS, "Ville", "Jalasto", "V. Jalasto", 38, "FIN", 180, 75, RIGHT, RB, RB_f, 63, 63, 120000, 2500, 240000, 1, 2);
        AddSimple(KPS, "Hannu", "Patronen", "H. Patronen", 41, "FIN", 185, 80, RIGHT, CB, CB_f, 64, 64, 100000, 2200, 200000, 1, 3);
        AddSimple(KPS, "Tomi", "Vaananen", "T. Vaananen", 34, "FIN", 178, 73, LEFT, LB, LB_f, 62, 62, 130000, 2600, 260000, 1, 4);
        AddSimple(KPS, "Akseli", "Pelvas", "A. Pelvas", 35, "FIN", 182, 78, RIGHT, ST, ST_f, 64, 64, 140000, 2800, 280000, 1, 5);
        AddSimple(KPS, "Thomas", "Silander", "T. Silander", 27, "FIN", 175, 70, RIGHT, CM, CM_f, 61, 65, 160000, 3000, 320000, 3, 6);
        AddSimple(KPS, "Petri", "Pasanen", "P. Pasanen", 44, "FIN", 186, 82, RIGHT, CB, CB_f, 60, 60, 80000, 2000, 160000, 1, 7);
        AddSimple(KPS, "Niko", "Hamalainen", "N. Hamalainen", 28, "FIN", 177, 72, LEFT, LB, LB_f, 63, 67, 200000, 3200, 400000, 2, 8);
        AddSimple(KPS, "Aleksei", "Kangaskolkka", "A. Kangaskolkka", 36, "FIN", 184, 80, RIGHT, ST, ST_f, 65, 65, 150000, 2800, 300000, 1, 9);
        AddSimple(KPS, "Mikko", "Kuningas", "M. Kuningas", 27, "FIN", 176, 71, RIGHT, CM, CM_f, 62, 66, 170000, 3100, 340000, 3, 10);
        AddSimple(KPS, "Anttoni", "Rissanen", "A. Rissanen", 22, "FIN", 175, 70, RIGHT, RB, RB_f, 58, 72, 220000, 2600, 440000, 4, 11);
        AddSimple(KPS, "Lassi", "Lappalainen", "L. Lappalainen", 26, "FIN", 181, 76, RIGHT, LW, LW_f, 66, 72, 350000, 4000, 700000, 3, 12);
        AddSimple(KPS, "Aaron", "Chukwu", "A. Chukwu", 25, "NGA", 188, 85, RIGHT, ST, ST_f, 60, 68, 180000, 2800, 360000, 3, 13);
        AddSimple(KPS, "Valtteri", "Moren", "V. Moren", 26, "FIN", 184, 79, RIGHT, CB, CB_f, 63, 70, 280000, 3300, 560000, 4, 14);
        AddSimple(KPS, "Joona", "Toivio", "J. Toivio", 36, "FIN", 186, 81, RIGHT, CB, CB_f, 65, 65, 110000, 2400, 220000, 1, 15);
        AddSimple(KPS, "Teemu", "Pukki", "T. Pukki", 35, "FIN", 180, 76, RIGHT, ST, ST_f, 70, 70, 400000, 6000, 800000, 1, 16);
        AddSimple(KPS, "Roope", "Riski", "R. Riski", 33, "FIN", 183, 79, RIGHT, ST, ST_f, 63, 63, 140000, 2700, 280000, 2, 17);
        AddSimple(KPS, "Toni", "Vidovic", "T. Vidovic", 26, "CRO", 178, 73, RIGHT, CM, CM_f, 64, 70, 250000, 3200, 500000, 3, 18);
        AddSimple(KPS, "Juhani", "Ojala", "J. Ojala", 35, "FIN", 185, 80, RIGHT, CB, CB_f, 64, 64, 130000, 2600, 260000, 1, 19);
        AddSimple(KPS, "Martin", "Ojala", "M. Ojala", 25, "FIN", 188, 83, RIGHT, GK, GK_f, 60, 68, 150000, 2600, 300000, 3, 20);
        AddSimple(KPS, "Riku", "Riski", "R. Riski", 33, "FIN", 178, 73, RIGHT, LW, LW_f, 62, 62, 130000, 2600, 260000, 2, 21);
        AddSimple(KPS, "Samu", "Salmela", "S. Salmela", 22, "FIN", 183, 78, RIGHT, CB, CB_f, 57, 70, 160000, 2200, 320000, 4, 22);

        // --- Team 158: RC Strasbourg Alsace (STR) ---
        int STR = 158;
        AddSimple(STR, "Matz", "Sels", "M. Sels", 33, "BEL", 188, 80, RIGHT, GK, GK_f, 76, 76, 3500000, 14000, 7000000, 2, 1);
        AddSimple(STR, "Bingourou", "Kamara", "B. Kamara", 28, "SEN", 196, 90, RIGHT, GK, GK_f, 68, 72, 900000, 6000, 1800000, 2, 16);
        AddSimple(STR, "Gerzino", "Nyamsi", "G. Nyamsi", 28, "FRA", 188, 82, RIGHT, CB, CB_f, 72, 75, 2500000, 10000, 5000000, 3, 22);
        AddSimple(STR, "Alexander", "Djiku", "A. Djiku", 30, "GHA", 182, 78, RIGHT, CB, CB_f, 75, 76, 4000000, 14000, 8000000, 2, 24);
        AddSimple(STR, "Maxime", "Le Marchand", "M. Le Marchand", 35, "FRA", 185, 80, LEFT, CB, CB_f, 70, 70, 600000, 7000, 1200000, 1, 5);
        AddSimple(STR, "Abakar", "Sylla", "A. Sylla", 22, "CIV", 186, 80, RIGHT, CB, CB_f, 70, 78, 5000000, 10000, 10000000, 4, 3);
        AddSimple(STR, "Leonidas", "Stergiou", "L. Stergiou", 23, "SUI", 181, 76, RIGHT, CB, CB_f, 69, 76, 3500000, 9000, 7000000, 4, 2);
        AddSimple(STR, "Lucas", "Perrin", "L. Perrin", 26, "FRA", 184, 79, RIGHT, CB, CB_f, 71, 74, 2800000, 10000, 5600000, 3, 14);
        AddSimple(STR, "Jean-Ricner", "Bellegarde", "J. Bellegarde", 27, "FRA", 176, 72, RIGHT, CM, CM_f, 74, 76, 5000000, 13000, 10000000, 3, 17);
        AddSimple(STR, "Lebo", "Mothiba", "L. Mothiba", 29, "RSA", 182, 80, RIGHT, ST, ST_f, 72, 73, 3000000, 11000, 6000000, 2, 9);
        AddSimple(STR, "Frederic", "Guilbert", "F. Guilbert", 30, "FRA", 178, 73, RIGHT, RB, RB_f, 73, 73, 3200000, 12000, 6400000, 2, 4);
        AddSimple(STR, "Habib", "Diallo", "H. Diallo", 30, "SEN", 186, 82, RIGHT, ST, ST_f, 74, 74, 4500000, 13000, 9000000, 2, 20);
        AddSimple(STR, "Nordine", "Keroumi", "N. Keroumi", 22, "FRA", 175, 70, RIGHT, CM, CM_f, 62, 74, 1200000, 5000, 2400000, 4, 8);
        AddSimple(STR, "Adrien", "Thomasson", "A. Thomasson", 31, "FRA", 175, 72, RIGHT, CM, CM_f, 75, 75, 4000000, 14000, 8000000, 2, 11);
        AddSimple(STR, "Jeremy", "Blasco", "J. Blasco", 21, "FRA", 180, 75, RIGHT, CB, CB_f, 60, 74, 1000000, 4000, 2000000, 4, 12);
        AddSimple(STR, "Sanjin", "Prcic", "S. Prcic", 31, "BIH", 181, 77, RIGHT, CDM, CDM_f, 71, 71, 1800000, 10000, 3600000, 1, 6);
        AddSimple(STR, "Nuno", "Da Costa", "N. Da Costa", 34, "CPV", 184, 80, RIGHT, ST, ST_f, 68, 68, 800000, 7000, 1600000, 1, 7);
        AddSimple(STR, "Kevin", "Gameiro", "K. Gameiro", 38, "FRA", 172, 68, RIGHT, ST, ST_f, 75, 75, 1800000, 12000, 3600000, 1, 10);
        AddSimple(STR, "Kevin", "Zohi", "K. Zohi", 28, "CIV", 178, 74, RIGHT, RW, RW_f, 69, 70, 2000000, 9000, 4000000, 2, 19);
        AddSimple(STR, "Ludovic", "Ajorque", "L. Ajorque", 31, "FRA", 196, 90, RIGHT, ST, ST_f, 76, 76, 5500000, 15000, 11000000, 2, 18);
        AddSimple(STR, "Ibrahima", "Sissoko", "I. Sissoko", 28, "FRA", 186, 82, RIGHT, CDM, CDM_f, 73, 74, 3500000, 12000, 7000000, 3, 13);
        AddSimple(STR, "Colin", "Dagba", "C. Dagba", 26, "FRA", 170, 66, RIGHT, RB, RB_f, 71, 73, 2500000, 10000, 5000000, 3, 15);

        // --- Team 159: 1. FSV Mainz 05 (MAIN) ---
        int MAIN = 159;
        AddSimple(MAIN, "Robin", "Zentner", "R. Zentner", 30, "GER", 194, 88, RIGHT, GK, GK_f, 76, 78, 5000000, 15000, 10000000, 3, 27);
        AddSimple(MAIN, "Finn", "Dahmen", "F. Dahmen", 27, "GER", 186, 80, RIGHT, GK, GK_f, 70, 74, 2000000, 9000, 4000000, 3, 1);
        AddSimple(MAIN, "Andreas", "Hanche-Olsen", "A. Hanche-Olsen", 28, "NOR", 185, 80, RIGHT, CB, CB_f, 73, 76, 4000000, 12000, 8000000, 3, 25);
        AddSimple(MAIN, "Alexander", "Hack", "A. Hack", 31, "GER", 188, 83, RIGHT, CB, CB_f, 72, 72, 2500000, 11000, 5000000, 2, 42);
        AddSimple(MAIN, "Stefan", "Bell", "S. Bell", 33, "GER", 186, 82, RIGHT, CB, CB_f, 73, 73, 2000000, 11000, 4000000, 1, 16);
        AddSimple(MAIN, "Silvan", "Widmer", "S. Widmer", 32, "SUI", 182, 78, RIGHT, RB, RB_f, 75, 75, 4000000, 14000, 8000000, 2, 30);
        AddSimple(MAIN, "Danny", "da Costa", "D. da Costa", 30, "GER", 183, 79, RIGHT, RB, RB_f, 72, 72, 2500000, 11000, 5000000, 2, 20);
        AddSimple(MAIN, "Edimilson", "Fernandes", "E. Fernandes", 29, "SUI", 185, 80, RIGHT, CM, CM_f, 72, 73, 3000000, 12000, 6000000, 2, 14);
        AddSimple(MAIN, "Jonathan", "Burkardt", "J. Burkardt", 25, "GER", 183, 78, RIGHT, ST, ST_f, 74, 78, 8000000, 14000, 16000000, 4, 29);
        AddSimple(MAIN, "Leandro", "Barreiro", "L. Barreiro", 25, "LUX", 174, 70, RIGHT, CDM, CDM_f, 73, 77, 6000000, 13000, 12000000, 4, 8);
        AddSimple(MAIN, "Aymen", "Barkok", "A. Barkok", 27, "MAR", 184, 79, RIGHT, CM, CM_f, 71, 73, 2800000, 11000, 5600000, 3, 10);
        AddSimple(MAIN, "Bo", "Svensson", "B. Svensson", 27, "DEN", 180, 75, LEFT, CB, CB_f, 68, 72, 1500000, 8000, 3000000, 3, 23);
        AddSimple(MAIN, "Angelo", "Fulgini", "A. Fulgini", 28, "FRA", 184, 79, RIGHT, CAM, CAM_f, 74, 75, 5000000, 13000, 10000000, 3, 11);
        AddSimple(MAIN, "Dominik", "Kohr", "D. Kohr", 31, "GER", 183, 78, RIGHT, CDM, CDM_f, 74, 74, 3500000, 13000, 7000000, 2, 31);
        AddSimple(MAIN, "Anton", "Stach", "A. Stach", 26, "GER", 188, 82, RIGHT, CM, CM_f, 74, 77, 7000000, 14000, 14000000, 4, 6);
        AddSimple(MAIN, "Paul", "Nebel", "P. Nebel", 22, "GER", 176, 72, LEFT, CAM, CAM_f, 66, 76, 3000000, 7000, 6000000, 4, 17);
        AddSimple(MAIN, "Jae-sung", "Lee", "J. Lee", 32, "KOR", 182, 76, RIGHT, CM, CM_f, 74, 74, 3000000, 12000, 6000000, 2, 7);
        AddSimple(MAIN, "Karim", "Onisiwo", "K. Onisiwo", 33, "AUT", 188, 83, RIGHT, ST, ST_f, 75, 75, 4000000, 14000, 8000000, 2, 9);
        AddSimple(MAIN, "Adam", "Szalai", "A. Szalai", 37, "HUN", 189, 84, RIGHT, ST, ST_f, 70, 70, 600000, 8000, 1200000, 1, 28);
        AddSimple(MAIN, "Sekou", "Koita", "S. Koita", 25, "MLI", 180, 76, RIGHT, ST, ST_f, 72, 76, 5000000, 12000, 10000000, 4, 11);
        AddSimple(MAIN, "Marko", "Dmitrovic", "M. Dmitrovic", 33, "SRB", 192, 86, RIGHT, GK, GK_f, 74, 74, 2500000, 12000, 5000000, 2, 1);
        AddSimple(MAIN, "Marcus", "Ingvartsen", "M. Ingvartsen", 29, "DEN", 186, 80, RIGHT, ST, ST_f, 73, 73, 3500000, 12000, 7000000, 2, 15);

        // --- Team 160: Lincoln Red Imps FC (LIN) ---
        int LIN = 160;
        AddSimple(LIN, "Jaylan", "Hankins", "J. Hankins", 25, "ENG", 185, 78, RIGHT, GK, GK_f, 58, 64, 60000, 1200, 120000, 3, 1);
        AddSimple(LIN, "Nauzet", "Santana", "N. Santana", 35, "ESP", 184, 80, RIGHT, GK, GK_f, 60, 60, 40000, 1000, 80000, 1, 13);
        AddSimple(LIN, "Francisco", "Fernandez", "F. Fernandez", 31, "ESP", 180, 76, RIGHT, CB, CB_f, 61, 61, 70000, 1500, 140000, 1, 3);
        AddSimple(LIN, "Ibrahim", "Ayew", "I. Ayew", 25, "GHA", 182, 78, RIGHT, CB, CB_f, 59, 65, 80000, 1400, 160000, 3, 4);
        AddSimple(LIN, "Bernardo", "Lopes", "B. Lopes", 31, "POR", 185, 80, RIGHT, CB, CB_f, 62, 62, 90000, 1600, 180000, 2, 6);
        AddSimple(LIN, "Ethan", "Britto", "E. Britto", 24, "GIB", 175, 70, RIGHT, RB, RB_f, 57, 64, 50000, 1000, 100000, 3, 2);
        AddSimple(LIN, "Christian", "Rutjens", "C. Rutjens", 28, "NED", 183, 79, RIGHT, CB, CB_f, 60, 62, 60000, 1300, 120000, 2, 5);
        AddSimple(LIN, "Jesus", "Toscano", "J. Toscano", 30, "ESP", 181, 77, RIGHT, CB, CB_f, 61, 61, 70000, 1400, 140000, 2, 16);
        AddSimple(LIN, "Kyle", "Clinton", "K. Clinton", 26, "GIB", 176, 72, RIGHT, CM, CM_f, 56, 62, 45000, 1000, 90000, 3, 8);
        AddSimple(LIN, "Graeme", "Torrilla", "G. Torrilla", 27, "GIB", 174, 70, RIGHT, CM, CM_f, 57, 62, 50000, 1000, 100000, 3, 10);
        AddSimple(LIN, "Nicholas", "Pozo", "N. Pozo", 24, "GIB", 178, 74, RIGHT, CM, CM_f, 55, 63, 45000, 900, 90000, 3, 14);
        AddSimple(LIN, "Mandi", "Mandi", "Mandi", 22, "GIB", 180, 76, RIGHT, ST, ST_f, 56, 66, 55000, 900, 110000, 4, 7);
        AddSimple(LIN, "Joe", "Joe", "Joe", 26, "GIB", 182, 78, RIGHT, ST, ST_f, 57, 61, 50000, 1000, 100000, 2, 11);
        AddSimple(LIN, "Toni", "Garcia", "T. Garcia", 28, "ESP", 180, 76, RIGHT, ST, ST_f, 60, 61, 70000, 1400, 140000, 2, 9);
        AddSimple(LIN, "Nano", "Nano", "Nano", 28, "ESP", 178, 74, RIGHT, ST, ST_f, 59, 60, 65000, 1300, 130000, 2, 19);
        AddSimple(LIN, "Toni", "Kolega", "T. Kolega", 25, "CRO", 184, 80, RIGHT, ST, ST_f, 61, 65, 100000, 1800, 200000, 3, 17);
        AddSimple(LIN, "Tjay", "De Barr", "T. De Barr", 25, "GIB", 180, 76, RIGHT, ST, ST_f, 58, 64, 60000, 1200, 120000, 3, 20);
        AddSimple(LIN, "Lee", "Casciaro", "L. Casciaro", 43, "GIB", 170, 65, RIGHT, ST, ST_f, 55, 55, 20000, 600, 40000, 1, 21);
        AddSimple(LIN, "Kike", "Gomez", "K. Gomez", 30, "ESP", 175, 72, RIGHT, ST, ST_f, 60, 60, 60000, 1300, 120000, 2, 18);
        AddSimple(LIN, "Ayman", "El Ghobashy", "A. El Ghobashy", 25, "GIB", 176, 72, RIGHT, CM, CM_f, 55, 62, 40000, 900, 80000, 3, 12);
        AddSimple(LIN, "Juanje", "Argüez", "J. Argüez", 27, "ESP", 178, 74, RIGHT, CM, CM_f, 59, 61, 65000, 1300, 130000, 2, 15);
        AddSimple(LIN, "Victor", "Villacañas", "V. Villacañas", 26, "ESP", 180, 76, RIGHT, CM, CM_f, 58, 62, 55000, 1200, 110000, 3, 22);

        // Conference League teams 161-164
        // ── Team 161: AEK ATHENS FC ──────────────────────────────────────────
        int KLS = 161;
        AddSimple(KLS, "Panagiotis", "Retsos", "Retsos", 26, "GRE", 185, 78, RIGHT, CB, CB_f, 72, 76, 3_000_000, 15_000, 6000000, 3, 6);
        AddSimple(KLS, "Vasilis", "Barkas", "Barkas", 31, "GRE", 196, 88, RIGHT, GK, GK_f, 70, 71, 1_500_000, 12_000, 3000000, 2, 1);
        AddSimple(KLS, "Oreste", "Frara", "Frara", 22, "GRE", 190, 82, RIGHT, GK, GK_f, 62, 72, 500_000, 3_000, 1000000, 4, 13);
        AddSimple(KLS, "Chidozie", "Awaziem", "Awaziem", 28, "NGA", 189, 82, RIGHT, CB, CB_f, 71, 73, 2_500_000, 12_000, 5000000, 2, 21);
        AddSimple(KLS, "Petros", "Mantalos", "Mantalos", 33, "GRE", 176, 72, RIGHT, CAM, CAM_f, 74, 75, 4_000_000, 25_000, 8000000, 2, 10);
        AddSimple(KLS, "Sasa", "Lukic", "Lukic", 28, "SRB", 182, 76, RIGHT, CM, CM_f, 73, 75, 4_500_000, 18_000, 9000000, 3, 20);
        AddSimple(KLS, "Lazar", "Randjelovic", "Randjelovic", 27, "SRB", 178, 72, RIGHT, LW, LW_f, 69, 71, 1_800_000, 10_000, 3600000, 2, 11);
        AddSimple(KLS, "Strahinja", "Pavlovic", "Pavlovic", 24, "SRB", 194, 84, LEFT, CB, CB_f, 73, 79, 5_000_000, 15_000, 10000000, 4, 5);
        AddSimple(KLS, "Kostas", "Galanopoulos", "Galanopoulos", 27, "GRE", 175, 70, RIGHT, CM, CM_f, 70, 72, 2_000_000, 10_000, 4000000, 2, 25);
        AddSimple(KLS, "Nikos", "Bakasetas", "Bakasetas", 32, "GRE", 183, 80, RIGHT, CAM, CAM_f, 75, 76, 5_000_000, 28_000, 10000000, 2, 7);
        AddSimple(KLS, "Helder", "Lopes", "H. Lopes", 35, "POR", 177, 70, LEFT, LB, LB_f, 69, 69, 1_000_000, 10_000, 2000000, 1, 3);
        AddSimple(KLS, "Marko", "Livaja", "Livaja", 31, "CRO", 182, 78, RIGHT, ST, ST_f, 76, 77, 6_000_000, 30_000, 12000000, 2, 9);
        AddSimple(KLS, "Cedric", "Bakambu", "Bakambu", 34, "COD", 182, 76, RIGHT, ST, ST_f, 74, 74, 3_500_000, 25_000, 7000000, 1, 17);
        AddSimple(KLS, "Steven", "Zuber", "Zuber", 33, "SUI", 182, 78, RIGHT, LW, LW_f, 72, 72, 2_500_000, 18_000, 5000000, 1, 14);
        AddSimple(KLS, "Erik", "Sviatchenko", "Sviatchenko", 33, "DEN", 185, 80, RIGHT, CB, CB_f, 71, 71, 1_500_000, 12_000, 3000000, 2, 4);
        AddSimple(KLS, "Georgios", "Masouras", "Masouras", 31, "GRE", 174, 72, RIGHT, LW, LW_f, 70, 71, 2_000_000, 10_000, 4000000, 2, 19);
        AddSimple(KLS, "Damian", "Szymanski", "Szymanski", 30, "POL", 182, 76, RIGHT, CM, CM_f, 69, 70, 1_500_000, 10_000, 3000000, 2, 23);
        AddSimple(KLS, "Jakub", "Brabec", "Brabec", 32, "CZE", 186, 80, RIGHT, CB, CB_f, 68, 68, 1_000_000, 8_000, 2000000, 2, 15);
        AddSimple(KLS, "Sergio", "Araujo", "S. Araujo", 33, "ARG", 179, 76, RIGHT, ST, ST_f, 71, 71, 2_000_000, 15_000, 4000000, 1, 22);
        AddSimple(KLS, "Elias", "Chatzitheodoridis", "Chatzitheodoridis", 26, "GRE", 178, 70, LEFT, LB, LB_f, 65, 68, 800_000, 5_000, 1600000, 3, 2);
        AddSimple(KLS, "Thomas", "Monconduit", "Monconduit", 34, "FRA", 180, 74, RIGHT, CDM, CDM_f, 68, 68, 800_000, 8_000, 1600000, 1, 8);
        AddSimple(KLS, "Erik", "Bovim", "Bovim", 24, "NOR", 188, 80, RIGHT, CB, CB_f, 60, 68, 500_000, 3_000, 1000000, 3, 26);

        // ── Team 162: BREIDABLIK UBK ──────────────────────────────────────────
        int BRE = 162;

        AddSimple(BRE, "Tomas", "Johannsson", "T. Johannsson", 26, "ISL", 188, 80, RIGHT, GK, GK_f, 63, 68, 400_000, 2_000, 800000, 3, 1);
        AddSimple(BRE, "Andri", "Baldursson", "Baldursson", 24, "ISL", 186, 78, RIGHT, GK, GK_f, 58, 66, 200_000, 1_500, 400000, 3, 13);
        AddSimple(BRE, "Aron", "Thrainsson", "Thrainsson", 28, "ISL", 183, 76, RIGHT, CB, CB_f, 61, 64, 300_000, 2_000, 600000, 2, 4);
        AddSimple(BRE, "Oskar", "Hauksson", "Hauksson", 30, "ISL", 180, 74, RIGHT, CB, CB_f, 60, 62, 250_000, 1_800, 500000, 2, 5);
        AddSimple(BRE, "Kari", "Arnason", "Arnason", 42, "ISL", 190, 86, RIGHT, CB, CB_f, 65, 65, 100_000, 2_500, 200000, 1, 6);
        AddSimple(BRE, "Arnor", "Sigurdsson", "A. Sigurdsson", 26, "ISL", 177, 70, RIGHT, LW, LW_f, 67, 70, 800_000, 4_000, 1600000, 3, 7);
        AddSimple(BRE, "Gisli", "Einarsson", "Einarsson", 26, "ISL", 178, 72, RIGHT, CM, CM_f, 63, 66, 400_000, 2_500, 800000, 3, 8);
        AddSimple(BRE, "Birkir", "Saevarsson", "Saevarsson", 41, "ISL", 182, 76, RIGHT, RB, RB_f, 64, 64, 80_000, 2_000, 160000, 1, 2);
        AddSimple(BRE, "Eidur", "Gudjohnsen", "E. Gudjohnsen", 25, "ISL", 185, 78, RIGHT, ST, ST_f, 62, 68, 400_000, 2_500, 800000, 3, 9);
        AddSimple(BRE, "Stefan", "Sigurdarson", "Sigurdarson", 29, "ISL", 180, 74, RIGHT, ST, ST_f, 64, 66, 500_000, 3_000, 1000000, 2, 10);
        AddSimple(BRE, "Thorsteinn", "Ingason", "Ingason", 23, "ISL", 186, 78, RIGHT, ST, ST_f, 58, 66, 250_000, 1_500, 500000, 3, 11);
        AddSimple(BRE, "Mikael", "Anderson", "Anderson", 27, "ISL", 178, 72, RIGHT, RW, RW_f, 66, 69, 700_000, 3_500, 1400000, 2, 14);
        AddSimple(BRE, "Brynjar", "Gudmundsson", "Gudmundsson", 28, "ISL", 177, 70, RIGHT, CM, CM_f, 62, 64, 300_000, 2_000, 600000, 2, 15);
        AddSimple(BRE, "Gardar", "Johannesson", "Johannesson", 25, "ISL", 176, 70, RIGHT, CM, CM_f, 60, 65, 250_000, 1_500, 500000, 3, 16);
        AddSimple(BRE, "Jon", "Sigurdarson", "J. Sigurdarson", 24, "ISL", 178, 72, RIGHT, CM, CM_f, 59, 65, 200_000, 1_500, 400000, 3, 17);
        AddSimple(BRE, "Arnor", "Traustason", "Traustason", 32, "ISL", 180, 74, LEFT, LW, LW_f, 66, 67, 500_000, 3_000, 1000000, 2, 18);
        AddSimple(BRE, "Kristinn", "Sigurdsson", "K. Sigurdsson", 27, "ISL", 184, 76, RIGHT, ST, ST_f, 63, 66, 400_000, 2_500, 800000, 2, 19);
        AddSimple(BRE, "Vignir", "Olafsson", "Olafsson", 22, "ISL", 186, 78, RIGHT, GK, GK_f, 56, 66, 150_000, 1_000, 300000, 4, 12);
        AddSimple(BRE, "Gretar", "Steinsson", "Steinsson", 24, "ISL", 182, 76, RIGHT, ST, ST_f, 57, 64, 200_000, 1_200, 400000, 3, 20);
        AddSimple(BRE, "Albert", "Gunnarsson", "Gunnarsson", 28, "ISL", 176, 70, RIGHT, CM, CM_f, 61, 63, 250_000, 1_800, 500000, 2, 21);
        AddSimple(BRE, "Aron", "Elisson", "Elisson", 22, "ISL", 184, 76, RIGHT, CB, CB_f, 55, 65, 150_000, 1_000, 300000, 4, 22);
        AddSimple(BRE, "Orri", "Oskarsson", "Oskarsson", 21, "ISL", 188, 78, RIGHT, ST, ST_f, 60, 72, 500_000, 1_500, 1000000, 4, 23);

        // ── Team 163: ACF FIORENTINA ──────────────────────────────────────────
        int FIO = 163;

        AddSimple(FIO, "Pietro", "Terracciano", "Terracciano", 35, "ITA", 193, 88, RIGHT, GK, GK_f, 76, 76, 3_000_000, 30_000, 6000000, 1, 1);
        AddSimple(FIO, "David", "de Gea", "De Gea", 35, "ESP", 192, 84, RIGHT, GK, GK_f, 78, 78, 5_000_000, 50_000, 10000000, 1, 43);
        AddSimple(FIO, "Lorenzo", "Venuti", "Venuti", 30, "ITA", 175, 68, RIGHT, RB, RB_f, 72, 73, 3_000_000, 18_000, 6000000, 2, 2);
        AddSimple(FIO, "Lucas", "Martinez Quarta", "Martinez Quarta", 29, "ARG", 183, 80, RIGHT, CB, CB_f, 76, 77, 6_000_000, 25_000, 12000000, 3, 28);
        AddSimple(FIO, "Nikola", "Milenkovic", "Milenkovic", 27, "SRB", 195, 88, RIGHT, CB, CB_f, 77, 79, 10_000_000, 30_000, 20000000, 2, 4);
        AddSimple(FIO, "Cristiano", "Biraghi", "Biraghi", 32, "ITA", 180, 74, LEFT, LB, LB_f, 75, 75, 5_000_000, 28_000, 10000000, 2, 3);
        AddSimple(FIO, "Dodo", "Dodo", "Dodo", 26, "BRA", 177, 70, RIGHT, RB, RB_f, 73, 76, 6_000_000, 20_000, 12000000, 3, 34);
        AddSimple(FIO, "Rolando", "Mandragora", "Mandragora", 28, "ITA", 183, 78, LEFT, CDM, CDM_f, 74, 76, 5_000_000, 22_000, 10000000, 3, 38);
        AddSimple(FIO, "Sofyan", "Amrabat", "Amrabat", 28, "MAR", 185, 80, RIGHT, CDM, CDM_f, 76, 77, 8_000_000, 28_000, 16000000, 2, 33);
        AddSimple(FIO, "Antonin", "Barak", "Barak", 30, "CZE", 185, 76, RIGHT, CAM, CAM_f, 74, 75, 5_000_000, 22_000, 10000000, 2, 9);
        AddSimple(FIO, "Arthur", "Cabral", "Arthur Cabral", 27, "BRA", 186, 80, RIGHT, ST, ST_f, 75, 77, 7_000_000, 25_000, 14000000, 3, 99);
        AddSimple(FIO, "Abdelhamid", "Sabiri", "Sabiri", 28, "MAR", 182, 76, RIGHT, CAM, CAM_f, 72, 74, 4_000_000, 18_000, 8000000, 2, 10);
        AddSimple(FIO, "Giacomo", "Bonaventura", "Bonaventura", 35, "ITA", 180, 74, RIGHT, CM, CM_f, 77, 77, 4_000_000, 35_000, 8000000, 1, 5);
        AddSimple(FIO, "Jonathan", "Ikone", "Ikone", 27, "FRA", 175, 72, LEFT, RW, RW_f, 74, 76, 6_000_000, 25_000, 12000000, 2, 11);
        AddSimple(FIO, "Riccardo", "Sottil", "Sottil", 25, "ITA", 180, 72, RIGHT, LW, LW_f, 73, 78, 7_000_000, 20_000, 14000000, 3, 7);
        AddSimple(FIO, "Michael", "Kayode", "Kayode", 20, "ITA", 180, 72, RIGHT, RB, RB_f, 68, 80, 5_000_000, 8_000, 10000000, 4, 26);
        AddSimple(FIO, "Moise", "Kean", "Moise Kean", 25, "ITA", 182, 76, RIGHT, ST, ST_f, 75, 80, 10_000_000, 25_000, 20000000, 3, 20);
        AddSimple(FIO, "Christian", "Kouame", "Kouame", 27, "CIV", 180, 74, RIGHT, LW, LW_f, 72, 73, 3_500_000, 18_000, 7000000, 2, 8);
        AddSimple(FIO, "Riccardo", "Saponara", "Saponara", 33, "ITA", 184, 78, RIGHT, CAM, CAM_f, 70, 70, 1_500_000, 15_000, 3000000, 1, 18);
        AddSimple(FIO, "Alfred", "Duncan", "Duncan", 32, "GHA", 178, 72, LEFT, CM, CM_f, 71, 71, 2_000_000, 18_000, 4000000, 2, 32);

        // ── Team 164: FC DRITA ────────────────────────────────────────────────
        int DRI = 164;

        AddSimple(DRI, "Dren", "Fejzullahu", "Fejzullahu", 25, "KVX", 188, 80, RIGHT, GK, GK_f, 60, 65, 300_000, 1_200, 600000, 3, 1);
        AddSimple(DRI, "Gazmend", "Bytyqi", "Bytyqi", 28, "KVX", 185, 78, RIGHT, GK, GK_f, 58, 60, 200_000, 1_000, 400000, 2, 12);
        AddSimple(DRI, "Edrin", "Krasniqi", "E. Krasniqi", 26, "KVX", 180, 74, RIGHT, CB, CB_f, 57, 63, 200_000, 1_000, 400000, 3, 4);
        AddSimple(DRI, "Visar", "Bekaj", "Bekaj", 28, "KVX", 182, 76, RIGHT, CB, CB_f, 58, 60, 180_000, 1_000, 360000, 2, 5);
        AddSimple(DRI, "Shpend", "Hasani", "Hasani", 30, "KVX", 178, 72, RIGHT, CB, CB_f, 60, 61, 200_000, 1_200, 400000, 2, 3);
        AddSimple(DRI, "Arton", "Krasniqi", "A. Krasniqi", 24, "KVX", 176, 70, RIGHT, CM, CM_f, 59, 65, 250_000, 1_200, 500000, 3, 8);
        AddSimple(DRI, "Naim", "Gjoci", "Gjoci", 27, "KVX", 184, 78, RIGHT, CB, CB_f, 61, 64, 300_000, 1_500, 600000, 2, 6);
        AddSimple(DRI, "Besart", "Ibraimi", "Ibraimi", 33, "MKD", 180, 76, RIGHT, ST, ST_f, 63, 63, 300_000, 2_000, 600000, 1, 9);
        AddSimple(DRI, "Arben", "Hoxha", "Hoxha", 24, "KVX", 186, 80, RIGHT, CB, CB_f, 56, 64, 180_000, 800, 360000, 3, 15);
        AddSimple(DRI, "Yllson", "Dermaku", "Dermaku", 25, "KVX", 176, 70, RIGHT, CM, CM_f, 60, 66, 300_000, 1_500, 600000, 3, 10);
        AddSimple(DRI, "Enis", "Fazliu", "Fazliu", 22, "KVX", 175, 68, RIGHT, CM, CM_f, 56, 66, 200_000, 800, 400000, 4, 16);
        AddSimple(DRI, "Avni", "Pepa", "Pepa", 26, "KVX", 175, 68, LEFT, LB, LB_f, 58, 63, 200_000, 1_000, 400000, 3, 2);
        AddSimple(DRI, "Lirak", "Cela", "Cela", 24, "KVX", 176, 70, RIGHT, RB, RB_f, 57, 64, 200_000, 1_000, 400000, 3, 14);
        AddSimple(DRI, "Durim", "Rexhepi", "D. Rexhepi", 25, "KVX", 177, 70, RIGHT, CM, CM_f, 58, 64, 200_000, 1_000, 400000, 3, 7);
        AddSimple(DRI, "Fidan", "Aliti", "Aliti", 31, "KVX", 183, 78, LEFT, CB, CB_f, 62, 62, 250_000, 1_500, 500000, 2, 13);
        AddSimple(DRI, "Besfort", "Celina", "Celina", 22, "KVX", 174, 68, RIGHT, CAM, CAM_f, 60, 70, 400_000, 1_500, 800000, 4, 11);
        AddSimple(DRI, "Egzon", "Hajredini", "Hajredini", 24, "KVX", 180, 74, RIGHT, ST, ST_f, 58, 64, 200_000, 1_000, 400000, 3, 17);
        AddSimple(DRI, "Ismet", "Kryeziu", "Kryeziu", 25, "KVX", 176, 70, RIGHT, CM, CM_f, 57, 63, 200_000, 1_000, 400000, 3, 20);
        AddSimple(DRI, "Agon", "Mehmeti", "Mehmeti", 35, "KVX", 182, 78, RIGHT, ST, ST_f, 65, 65, 300_000, 2_500, 600000, 1, 18);
        AddSimple(DRI, "Albion", "Rrahmani", "Rrahmani", 23, "KVX", 178, 72, RIGHT, ST, ST_f, 59, 68, 350_000, 1_500, 700000, 4, 19);
        AddSimple(DRI, "Florent", "Rexhepi", "F. Rexhepi", 22, "KVX", 180, 74, RIGHT, ST, ST_f, 55, 65, 150_000, 800, 300000, 4, 21);
        AddSimple(DRI, "Milot", "Rashica", "Rashica", 28, "KVX", 178, 72, RIGHT, LW, LW_f, 68, 70, 1_200_000, 5_000, 2400000, 2, 22);

        // Conference League Teams 165-168
        // HAMRUN SPARTANS FC (165), AZ ALKMAAR (166), KF SHKENDIJA (167), JAGIELLONIA BIALYSTOK (168)

        // Constants
        // RIGHT=0, LEFT=1
        // Positions: GK=0, CB=1, LB=2, RB=3, CDM=4, CM=5, CAM=6, LW=7, RW=8, ST=9
        // Secondary pos suffix _f=100

        // ========== TEAM 165: HAMRUN SPARTANS FC ==========
        int HAM = 165;
        AddSimple(HAM, "Uros", "Durdevic", "Uros Durdevic", 32, "SRB", 194, 88, RIGHT, GK, 0, 64, 64, 120000, 3000, 240000, 2, 1);
        AddSimple(HAM, "Cain", "Attard", "Cain Attard", 30, "MLT", 183, 78, RIGHT, CB, CDM_f, 63, 63, 180000, 2500, 360000, 3, 5);
        AddSimple(HAM, "Matthew", "Guillaumier", "Matthew Guillaumier", 26, "MLT", 178, 72, RIGHT, CM, CDM_f, 65, 68, 350000, 3000, 700000, 4, 10);
        AddSimple(HAM, "Omar", "Borg", "Omar Borg", 26, "MLT", 185, 80, RIGHT, CB, LB_f, 64, 66, 200000, 2500, 400000, 3, 15);
        AddSimple(HAM, "Steve", "Borg", "Steve Borg", 37, "MLT", 186, 82, RIGHT, CB, 0, 66, 66, 150000, 3000, 300000, 1, 22);
        AddSimple(HAM, "Dunstan", "Vella", "Dunstan Vella", 27, "MLT", 177, 70, RIGHT, CM, 0, 63, 65, 250000, 2500, 500000, 3, 8);
        AddSimple(HAM, "Luke", "Montebello", "Luke Montebello", 29, "MLT", 180, 76, RIGHT, ST, 0, 64, 66, 300000, 3000, 600000, 3, 9);
        AddSimple(HAM, "Triston", "Caruana", "Triston Caruana", 33, "MLT", 182, 77, RIGHT, CB, CDM_f, 62, 62, 160000, 2500, 320000, 2, 4);
        AddSimple(HAM, "Andre", "Abela", "Andre Abela", 24, "MLT", 175, 70, RIGHT, CM, LW_f, 62, 68, 280000, 2500, 560000, 4, 18);
        AddSimple(HAM, "Njongo", "Priso", "Njongo Priso", 25, "CMR", 183, 78, RIGHT, ST, LW_f, 66, 70, 400000, 3500, 800000, 3, 27);
        AddSimple(HAM, "Jake", "Grech", "Jake Grech", 27, "MLT", 177, 71, LEFT, CM, LW_f, 63, 66, 220000, 2500, 440000, 3, 11);
        AddSimple(HAM, "Clyde", "Borg", "Clyde Borg", 26, "MLT", 184, 79, RIGHT, CB, CDM_f, 62, 64, 180000, 2200, 360000, 3, 20);
        AddSimple(HAM, "Omar", "Briffa", "Omar Briffa", 25, "MLT", 176, 70, RIGHT, CM, 0, 61, 66, 200000, 2300, 400000, 3, 14);
        AddSimple(HAM, "Terence", "Vella", "Terence Vella", 25, "MLT", 179, 74, RIGHT, ST, LW_f, 62, 66, 250000, 2500, 500000, 3, 17);
        AddSimple(HAM, "Keanu", "Sammut", "Keanu Sammut", 31, "MLT", 175, 69, RIGHT, CM, CDM_f, 63, 63, 200000, 2500, 400000, 2, 6);
        AddSimple(HAM, "Clayton", "Failla", "Clayton Failla", 39, "MLT", 172, 68, LEFT, LB, LW_f, 60, 60, 80000, 2000, 160000, 1, 3);
        AddSimple(HAM, "Justin", "Grioli", "Justin Grioli", 26, "MLT", 183, 78, RIGHT, CB, CDM_f, 61, 64, 150000, 2200, 300000, 3, 24);
        AddSimple(HAM, "Enrico", "Pepe", "Enrico Pepe", 27, "MLT", 188, 82, RIGHT, CB, 0, 64, 66, 200000, 2500, 400000, 3, 13);
        AddSimple(HAM, "Ryan", "Camilleri", "Ryan Camilleri", 37, "MLT", 185, 80, RIGHT, CB, CDM_f, 61, 61, 100000, 2200, 200000, 1, 21);
        AddSimple(HAM, "Pieter", "Bogert", "Pieter Bogert", 25, "NED", 190, 84, RIGHT, ST, 0, 67, 72, 500000, 4000, 1000000, 4, 19);
        AddSimple(HAM, "Thiago", "Ribeiro", "Thiago Ribeiro", 24, "BRA", 178, 74, RIGHT, ST, RW_f, 68, 74, 600000, 4500, 1200000, 4, 7);
        AddSimple(HAM, "Pedro", "Teixeira", "Pedro Teixeira", 22, "POR", 177, 71, RIGHT, CM, CAM_f, 66, 74, 550000, 4000, 1100000, 5, 23);

        // ========== TEAM 166: AZ ALKMAAR ==========
        int ALK = 166;
        AddSimple(ALK, "Hobie", "Verhulst", "Hobie Verhulst", 32, "NED", 190, 85, RIGHT, GK, 0, 70, 70, 500000, 5000, 1000000, 3, 1);
        AddSimple(ALK, "Rome-Jayden", "Owusu-Oduro", "Rome-Jayden Owusu-Oduro", 21, "NED", 189, 82, RIGHT, GK, 0, 66, 78, 800000, 3000, 1600000, 5, 12);
        AddSimple(ALK, "Yukinari", "Sugawara", "Yukinari Sugawara", 25, "Japan", 175, 68, RIGHT, RB, RW_f, 74, 77, 3500000, 12000, 7000000, 4, 2);
        AddSimple(ALK, "Maximiliano", "Wouter", "Maximiliano Wouter", 22, "NED", 184, 79, RIGHT, CB, CDM_f, 67, 76, 1200000, 5000, 2400000, 5, 3);
        AddSimple(ALK, "Sam", "Beukema", "Sam Beukema", 26, "NED", 187, 82, RIGHT, CB, 0, 75, 78, 4000000, 15000, 8000000, 4, 5);
        AddSimple(ALK, "Pantelis", "Hatzidiakos", "Pantelis Hatzidiakos", 28, "GRE", 185, 80, RIGHT, CB, CDM_f, 73, 75, 3000000, 12000, 6000000, 3, 8);
        AddSimple(ALK, "Peer", "Koopmeiners", "Peer Koopmeiners", 24, "NED", 183, 77, LEFT, CDM, CM_f, 72, 77, 3500000, 10000, 7000000, 5, 6);
        AddSimple(ALK, "Dani", "de Wit", "Dani de Wit", 27, "NED", 184, 78, RIGHT, CAM, ST_f, 74, 76, 3500000, 14000, 7000000, 3, 10);
        AddSimple(ALK, "Kasper", "Boogaard", "Kasper Boogaard", 24, "NED", 186, 81, RIGHT, CB, 0, 68, 74, 1500000, 6000, 3000000, 4, 4);
        AddSimple(ALK, "Kees", "Smit", "Kees Smit", 20, "NED", 176, 70, RIGHT, CM, 0, 65, 78, 1000000, 3500, 2000000, 5, 16);
        AddSimple(ALK, "Myron", "van Brederode", "Myron van Brederode", 22, "NED", 177, 72, RIGHT, LW, RW_f, 71, 78, 3000000, 8000, 6000000, 5, 11);
        AddSimple(ALK, "Vangelis", "Pavlidis", "Vangelis Pavlidis", 26, "GRE", 186, 80, RIGHT, ST, 0, 78, 80, 6000000, 20000, 12000000, 4, 9);
        AddSimple(ALK, "Thijs", "Oosting", "Thijs Oosting", 23, "NED", 180, 75, RIGHT, CM, LW_f, 68, 75, 1800000, 7000, 3600000, 5, 14);
        AddSimple(ALK, "Lasse", "Bohm", "Lasse Böhm", 21, "NED", 178, 72, LEFT, CM, LW_f, 67, 77, 1500000, 5000, 3000000, 5, 15);
        AddSimple(ALK, "Sven", "Mijnans", "Sven Mijnans", 25, "NED", 178, 72, RIGHT, CAM, CM_f, 73, 77, 3000000, 11000, 6000000, 4, 19);
        AddSimple(ALK, "Ibrahim", "Agyei", "Ibrahim Agyei", 23, "GHA", 182, 78, RIGHT, ST, RW_f, 68, 76, 2000000, 7000, 4000000, 5, 17);
        AddSimple(ALK, "Jordy", "Clasie", "Jordy Clasie", 33, "NED", 169, 66, RIGHT, CDM, CM_f, 72, 72, 1500000, 10000, 3000000, 2, 20);
        AddSimple(ALK, "Calvin", "Stengs", "Calvin Stengs", 26, "NED", 182, 77, LEFT, RW, CAM_f, 75, 77, 4500000, 16000, 9000000, 3, 7);
        AddSimple(ALK, "Jesper", "Karlsson", "Jesper Karlsson", 26, "SWE", 178, 72, RIGHT, LW, ST_f, 76, 78, 5000000, 18000, 10000000, 3, 18);
        AddSimple(ALK, "Troy", "Parrott", "Troy Parrott", 23, "IRL", 185, 80, RIGHT, ST, CAM_f, 70, 77, 3000000, 9000, 6000000, 5, 21);
        AddSimple(ALK, "Ernest", "Poku", "Ernest Poku", 21, "NED", 175, 70, RIGHT, RW, ST_f, 66, 77, 1500000, 4500, 3000000, 5, 22);
        AddSimple(ALK, "Mayckel", "Lahdo", "Mayckel Lahdo", 22, "SWE", 170, 66, LEFT, RW, LW_f, 68, 76, 2000000, 6000, 4000000, 5, 23);


        // ========== TEAM 167: KF SHKENDIJA ==========
        int SKN = 167;
        AddSimple(SKN, "Kostadin", "Zahov", "Kostadin Zahov", 30, "MKD", 188, 84, RIGHT, GK, 0, 63, 63, 150000, 3000, 300000, 3, 1);
        AddSimple(SKN, "Ardian", "Cuculi", "Ardian Cuculi", 27, "MKD", 185, 80, RIGHT, GK, 0, 60, 62, 100000, 2500, 200000, 3, 12);
        AddSimple(SKN, "Gledi", "Mici", "Gledi Mici", 33, "ALB", 184, 80, RIGHT, CB, 0, 64, 64, 180000, 3500, 360000, 2, 5);
        AddSimple(SKN, "Egzon", "Bejtullai", "Egzon Bejtullai", 30, "MKD", 180, 75, RIGHT, CB, 0, 61, 61, 120000, 2500, 240000, 2, 4);
        AddSimple(SKN, "Armend", "Alimi", "Armend Alimi", 36, "MKD", 178, 73, LEFT, LB, 0, 62, 62, 100000, 2500, 200000, 1, 3);
        AddSimple(SKN, "Besar", "Iseni", "Besar Iseni", 25, "MKD", 178, 74, RIGHT, RB, 0, 60, 64, 150000, 2000, 300000, 4, 2);
        AddSimple(SKN, "Ennur", "Totre", "Ennur Totre", 27, "ALB", 176, 72, RIGHT, RB, 0, 60, 62, 100000, 2000, 200000, 3, 20);
        AddSimple(SKN, "Ferhan", "Hasani", "Ferhan Hasani", 33, "MKD", 182, 76, RIGHT, CM, 0, 64, 64, 200000, 3000, 400000, 2, 8);
        AddSimple(SKN, "Bujar", "Muharem", "Bujar Muharem", 33, "MKD", 178, 72, RIGHT, CM, 0, 62, 62, 150000, 2500, 300000, 2, 10);
        AddSimple(SKN, "Qazim", "Laci", "Qazim Laci", 28, "ALB", 182, 76, RIGHT, CM, 0, 65, 67, 350000, 3500, 700000, 3, 14);
        AddSimple(SKN, "Abdoulaye", "Diallo", "Abdoulaye Diallo", 30, "SEN", 182, 76, RIGHT, CM, 0, 63, 63, 200000, 3000, 400000, 2, 25);
        AddSimple(SKN, "Valon", "Ahmedi", "Valon Ahmedi", 30, "MKD", 175, 70, RIGHT, LW, 0, 63, 65, 250000, 3000, 500000, 3, 7);
        AddSimple(SKN, "Ljupcho", "Doriev", "Ljupcho Doriev", 29, "MKD", 176, 72, RIGHT, LW, 0, 62, 64, 200000, 2500, 400000, 3, 11);
        AddSimple(SKN, "Anes", "Ibrahimi", "Anes Ibrahimi", 22, "MKD", 172, 68, RIGHT, LW, 0, 58, 66, 150000, 1500, 300000, 5, 18);
        AddSimple(SKN, "Rahim", "Ademi", "Rahim Ademi", 33, "MKD", 178, 74, RIGHT, RW, 0, 64, 64, 200000, 3000, 400000, 2, 27);
        AddSimple(SKN, "Florent", "Ramadani", "Florent Ramadani", 27, "ALB", 174, 70, RIGHT, RW, 0, 62, 64, 200000, 2500, 400000, 3, 17);
        AddSimple(SKN, "Mirsad", "Ramani", "Mirsad Ramani", 25, "MKD", 180, 76, RIGHT, ST, 0, 62, 66, 250000, 2500, 500000, 4, 9);
        AddSimple(SKN, "Luis", "Lenk", "Luis Lenk", 31, "GER", 188, 82, RIGHT, ST, 0, 64, 64, 200000, 3500, 400000, 2, 19);
        AddSimple(SKN, "Walid", "Noom", "Walid Noom", 29, "SEN", 185, 80, RIGHT, ST, 0, 63, 65, 250000, 3000, 500000, 3, 22);
        AddSimple(SKN, "Ermedin", "Demi", "Ermedin Demi", 25, "MKD", 180, 75, RIGHT, ST, 0, 60, 64, 180000, 2000, 360000, 4, 24);

        // ========== TEAM 168: JAGIELLONIA BIALYSTOK ==========
        int JAG = 168;
        AddSimple(JAG, "Zdenek", "Mika", "Zdenek Mika", 36, "CZE", 190, 86, RIGHT, GK, 0, 64, 64, 150000, 3000, 300000, 1, 1);
        AddSimple(JAG, "Maksymilian", "Krynicki", "Maksymilian Krynicki", 25, "POL", 188, 82, RIGHT, GK, 0, 62, 66, 200000, 2500, 400000, 4, 23);
        AddSimple(JAG, "Michal", "Pazdan", "Michal Pazdan", 37, "POL", 182, 78, RIGHT, CB, 0, 67, 67, 200000, 4000, 400000, 1, 4);
        AddSimple(JAG, "Igor", "Lewicki", "Igor Lewicki", 34, "POL", 185, 80, RIGHT, CB, 0, 65, 65, 150000, 3500, 300000, 2, 5);
        AddSimple(JAG, "Jakub", "Wojcicki", "Jakub Wojcicki", 36, "POL", 184, 80, RIGHT, CB, 0, 63, 63, 120000, 3000, 240000, 1, 17);
        AddSimple(JAG, "Bodvar", "Bodvarsson", "Bodvar Bodvarsson", 29, "ISL", 178, 74, LEFT, LB, 0, 64, 66, 250000, 3000, 500000, 3, 2);
        AddSimple(JAG, "Pawel", "Olszewski", "Pawel Olszewski", 26, "POL", 176, 72, RIGHT, RB, 0, 63, 67, 300000, 2500, 600000, 4, 21);
        AddSimple(JAG, "Karol", "Pieta", "Karol Pieta", 26, "POL", 178, 74, RIGHT, RB, 0, 60, 64, 150000, 2000, 300000, 4, 14);
        AddSimple(JAG, "Jakub", "Kubicki", "Jakub Kubicki", 25, "POL", 178, 72, RIGHT, CM, 0, 63, 67, 300000, 3000, 600000, 4, 8);
        AddSimple(JAG, "Mateusz", "Lewicki", "Mateusz Lewicki", 32, "POL", 180, 75, RIGHT, CDM, 0, 65, 65, 250000, 3500, 500000, 2, 7);
        AddSimple(JAG, "Tomasz", "Kupisz", "Tomasz Kupisz", 34, "POL", 178, 72, RIGHT, CAM, 0, 65, 65, 200000, 3500, 400000, 2, 10);
        AddSimple(JAG, "Dawid", "Czerw", "Dawid Czerw", 29, "POL", 176, 72, RIGHT, LW, 0, 64, 66, 250000, 3000, 500000, 3, 15);
        AddSimple(JAG, "Sebastian", "Szymanski", "Sebastian Szymanski", 26, "POL", 183, 76, RIGHT, LW, 0, 65, 68, 500000, 4000, 1000000, 4, 18);
        AddSimple(JAG, "Martin", "Pospisil", "Martin Pospisil", 33, "CZE", 178, 74, RIGHT, RW, 0, 66, 66, 300000, 3500, 600000, 2, 11);
        AddSimple(JAG, "Maciej", "Makuszewski", "Maciej Makuszewski", 34, "POL", 178, 72, RIGHT, RW, 0, 65, 65, 200000, 3500, 400000, 1, 16);
        AddSimple(JAG, "Cillian", "Sheridan", "Cillian Sheridan", 36, "IRL", 198, 86, RIGHT, ST, 0, 64, 64, 150000, 3000, 300000, 1, 9);
        AddSimple(JAG, "Karol", "Mackiewicz", "Karol Mackiewicz", 30, "POL", 184, 80, RIGHT, ST, 0, 64, 66, 300000, 3500, 600000, 3, 30);
        AddSimple(JAG, "Krzysztof", "Dragowski", "Krzysztof Dragowski", 27, "POL", 182, 76, RIGHT, ST, 0, 63, 67, 350000, 3000, 700000, 4, 19);
        AddSimple(JAG, "Jakub", "Jagiello", "Jakub Jagiello", 24, "POL", 176, 70, RIGHT, ST, 0, 60, 66, 200000, 2000, 400000, 5, 20);
        AddSimple(JAG, "Fedor", "Cernych", "Fedor Cernych", 33, "LTU", 180, 76, RIGHT, ST, 0, 65, 65, 200000, 3500, 400000, 2, 22);
        AddSimple(JAG, "Maksymilian", "Styszko", "Maksymilian Styszko", 22, "POL", 182, 76, RIGHT, CM, 0, 60, 68, 250000, 1500, 500000, 5, 24);
        // Conference League Teams - Part 6
        // Teams 169-172

        // Team 169: LECH POZNAN
        int LEC = 169;

        AddSimple(LEC, "Bartosz", "Mrozek", "B. Mrozek", 25, "POL", 190, 84, RIGHT, GK, 0, 66, 70, 500000, 5000, 1000000, 3, 1);
        AddSimple(LEC, "Artur", "Rudko", "A. Rudko", 27, "UKR", 189, 82, RIGHT, GK, 0, 65, 68, 400000, 4000, 800000, 2, 33);
        AddSimple(LEC, "Joel", "Pereira", "J. Pereira", 28, "POR", 187, 80, LEFT, GK, 0, 64, 66, 350000, 3500, 700000, 2, 12);
        AddSimple(LEC, "Antonio", "Milic", "A. Milic", 31, "CRO", 196, 88, RIGHT, CB, 0, 67, 69, 600000, 6000, 1200000, 2, 4);
        AddSimple(LEC, "Lubomir", "Satka", "L. Satka", 29, "SVK", 188, 83, RIGHT, CB, CDM_f, 69, 71, 750000, 7500, 1500000, 3, 37);
        AddSimple(LEC, "Pedro", "Rebocho", "P. Rebocho", 29, "POR", 173, 72, LEFT, LB, 0, 67, 68, 600000, 6000, 1200000, 2, 3);
        AddSimple(LEC, "Thomas", "Rogne", "T. Rogne", 35, "NOR", 193, 86, RIGHT, CB, 0, 65, 65, 350000, 3500, 700000, 1, 44);
        AddSimple(LEC, "Afonso", "Sousa", "A. Sousa", 25, "POR", 175, 70, RIGHT, CM, CAM_f, 70, 74, 1200000, 10000, 2400000, 3, 8);
        AddSimple(LEC, "Filip", "Marchwinski", "F. Marchwinski", 23, "POL", 186, 78, RIGHT, CAM, CM_f, 69, 75, 1100000, 9000, 2200000, 3, 11);
        AddSimple(LEC, "Mikael", "Ishak", "M. Ishak", 32, "SWE", 191, 85, RIGHT, ST, 0, 73, 73, 1500000, 12000, 3000000, 2, 9);
        AddSimple(LEC, "Dino", "Hotic", "D. Hotic", 29, "BIH", 180, 75, RIGHT, LW, RW_f, 67, 68, 550000, 5500, 1100000, 2, 17);
        AddSimple(LEC, "Kristoffer", "Velde", "K. Velde", 25, "NOR", 179, 73, RIGHT, LW, RW_f, 70, 74, 1100000, 9000, 2200000, 3, 7);
        AddSimple(LEC, "Jan", "Sykora", "J. Sykora", 31, "CZE", 175, 72, RIGHT, RW, LW_f, 68, 69, 600000, 6000, 1200000, 2, 23);
        AddSimple(LEC, "Radoslaw", "Murawski", "R. Murawski", 31, "POL", 178, 74, RIGHT, CDM, CM_f, 70, 71, 900000, 8000, 1800000, 2, 22);
        AddSimple(LEC, "Jakub", "Kaminski", "J. Kaminski", 28, "POL", 179, 73, RIGHT, LW, RW_f, 74, 75, 2000000, 15000, 4000000, 3, 10);
        AddSimple(LEC, "Ariel", "Mosor", "A. Mosor", 22, "POL", 186, 80, RIGHT, CB, 0, 61, 72, 400000, 4000, 800000, 3, 5);
        AddSimple(LEC, "Michal", "Skoras", "M. Skoras", 25, "POL", 175, 70, RIGHT, RW, LW_f, 69, 72, 800000, 7000, 1600000, 3, 21);
        AddSimple(LEC, "Jesper", "Karlsson", "J. Karlsson", 26, "SWE", 178, 72, RIGHT, LW, RW_f, 75, 77, 2500000, 18000, 5000000, 3, 19);
        AddSimple(LEC, "Adriel", "Ba Loua", "A. Ba Loua", 28, "CIV", 172, 69, RIGHT, RW, LW_f, 67, 69, 550000, 5500, 1100000, 2, 14);
        AddSimple(LEC, "Antonio", "Colak", "A. Colak", 31, "CRO", 188, 82, RIGHT, ST, 0, 72, 72, 1300000, 11000, 2600000, 2, 20);
        AddSimple(LEC, "Nika", "Kwekweskiri", "N. Kwekweskiri", 29, "GEO", 183, 76, RIGHT, CM, CDM_f, 66, 68, 500000, 5000, 1000000, 2, 30);
        AddSimple(LEC, "Yann", "Sandjo", "Y. Sandjo", 24, "CMR", 187, 81, RIGHT, CB, 0, 63, 70, 350000, 3500, 700000, 3, 6);

        // Team 170: LEGIA WARSZAWA
        int LGW = 170;

        AddSimple(LGW, "Kacper", "Tobiasz", "K. Tobiasz", 22, "POL", 189, 81, RIGHT, GK, 0, 68, 76, 1000000, 8000, 2000000, 4, 1);
        AddSimple(LGW, "Gabriel", "Kobylak", "G. Kobylak", 22, "POL", 190, 82, LEFT, GK, 0, 61, 70, 300000, 3000, 600000, 3, 21);
        AddSimple(LGW, "Artur", "Jedrzejczyk", "A. Jedrzejczyk", 37, "POL", 189, 83, RIGHT, RB, LB_f, 69, 69, 300000, 5000, 600000, 1, 55);
        AddSimple(LGW, "Maik", "Nawrocki", "M. Nawrocki", 24, "POL", 186, 80, RIGHT, CB, 0, 70, 76, 1400000, 10000, 2800000, 4, 3);
        AddSimple(LGW, "Josue", "Pesqueira", "Josue", 34, "POR", 175, 72, RIGHT, CAM, CM_f, 74, 74, 1200000, 12000, 2400000, 2, 99);
        AddSimple(LGW, "Mattias", "Johansson", "M. Johansson", 33, "SWE", 174, 70, RIGHT, RB, 0, 67, 67, 400000, 5000, 800000, 2, 20);
        AddSimple(LGW, "Pawel", "Wszolek", "P. Wszolek", 33, "POL", 183, 76, RIGHT, RW, LW_f, 70, 70, 600000, 7000, 1200000, 2, 13);
        AddSimple(LGW, "Mateusz", "Wieteska", "M. Wieteska", 28, "POL", 187, 82, RIGHT, CB, 0, 73, 75, 1800000, 13000, 3600000, 3, 4);
        AddSimple(LGW, "Rafal", "Augustyniak", "R. Augustyniak", 31, "POL", 184, 80, RIGHT, CB, CDM_f, 70, 71, 800000, 8000, 1600000, 2, 6);
        AddSimple(LGW, "Ernest", "Muci", "E. Muci", 24, "ALB", 183, 75, RIGHT, LW, ST_f, 72, 77, 2000000, 14000, 4000000, 3, 7);
        AddSimple(LGW, "Yuri", "Ribeiro", "Y. Ribeiro", 28, "POR", 173, 68, LEFT, LB, 0, 68, 70, 700000, 6500, 1400000, 3, 5);
        AddSimple(LGW, "Bartosz", "Kapustka", "B. Kapustka", 28, "POL", 177, 73, RIGHT, CAM, LW_f, 71, 73, 1100000, 10000, 2200000, 3, 67);
        AddSimple(LGW, "Tomas", "Pekhart", "T. Pekhart", 36, "CZE", 194, 90, RIGHT, ST, 0, 69, 69, 300000, 5000, 600000, 1, 9);
        AddSimple(LGW, "Marc", "Gual", "M. Gual", 29, "ESP", 183, 78, RIGHT, ST, 0, 70, 71, 900000, 8000, 1800000, 2, 10);
        AddSimple(LGW, "Blaz", "Kramer", "B. Kramer", 28, "SVN", 195, 88, RIGHT, ST, 0, 66, 68, 500000, 5500, 1000000, 2, 19);
        AddSimple(LGW, "Luquinhas", "Santos", "Luquinhas", 28, "BRA", 168, 65, RIGHT, CAM, LW_f, 69, 71, 800000, 7500, 1600000, 3, 87);
        AddSimple(LGW, "Jurgen", "Celhaka", "J. Celhaka", 30, "ALB", 182, 76, RIGHT, CDM, CM_f, 66, 68, 450000, 5000, 900000, 2, 8);
        AddSimple(LGW, "Ryoya", "Morishita", "R. Morishita", 24, "JPN", 175, 70, RIGHT, CM, CAM_f, 68, 74, 900000, 7500, 1800000, 3, 22);
        AddSimple(LGW, "Tomas", "Perez", "T. Perez", 23, "ESP", 178, 73, RIGHT, CM, 0, 63, 72, 500000, 4500, 1000000, 3, 18);
        AddSimple(LGW, "Juergen", "Elitim", "J. Elitim", 25, "COL", 175, 71, RIGHT, CM, CDM_f, 65, 71, 600000, 5500, 1200000, 3, 11);
        AddSimple(LGW, "Filip", "Mladenovic", "F. Mladenovic", 33, "SRB", 180, 74, LEFT, LB, RB_f, 67, 67, 350000, 4000, 700000, 1, 14);
        AddSimple(LGW, "Maciej", "Rosolek", "M. Rosolek", 25, "POL", 184, 79, RIGHT, ST, 0, 62, 69, 400000, 4000, 800000, 3, 17);

        // Team 171: RAKÓW CZĘSTOCHOWA
        int RAK = 171;

        AddSimple(RAK, "Kacper", "Tobiasz", "K. Tobiasz", 22, "POL", 189, 81, RIGHT, GK, 0, 66, 74, 800000, 7000, 1600000, 3, 1);
        AddSimple(RAK, "Vladan", "Kovacevic", "V. Kovacevic", 27, "BIH", 192, 85, RIGHT, GK, 0, 73, 76, 1800000, 12000, 3600000, 3, 12);
        AddSimple(RAK, "Zoran", "Arsenic", "Z. Arsenic", 27, "SRB", 185, 80, RIGHT, CB, LB_f, 68, 71, 700000, 6500, 1400000, 3, 5);
        AddSimple(RAK, "Wladyslaw", "Kalu", "W. Kalu", 23, "POL", 188, 82, RIGHT, CB, 0, 63, 72, 450000, 4000, 900000, 3, 3);
        AddSimple(RAK, "Ivan", "Petryak", "I. Petryak", 31, "UKR", 177, 72, RIGHT, LW, RW_f, 69, 69, 600000, 6000, 1200000, 2, 11);
        AddSimple(RAK, "Srdjan", "Grahovac", "S. Grahovac", 32, "BIH", 185, 79, RIGHT, CDM, CM_f, 70, 70, 700000, 7000, 1400000, 2, 66);
        AddSimple(RAK, "Milan", "Rundic", "M. Rundic", 33, "SRB", 186, 82, RIGHT, CB, 0, 70, 70, 600000, 6500, 1200000, 2, 15);
        AddSimple(RAK, "Bogdan", "Racovitan", "B. Racovitan", 23, "ROU", 186, 80, RIGHT, CB, 0, 67, 75, 900000, 7000, 1800000, 4, 4);
        AddSimple(RAK, "Fran", "Tudor", "F. Tudor", 29, "CRO", 175, 71, RIGHT, RB, 0, 71, 72, 1100000, 9500, 2200000, 3, 7);
        AddSimple(RAK, "Ante", "Crnac", "A. Crnac", 21, "CRO", 189, 80, RIGHT, ST, 0, 70, 80, 2000000, 10000, 4000000, 4, 9);
        AddSimple(RAK, "Ivi", "Lopez", "I. Lopez", 30, "ESP", 175, 70, RIGHT, LW, CAM_f, 74, 74, 1800000, 13000, 3600000, 2, 10);
        AddSimple(RAK, "Sime", "Vrsaljko", "S. Vrsaljko", 33, "CRO", 183, 76, RIGHT, RB, LB_f, 76, 76, 1500000, 15000, 3000000, 2, 13);
        AddSimple(RAK, "Ben", "Lederman", "B. Lederman", 25, "POL", 181, 75, RIGHT, CM, CAM_f, 67, 72, 700000, 6000, 1400000, 3, 14);
        AddSimple(RAK, "Patryk", "Kun", "P. Kun", 29, "POL", 180, 74, LEFT, LB, 0, 67, 68, 550000, 5500, 1100000, 2, 17);
        AddSimple(RAK, "Fabio", "Nunes", "F. Nunes", 28, "BRA", 173, 69, LEFT, LB, RB_f, 66, 68, 500000, 5000, 1000000, 2, 18);
        AddSimple(RAK, "Sonny", "Kittel", "S. Kittel", 32, "GER", 178, 73, RIGHT, CAM, CM_f, 72, 72, 1000000, 10000, 2000000, 2, 8);
        AddSimple(RAK, "Mathias", "Haarup", "M. Haarup", 23, "DEN", 186, 80, RIGHT, GK, 0, 60, 70, 250000, 2500, 500000, 3, 33);
        AddSimple(RAK, "Giovi", "Barros", "Giovi", 25, "BRA", 178, 72, RIGHT, CM, 0, 65, 71, 500000, 5000, 1000000, 3, 6);
        AddSimple(RAK, "Bartlomiej", "Pawlowski", "B. Pawlowski", 32, "POL", 178, 72, RIGHT, LW, RW_f, 71, 72, 800000, 8000, 1600000, 2, 19);
        AddSimple(RAK, "Sylvester", "Jasper", "S. Jasper", 23, "BUL", 183, 75, RIGHT, LW, RW_f, 64, 74, 550000, 4500, 1100000, 4, 21);
        AddSimple(RAK, "Makana", "Baku", "M. Baku", 26, "COD", 182, 76, RIGHT, RW, LW_f, 67, 70, 600000, 5500, 1200000, 3, 20);
        AddSimple(RAK, "Stratos", "Svarnas", "S. Svarnas", 27, "GRE", 183, 79, RIGHT, CB, CDM_f, 69, 72, 800000, 7000, 1600000, 3, 23);

        // Team 172: SHAMROCK ROVERS FC
        int SHM = 172;

        AddSimple(SHM, "Leon", "Pohls", "L. Pohls", 23, "GER", 191, 85, RIGHT, GK, 0, 61, 70, 200000, 2000, 400000, 2, 25);
        AddSimple(SHM, "Alan", "Mannus", "A. Mannus", 42, "IRL", 188, 84, RIGHT, GK, 0, 67, 67, 100000, 2500, 200000, 1, 1);
        AddSimple(SHM, "Roberto", "Lopes", "R. Lopes", 33, "IRL", 185, 80, RIGHT, CB, 0, 69, 69, 250000, 3500, 500000, 2, 4);
        AddSimple(SHM, "Sean", "Hoare", "S. Hoare", 31, "IRL", 186, 81, RIGHT, CB, 0, 65, 66, 200000, 2800, 400000, 2, 5);
        AddSimple(SHM, "Daniel", "Cleary", "D. Cleary", 31, "IRL", 183, 78, RIGHT, CB, 0, 64, 66, 180000, 2500, 360000, 2, 15);
        AddSimple(SHM, "Liam", "Scales", "L. Scales", 26, "IRL", 190, 82, LEFT, CB, LB_f, 67, 72, 500000, 5000, 1000000, 3, 6);
        AddSimple(SHM, "Cameron", "McJannet", "C. McJannet", 26, "IRL", 180, 74, LEFT, LB, CB_f, 64, 69, 250000, 2800, 500000, 2, 3);
        AddSimple(SHM, "Andy", "Lyons", "A. Lyons", 24, "IRL", 175, 70, RIGHT, RB, 0, 65, 72, 400000, 3500, 800000, 3, 2);
        AddSimple(SHM, "Richie", "Towell", "R. Towell", 33, "IRL", 173, 70, RIGHT, CM, CAM_f, 66, 66, 200000, 3000, 400000, 1, 8);
        AddSimple(SHM, "Gary", "O'Neill", "G. O'Neill", 29, "IRL", 180, 75, RIGHT, CM, CDM_f, 65, 67, 250000, 3000, 500000, 2, 16);
        AddSimple(SHM, "Jack", "Byrne", "J. Byrne", 29, "IRL", 175, 71, RIGHT, CAM, CM_f, 70, 72, 600000, 6000, 1200000, 2, 10);
        AddSimple(SHM, "Ronan", "Finn", "R. Finn", 37, "IRL", 178, 73, RIGHT, CM, 0, 64, 64, 120000, 2200, 240000, 1, 7);
        AddSimple(SHM, "Pico", "Lopes", "Pico", 34, "ESP", 172, 68, RIGHT, CAM, CM_f, 67, 67, 200000, 3000, 400000, 1, 20);
        AddSimple(SHM, "Aaron", "McEneff", "A. McEneff", 29, "IRL", 180, 74, RIGHT, CM, 0, 64, 66, 200000, 2500, 400000, 2, 18);
        AddSimple(SHM, "Danny", "Mandroiu", "D. Mandroiu", 26, "IRL", 175, 70, RIGHT, CAM, LW_f, 65, 70, 350000, 3500, 700000, 2, 11);
        AddSimple(SHM, "Aidomo", "Emakhu", "A. Emakhu", 20, "IRL", 180, 75, RIGHT, ST, LW_f, 60, 76, 500000, 2500, 1000000, 4, 21);
        AddSimple(SHM, "Rory", "Gaffney", "R. Gaffney", 35, "IRL", 183, 78, RIGHT, ST, 0, 68, 68, 200000, 3000, 400000, 1, 9);
        AddSimple(SHM, "Barry", "Coffey", "B. Coffey", 26, "IRL", 178, 73, RIGHT, CM, 0, 63, 68, 220000, 2500, 440000, 2, 26);
        AddSimple(SHM, "Aaron", "Greene", "A. Greene", 34, "IRL", 178, 74, RIGHT, ST, 0, 63, 63, 150000, 2500, 300000, 1, 14);
        AddSimple(SHM, "Graham", "Burke", "G. Burke", 31, "IRL", 176, 72, RIGHT, ST, 0, 66, 67, 250000, 3000, 500000, 2, 24);
        AddSimple(SHM, "Paddy", "Barrett", "P. Barrett", 31, "IRL", 185, 80, RIGHT, CB, 0, 64, 65, 180000, 2500, 360000, 2, 22);
        AddSimple(SHM, "Lee", "Grace", "L. Grace", 32, "IRL", 188, 82, RIGHT, CB, 0, 64, 65, 170000, 2500, 340000, 2, 13);

        // Conference League Teams 7 - Auto-generated
        // Teams: Shelbourne (173), Craiova (174), Aberdeen (175), Slovan Bratislava (176)

        // ========================================
        // Team 173: SHELBOURNE FC
        int SHE = 173;
        // ========================================
        AddSimple(SHE, "Brendan", "Clarke", "Brendan Clarke", 39, "IRL", 188, 85, RIGHT, GK, 0, 63, 63, 120000, 2000, 240000, 1, 1);
        AddSimple(SHE, "Conor", "Kearns", "Conor Kearns", 27, "IRL", 185, 78, RIGHT, GK, 0, 66, 70, 250000, 3000, 500000, 3, 25);
        AddSimple(SHE, "Mark", "Coyle", "Mark Coyle", 28, "IRL", 180, 76, RIGHT, CB, RB_f, 66, 68, 280000, 3200, 560000, 2, 4);
        AddSimple(SHE, "Shane", "Griffin", "Shane Griffin", 27, "IRL", 175, 70, LEFT, LB, 0, 65, 67, 220000, 2800, 440000, 2, 3);
        AddSimple(SHE, "Kevin", "O'Connor", "Kevin O'Connor", 26, "IRL", 178, 72, RIGHT, RB, 0, 67, 70, 300000, 3500, 600000, 3, 2);
        AddSimple(SHE, "Ciaran", "Kilduff", "Ciaran Kilduff", 36, "IRL", 185, 80, RIGHT, ST, 0, 64, 64, 130000, 2200, 260000, 1, 9);
        AddSimple(SHE, "Mark", "Coyle", "Mark Coyle", 28, "IRL", 180, 76, RIGHT, CM, CDM_f, 67, 69, 300000, 3400, 600000, 2, 8);
        AddSimple(SHE, "Cameron", "Dummigan", "Cameron Dummigan", 28, "IRL", 175, 71, RIGHT, RB, RM_f, 66, 68, 260000, 3000, 520000, 2, 22);
        AddSimple(SHE, "Jack", "Moylan", "Jack Moylan", 23, "IRL", 178, 72, RIGHT, CM, CAM_f, 68, 75, 500000, 4500, 1000000, 4, 10);
        AddSimple(SHE, "Aidan", "Keena", "Aidan Keena", 26, "IRL", 180, 78, RIGHT, ST, RW_f, 69, 73, 450000, 4200, 900000, 3, 11);
        AddSimple(SHE, "Darragh", "Burns", "Darragh Burns", 23, "IRL", 173, 68, LEFT, LW, RW_f, 70, 76, 600000, 5000, 1200000, 4, 7);
        AddSimple(SHE, "Eoin", "Doyle", "Eoin Doyle", 37, "IRL", 183, 80, RIGHT, ST, 0, 63, 63, 100000, 1800, 200000, 1, 14);
        AddSimple(SHE, "Matty", "Smith", "Matty Smith", 26, "IRL", 178, 75, RIGHT, ST, LW_f, 66, 69, 280000, 3200, 560000, 2, 19);
        AddSimple(SHE, "Jamie", "Malone", "Jamie Malone", 25, "IRL", 180, 75, RIGHT, CB, LB_f, 64, 68, 240000, 2800, 480000, 3, 5);
        AddSimple(SHE, "Shannon", "Noel", "Shannon Noel", 24, "IRL", 182, 77, RIGHT, CDM, CM_f, 65, 69, 250000, 2800, 500000, 3, 6);
        AddSimple(SHE, "Ben", "Duffy", "Ben Duffy", 22, "IRL", 185, 78, RIGHT, CB, LB_f, 60, 68, 180000, 2000, 360000, 4, 15);
        AddSimple(SHE, "Jack", "O'Brien", "Jack O'Brien", 23, "IRL", 176, 71, RIGHT, CM, RB_f, 64, 69, 220000, 2500, 440000, 3, 12);
        AddSimple(SHE, "Luke", "Heary", "Luke Heary", 22, "IRL", 174, 68, RIGHT, LB, LM_f, 62, 69, 200000, 2200, 400000, 4, 18);
        AddSimple(SHE, "Adam", "McDonnell", "Adam McDonnell", 26, "IRL", 178, 73, RIGHT, CM, CDM_f, 63, 66, 220000, 2800, 440000, 2, 16);
        AddSimple(SHE, "John", "Mountney", "John Mountney", 31, "IRL", 180, 76, RIGHT, RB, CDM_f, 64, 65, 200000, 2800, 400000, 2, 13);
        AddSimple(SHE, "Kyle", "Robinson", "Kyle Robinson", 22, "IRL", 177, 72, RIGHT, ST, LW_f, 60, 68, 200000, 2200, 400000, 4, 20);
        AddSimple(SHE, "James", "Talbot", "James Talbot", 27, "IRL", 183, 78, RIGHT, GK, 0, 65, 70, 300000, 3200, 600000, 3, 26);

        // ========================================
        // Team 174: FCV UNIVERSITATEA CRAIOVA
        int CRA = 174;
        // ========================================
        AddSimple(CRA, "Daniel", "Banciu", "Daniel Banciu", 29, "ROU", 188, 84, RIGHT, GK, 0, 65, 67, 300000, 3500, 600000, 2, 1);
        AddSimple(CRA, "Cristian", "Balan", "Cristian Balan", 28, "ROU", 185, 80, RIGHT, GK, 0, 63, 66, 250000, 3000, 500000, 2, 12);
        AddSimple(CRA, "Stefan", "Vladu", "Stefan Vladu", 26, "ROU", 182, 77, RIGHT, CB, RB_f, 66, 69, 350000, 3500, 700000, 3, 4);
        AddSimple(CRA, "Mihai", "Balan", "Mihai Balan", 28, "ROU", 184, 80, RIGHT, CB, CDM_f, 65, 68, 320000, 3200, 640000, 2, 5);
        AddSimple(CRA, "Raul", "Triska", "Raul Triska", 30, "ROU", 180, 75, RIGHT, LB, LM_f, 64, 66, 280000, 3000, 560000, 2, 3);
        AddSimple(CRA, "Daniel", "Dan", "Daniel Dan", 27, "ROU", 176, 72, RIGHT, RB, RM_f, 65, 68, 300000, 3200, 600000, 3, 2);
        AddSimple(CRA, "Alexandru", "Cicaldau", "Alexandru Cicaldau", 27, "ROU", 180, 75, RIGHT, CM, CAM_f, 69, 72, 600000, 5000, 1200000, 3, 8);
        AddSimple(CRA, "Daniel", "Pirvulescu", "Daniel Pirvulescu", 28, "ROU", 178, 73, RIGHT, CM, CDM_f, 66, 68, 350000, 3500, 700000, 2, 6);
        AddSimple(CRA, "Andrei", "Ivan", "Andrei Ivan", 27, "ROU", 182, 77, RIGHT, ST, RW_f, 68, 71, 500000, 4500, 1000000, 3, 9);
        AddSimple(CRA, "Claudiu", "Bumba", "Claudiu Bumba", 30, "ROU", 175, 70, RIGHT, CAM, RW_f, 67, 68, 400000, 4000, 800000, 2, 10);
        AddSimple(CRA, "George", "Sima", "George Sima", 28, "ROU", 177, 72, RIGHT, LW, RW_f, 66, 68, 350000, 3500, 700000, 2, 11);
        AddSimple(CRA, "Andrei", "Popescu", "Andrei Popescu", 25, "ROU", 184, 80, RIGHT, CB, LB_f, 63, 67, 280000, 2800, 560000, 3, 14);
        AddSimple(CRA, "Nicolae", "Iancu", "Nicolae Iancu", 29, "ROU", 179, 74, RIGHT, CM, CAM_f, 65, 66, 300000, 3200, 600000, 2, 15);
        AddSimple(CRA, "Andrei", "Cordea", "Andrei Cordea", 25, "ROU", 176, 70, RIGHT, LW, ST_f, 67, 72, 500000, 4000, 1000000, 3, 7);
        AddSimple(CRA, "Marian", "Aioani", "Marian Aioani", 22, "ROU", 190, 85, RIGHT, GK, 0, 63, 72, 350000, 2500, 700000, 4, 24);
        AddSimple(CRA, "Cristian", "Manea", "Cristian Manea", 27, "ROU", 183, 79, RIGHT, RB, CB_f, 65, 67, 350000, 3500, 700000, 2, 16);
        AddSimple(CRA, "Enes", "Sipovic", "Enes Sipovic", 33, "ROU", 186, 82, RIGHT, CB, CDM_f, 66, 66, 280000, 3000, 560000, 1, 13);
        AddSimple(CRA, "Ionut", "Vlad", "Ionut Vlad", 23, "ROU", 177, 72, RIGHT, CM, LM_f, 63, 69, 300000, 2800, 600000, 4, 17);
        AddSimple(CRA, "Dragos", "Andrei", "Dragos Andrei", 24, "ROU", 180, 76, RIGHT, ST, LW_f, 64, 69, 320000, 3000, 640000, 3, 18);
        AddSimple(CRA, "Valentin", "Costache", "Valentin Costache", 27, "ROU", 181, 77, RIGHT, LW, ST_f, 65, 67, 350000, 3500, 700000, 2, 19);
        AddSimple(CRA, "Darius", "Olariu", "Darius Olariu", 25, "ROU", 175, 70, RIGHT, RB, RM_f, 63, 66, 250000, 2800, 500000, 3, 20);
        AddSimple(CRA, "Eduard", "Florescu", "Eduard Florescu", 27, "ROU", 182, 78, RIGHT, ST, RW_f, 66, 68, 380000, 3500, 760000, 2, 21);

        // ========================================
        // Team 175: ABERDEEN FC
        int ABE = 175;
        // ========================================
        AddSimple(ABE, "Kieran", "Watson", "Kieran Watson", 28, "SCO", 185, 80, RIGHT, GK, 0, 64, 66, 250000, 3000, 500000, 2, 1);
        AddSimple(ABE, "Ross", "Gallacher", "Ross Gallacher", 27, "SCO", 183, 77, RIGHT, GK, 0, 62, 65, 220000, 2800, 440000, 2, 12);
        AddSimple(ABE, "Andrew", "Considine", "Andrew Considine", 37, "SCO", 186, 82, RIGHT, CB, LB_f, 70, 70, 300000, 5000, 600000, 1, 4);
        AddSimple(ABE, "Scott", "McKenna", "Scott McKenna", 28, "SCO", 188, 85, RIGHT, CB, 0, 72, 74, 1500000, 10000, 3000000, 2, 5);
        AddSimple(ABE, "Graeme", "Shinnie", "Graeme Shinnie", 33, "SCO", 177, 72, LEFT, LB, CM_f, 71, 71, 700000, 7000, 1400000, 2, 3);
        AddSimple(ABE, "Jonny", "Hayes", "Jonny Hayes", 38, "IRL", 175, 70, RIGHT, RB, RM_f, 67, 67, 150000, 3500, 300000, 1, 2);
        AddSimple(ABE, "Ryan", "Hedges", "Ryan Hedges", 29, "WAL", 182, 76, RIGHT, RW, LW_f, 69, 71, 800000, 6000, 1600000, 2, 7);
        AddSimple(ABE, "Lewis", "Ferguson", "Lewis Ferguson", 25, "SCO", 180, 75, RIGHT, CM, CAM_f, 73, 77, 3000000, 12000, 6000000, 3, 8);
        AddSimple(ABE, "Connor", "McLennan", "Connor McLennan", 25, "SCO", 176, 71, RIGHT, LW, RW_f, 66, 70, 500000, 4000, 1000000, 3, 11);
        AddSimple(ABE, "Sam", "Cosgrove", "Sam Cosgrove", 28, "ENG", 188, 84, RIGHT, ST, 0, 68, 70, 700000, 6000, 1400000, 2, 9);
        AddSimple(ABE, "Dean", "Campbell", "Dean Campbell", 24, "SCO", 175, 70, RIGHT, CM, CDM_f, 63, 68, 300000, 3000, 600000, 3, 14);
        AddSimple(ABE, "Mikey", "Devlin", "Mikey Devlin", 31, "SCO", 186, 80, RIGHT, CB, RB_f, 65, 66, 250000, 3000, 500000, 2, 6);
        AddSimple(ABE, "Niall", "McGinn", "Niall McGinn", 38, "NIR", 178, 72, RIGHT, LW, RW_f, 65, 65, 120000, 3000, 240000, 1, 17);
        AddSimple(ABE, "Bruce", "Anderson", "Bruce Anderson", 26, "SCO", 180, 76, RIGHT, ST, LW_f, 63, 66, 300000, 3000, 600000, 2, 18);
        AddSimple(ABE, "Ethan", "Ross", "Ethan Ross", 23, "SCO", 177, 73, RIGHT, CM, CAM_f, 62, 68, 250000, 2500, 500000, 3, 15);
        AddSimple(ABE, "Jack", "Mackenzie", "Jack Mackenzie", 24, "SCO", 176, 70, LEFT, LB, LM_f, 64, 69, 350000, 3000, 700000, 3, 16);
        AddSimple(ABE, "Scott", "Wright", "Scott Wright", 27, "SCO", 177, 72, RIGHT, RW, LW_f, 68, 70, 600000, 5000, 1200000, 2, 10);
        AddSimple(ABE, "Calvin", "Ramsay", "Calvin Ramsay", 22, "SCO", 174, 68, RIGHT, RB, RM_f, 65, 74, 1000000, 4000, 2000000, 4, 19);
        AddSimple(ABE, "Fletcher", "Boyd", "Fletcher Boyd", 22, "SCO", 182, 76, RIGHT, ST, RW_f, 60, 68, 200000, 2200, 400000, 4, 20);
        AddSimple(ABE, "Dylan", "McGeouch", "Dylan McGeouch", 31, "SCO", 175, 70, RIGHT, CM, CDM_f, 63, 63, 200000, 3000, 400000, 1, 21);
        AddSimple(ABE, "Mark", "Reynolds", "Mark Reynolds", 37, "SCO", 183, 78, RIGHT, CB, LB_f, 63, 63, 100000, 2500, 200000, 1, 22);
        AddSimple(ABE, "Tom", "Cerny", "Tom Cerny", 27, "CZE", 186, 82, RIGHT, CB, RB_f, 58, 66, 200000, 2200, 400000, 3, 23);

        // ========================================
        // Team 176: SK SLOVAN BRATISLAVA
        int SLB = 176;
        // ========================================
        AddSimple(SLB, "Michal", "Sulla", "Michal Sulla", 33, "SVK", 188, 84, RIGHT, GK, 0, 69, 69, 300000, 4000, 600000, 2, 1);
        AddSimple(SLB, "Adam", "Hlousek", "Adam Hlousek", 36, "CZE", 180, 75, LEFT, LB, LM_f, 67, 67, 200000, 3500, 400000, 1, 2);
        AddSimple(SLB, "Vernon", "De Marco", "Vernon De Marco", 32, "SVK", 183, 78, RIGHT, CB, CDM_f, 68, 68, 350000, 4500, 700000, 2, 4);
        AddSimple(SLB, "Vasil", "Bozhikov", "Vasil Bozhikov", 37, "BUL", 186, 82, RIGHT, CB, RB_f, 66, 66, 180000, 3500, 360000, 1, 5);
        AddSimple(SLB, "Kenan", "Bajric", "Kenan Bajric", 28, "BIH", 185, 80, RIGHT, CB, 0, 67, 68, 400000, 4500, 800000, 2, 14);
        AddSimple(SLB, "Lucas", "Lovat", "Lucas Lovat", 28, "ARG", 178, 73, RIGHT, RB, RM_f, 69, 71, 600000, 5000, 1200000, 2, 3);
        AddSimple(SLB, "Juraj", "Kucka", "Juraj Kucka", 38, "SVK", 184, 80, RIGHT, CM, CDM_f, 72, 72, 500000, 7000, 1000000, 1, 6);
        AddSimple(SLB, "Andre", "Green", "Andre Green", 26, "ENG", 182, 76, RIGHT, LW, RW_f, 67, 70, 500000, 4500, 1000000, 2, 7);
        AddSimple(SLB, "Aleksandar", "Cavric", "Aleksandar Cavric", 31, "SRB", 189, 85, RIGHT, ST, RW_f, 70, 70, 600000, 5500, 1200000, 2, 9);
        AddSimple(SLB, "David", "Strelec", "David Strelec", 24, "SVK", 185, 80, RIGHT, ST, LW_f, 67, 73, 1000000, 6000, 2000000, 3, 10);
        AddSimple(SLB, "Marko", "Savic", "Marko Savic", 25, "SRB", 178, 73, RIGHT, CM, CAM_f, 67, 71, 600000, 4500, 1200000, 3, 11);
        AddSimple(SLB, "Jakub", "Kadlec", "Jakub Kadlec", 24, "CZE", 182, 77, RIGHT, CB, RB_f, 64, 69, 350000, 3000, 700000, 3, 15);
        AddSimple(SLB, "Michal", "Sipov", "Michal Sipov", 29, "SVK", 177, 72, RIGHT, RB, RW_f, 64, 66, 280000, 3000, 560000, 2, 16);
        AddSimple(SLB, "Artem", "Sukhotskyi", "Artem Sukhotskyi", 30, "UKR", 180, 75, LEFT, LB, LM_f, 64, 65, 250000, 3000, 500000, 2, 17);
        AddSimple(SLB, "Martin", "Chrien", "Martin Chrien", 29, "SVK", 182, 78, RIGHT, CAM, CM_f, 66, 67, 400000, 4000, 800000, 2, 8);
        AddSimple(SLB, "Robin", "Krizan", "Robin Krizan", 22, "SVK", 183, 77, RIGHT, CB, RB_f, 60, 69, 250000, 2200, 500000, 4, 18);
        AddSimple(SLB, "Miroslav", "Kacer", "Miroslav Kacer", 28, "SVK", 175, 70, RIGHT, CM, CDM_f, 63, 66, 300000, 3000, 600000, 2, 19);
        AddSimple(SLB, "Milan", "Kvasnic", "Milan Kvasnic", 24, "SVK", 178, 72, RIGHT, LW, RW_f, 63, 68, 320000, 2800, 640000, 3, 20);
        AddSimple(SLB, "Alen", "Ozbold", "Alen Ozbold", 23, "SVN", 175, 70, RIGHT, LB, RB_f, 61, 67, 250000, 2200, 500000, 4, 21);
        AddSimple(SLB, "Filip", "Lichy", "Filip Lichy", 23, "SVK", 180, 75, RIGHT, CDM, CM_f, 63, 69, 300000, 2800, 600000, 3, 22);
        AddSimple(SLB, "Michal", "Tomasek", "Michal Tomasek", 28, "CZE", 186, 82, RIGHT, ST, 0, 66, 68, 450000, 4000, 900000, 2, 13);
        AddSimple(SLB, "Eric", "Ramirez", "Eric Ramirez", 28, "SVK", 184, 80, RIGHT, ST, RW_f, 65, 67, 350000, 3500, 700000, 2, 12);



        // Conference League teams — generated data
        // Constants: RIGHT=0, LEFT=1
        // Positions: GK=0 CB=1 LB=2 RB=3 CDM=4 CM=5 CAM=6 LW=7 RW=8 ST=9
        // Secondary pos suffix _f = position + 100

        // ── Team 177: NK CELJE ────────────────────────────────────────────
        int CLJ = 177;
        AddSimple(CLJ, "Matevz", "Vidovsek", "M. Vidovsek", 26, "SVN", 190, 84, LEFT, GK, 0, 68, 72, 700_000, 4_000, 1400000, 3, 1);
        AddSimple(CLJ, "Dmitrijs", "Zolnerovics", "D. Zolnerovics", 22, "LVA", 188, 80, LEFT, GK, 0, 60, 74, 300_000, 2_000, 600000, 3, 12);
        AddSimple(CLJ, "Gregor", "Sikosek", "G. Sikosek", 30, "SVN", 176, 72, LEFT, LB, 0, 70, 70, 1_200_000, 8_000, 2400000, 2, 23);
        AddSimple(CLJ, "Zan", "Karnicnik", "Z. Karnicnik", 29, "SVN", 180, 74, RIGHT, RB, 0, 72, 72, 2_000_000, 10_000, 4000000, 3, 22);
        AddSimple(CLJ, "Tjas", "Begic", "T. Begic", 24, "SVN", 185, 80, LEFT, CB, 0, 67, 75, 1_500_000, 7_000, 3000000, 4, 4);
        AddSimple(CLJ, "Urban", "Svara", "U. Svara", 24, "SVN", 183, 78, LEFT, CB, LB_f, 65, 73, 1_000_000, 6_000, 2000000, 3, 5);
        AddSimple(CLJ, "Uros", "Celcer", "U. Celcer", 21, "SVN", 186, 82, RIGHT, CB, 0, 62, 76, 800_000, 4_000, 1600000, 4, 15);
        AddSimple(CLJ, "Filip", "Dangubic", "F. Dangubic", 26, "SVN", 185, 80, RIGHT, ST, 0, 70, 73, 1_800_000, 9_000, 3600000, 3, 9);
        AddSimple(CLJ, "Timotej", "Dodlek", "T. Dodlek", 25, "SVN", 178, 74, RIGHT, CM, 0, 68, 72, 1_400_000, 7_000, 2800000, 3, 8);
        AddSimple(CLJ, "Dario", "Krekovic", "D. Krekovic", 23, "SVN", 180, 75, RIGHT, CM, 0, 66, 75, 1_200_000, 6_000, 2400000, 4, 16);
        AddSimple(CLJ, "Aljosa", "Matko", "A. Matko", 24, "SVN", 182, 78, RIGHT, ST, 0, 69, 75, 1_600_000, 8_000, 3200000, 4, 19);
        AddSimple(CLJ, "Svit", "Seslar", "S. Seslar", 23, "SVN", 177, 73, RIGHT, CM, 0, 67, 74, 1_300_000, 6_500, 2600000, 3, 21);
        AddSimple(CLJ, "Arjan", "Sukaj", "A. Sukaj", 24, "ALB", 180, 76, RIGHT, ST, 0, 65, 72, 1_000_000, 5_000, 2000000, 3, 18);
        AddSimple(CLJ, "Nino", "Zugelj", "N. Zugelj", 25, "SVN", 175, 70, LEFT, RW, 0, 71, 74, 2_500_000, 12_000, 5000000, 3, 7);
        AddSimple(CLJ, "Ziga", "Kous", "Z. Kous", 22, "SVN", 184, 79, LEFT, CB, RB_f, 63, 74, 900_000, 4_500, 1800000, 4, 28);
        AddSimple(CLJ, "Luka", "Bobicanec", "L. Bobicanec", 21, "SVN", 176, 72, RIGHT, CM, 0, 64, 78, 1_200_000, 5_500, 2400000, 4, 14);
        AddSimple(CLJ, "Zan", "Vipotnik", "Z. Vipotnik", 23, "SVN", 186, 84, RIGHT, ST, 0, 73, 80, 4_000_000, 18_000, 8000000, 4, 11);
        AddSimple(CLJ, "Stefan", "Marusic", "S. Marusic", 26, "SVN", 188, 82, RIGHT, CB, 0, 69, 72, 1_500_000, 7_500, 3000000, 3, 6);
        AddSimple(CLJ, "Ivan", "Brnovic", "I. Brnovic", 22, "MNE", 179, 74, RIGHT, CM, 0, 65, 76, 1_100_000, 5_500, 2200000, 4, 20);
        AddSimple(CLJ, "Nicolas", "Mees", "N. Mees", 22, "BEL", 184, 80, RIGHT, ST, 0, 63, 75, 900_000, 4_500, 1800000, 3, 29);
        AddSimple(CLJ, "Gian Luca", "Grubesic", "G. Grubesic", 20, "SVN", 185, 81, LEFT, CB, 0, 58, 76, 600_000, 3_000, 1200000, 4, 25);

        // ── Team 178: BK HACKEN ───────────────────────────────────────────
        int HAC = 178;
        AddSimple(HAC, "Peter", "Abrahamsson", "P. Abrahamsson", 38, "SWE", 190, 85, LEFT, GK, 0, 72, 72, 800_000, 10_000, 1600000, 2, 1);
        AddSimple(HAC, "Moses", "Varmods", "M. Varmods", 24, "SWE", 185, 80, LEFT, CB, LB_f, 66, 72, 1_200_000, 6_000, 2400000, 3, 4);
        AddSimple(HAC, "Gustav", "Berggren", "G. Berggren", 26, "SWE", 182, 76, RIGHT, CM, 0, 73, 74, 3_500_000, 16_000, 7000000, 3, 8);
        AddSimple(HAC, "Godfred", "Donsah", "G. Donsah", 28, "GHA", 176, 73, RIGHT, CM, 0, 70, 70, 2_500_000, 12_000, 5000000, 2, 6);
        AddSimple(HAC, "Andreas", "Granqvist", "A. Granqvist", 40, "SWE", 192, 86, RIGHT, CB, 0, 74, 74, 1_500_000, 18_000, 3000000, 1, 3);
        AddSimple(HAC, "Carlos", "Moros Gracia", "C. Moros Gracia", 31, "ESP", 184, 80, RIGHT, CB, 0, 68, 68, 1_500_000, 8_000, 3000000, 2, 5);
        AddSimple(HAC, "Alejandro", "Bedoya", "A. Bedoya", 38, "USA", 178, 74, RIGHT, CM, CDM_f, 70, 70, 1_000_000, 12_000, 2000000, 1, 10);
        AddSimple(HAC, "Alexander", "Jeremejeff", "A. Jeremejeff", 33, "SWE", 188, 84, RIGHT, ST, 0, 72, 72, 3_000_000, 15_000, 6000000, 2, 9);
        AddSimple(HAC, "Armin", "Gigovic", "A. Gigovic", 23, "SWE", 183, 77, LEFT, CM, 0, 67, 76, 1_600_000, 7_000, 3200000, 4, 20);
        AddSimple(HAC, "Lawrence", "Ennali", "L. Ennali", 22, "SWE", 170, 65, RIGHT, LW, 0, 66, 77, 1_400_000, 6_000, 2800000, 4, 11);
        AddSimple(HAC, "Sam", "Nicholson", "S. Nicholson", 30, "SCO", 175, 70, LEFT, LW, 0, 68, 68, 1_800_000, 9_000, 3600000, 2, 17);
        AddSimple(HAC, "Isaac", "Kiese Thelin", "I. Kiese Thelin", 33, "SWE", 189, 85, RIGHT, ST, 0, 74, 74, 4_000_000, 20_000, 8000000, 2, 19);
        AddSimple(HAC, "Robin", "Soder", "R. Soder", 34, "SWE", 176, 73, RIGHT, ST, 0, 70, 70, 2_000_000, 12_000, 4000000, 1, 23);
        AddSimple(HAC, "Deniz", "Hummet", "D. Hummet", 26, "TUR", 185, 81, RIGHT, ST, 0, 69, 72, 2_200_000, 10_000, 4400000, 3, 14);
        AddSimple(HAC, "Viktor", "Lundberg", "V. Lundberg", 34, "SWE", 184, 80, RIGHT, ST, 0, 68, 68, 1_500_000, 9_000, 3000000, 1, 16);
        AddSimple(HAC, "Jakob", "Hard af Segerstad", "J. Hard af Segerstad", 25, "SWE", 179, 74, LEFT, CM, 0, 65, 72, 1_000_000, 5_000, 2000000, 3, 24);
        AddSimple(HAC, "Tinotenda", "Kadewere", "T. Kadewere", 31, "ZIM", 183, 78, RIGHT, ST, 0, 73, 73, 3_500_000, 17_000, 7000000, 2, 12);
        AddSimple(HAC, "Marcus", "Danielson", "M. Danielson", 36, "SWE", 192, 87, RIGHT, CB, 0, 75, 75, 2_500_000, 20_000, 5000000, 2, 2);
        AddSimple(HAC, "Mohanad", "Jeahze", "M. Jeahze", 28, "SWE", 172, 68, LEFT, LB, 0, 70, 71, 2_000_000, 10_000, 4000000, 3, 18);
        AddSimple(HAC, "Aron", "Sele", "A. Sele", 22, "SWE", 184, 79, RIGHT, CB, LB_f, 62, 75, 800_000, 4_000, 1600000, 4, 29);
        AddSimple(HAC, "Muamer", "Tankovic", "M. Tankovic", 30, "SWE", 176, 72, RIGHT, LW, 0, 72, 72, 3_000_000, 15_000, 6000000, 2, 7);
        AddSimple(HAC, "Sebastian", "Rajalakso", "S. Rajalakso", 35, "SWE", 179, 74, RIGHT, CM, 0, 66, 66, 800_000, 6_000, 1600000, 1, 21);

        // ── Team 179: FC LAUSANNE-SPORT ────────────────────────────────────
        int LAU = 179;
        AddSimple(LAU, "Mory", "Kone", "M. Kone", 30, "CIV", 186, 82, LEFT, GK, 0, 70, 70, 1_500_000, 9_000, 3000000, 2, 1);
        AddSimple(LAU, "Leo", "Dussenne", "L. Dussenne", 33, "BEL", 191, 86, LEFT, CB, 0, 72, 72, 2_500_000, 14_000, 5000000, 2, 4);
        AddSimple(LAU, "Danijel", "Subasic", "D. Subasic", 40, "CRO", 191, 84, RIGHT, GK, 0, 76, 76, 2_000_000, 25_000, 4000000, 1, 12);
        AddSimple(LAU, "Kevin", "Fickentscher", "K. Fickentscher", 36, "SUI", 185, 80, LEFT, GK, 0, 68, 68, 500_000, 6_000, 1000000, 1, 25);
        AddSimple(LAU, "Anel", "Husic", "A. Husic", 24, "SUI", 186, 82, RIGHT, CB, 0, 66, 74, 1_200_000, 6_000, 2400000, 3, 5);
        AddSimple(LAU, "Theo", "Baumann", "T. Baumann", 22, "SUI", 180, 76, LEFT, CB, LB_f, 62, 75, 800_000, 4_000, 1600000, 4, 33);
        AddSimple(LAU, "Justin", "Che", "J. Che", 21, "USA", 178, 72, RIGHT, RB, 0, 65, 78, 1_500_000, 6_000, 3000000, 4, 22);
        AddSimple(LAU, "Jordan", "Lotomba", "J. Lotomba", 26, "SUI", 178, 73, RIGHT, RB, 0, 73, 74, 4_000_000, 18_000, 8000000, 3, 2);
        AddSimple(LAU, "Alvyn", "Sanches", "A. Sanches", 22, "SUI", 175, 70, RIGHT, CM, 0, 67, 78, 2_000_000, 8_000, 4000000, 4, 10);
        AddSimple(LAU, "Filip", "Stojilkovic", "F. Stojilkovic", 25, "SUI", 185, 80, RIGHT, ST, 0, 70, 74, 2_500_000, 11_000, 5000000, 3, 9);
        AddSimple(LAU, "Theo", "Nkuba", "T. Nkuba", 24, "BEL", 172, 68, LEFT, RW, 0, 68, 74, 1_800_000, 8_000, 3600000, 3, 11);
        AddSimple(LAU, "Maciej", "Rybus", "M. Rybus", 35, "POL", 174, 70, LEFT, LB, 0, 71, 71, 1_500_000, 12_000, 3000000, 1, 3);
        AddSimple(LAU, "Aurele", "Amenda", "A. Amenda", 22, "SUI", 188, 84, RIGHT, CB, 0, 66, 78, 1_600_000, 7_000, 3200000, 4, 6);
        AddSimple(LAU, "Ousmane", "Doumbia", "O. Doumbia", 33, "CIV", 180, 76, RIGHT, CDM, 0, 72, 72, 2_000_000, 13_000, 4000000, 2, 8);
        AddSimple(LAU, "Numa", "Lavanchy", "N. Lavanchy", 31, "SUI", 175, 72, RIGHT, RB, 0, 69, 69, 1_500_000, 8_000, 3000000, 2, 14);
        AddSimple(LAU, "Joel", "Monteiro", "J. Monteiro", 25, "SUI", 182, 78, RIGHT, ST, 0, 68, 73, 1_800_000, 9_000, 3600000, 3, 17);
        AddSimple(LAU, "Sebastien", "Wuthrich", "S. Wuthrich", 34, "SUI", 186, 82, RIGHT, CB, 0, 70, 70, 1_500_000, 9_000, 3000000, 2, 26);
        AddSimple(LAU, "Moussa", "Sissako", "M. Sissako", 24, "MLI", 184, 78, RIGHT, CM, 0, 67, 74, 1_400_000, 6_500, 2800000, 3, 20);
        AddSimple(LAU, "Marvin", "Tshibuabua", "M. Tshibuabua", 24, "COD", 185, 81, LEFT, CB, 0, 64, 73, 1_000_000, 5_000, 2000000, 3, 15);
        AddSimple(LAU, "Stjepan", "Kukuruzovic", "S. Kukuruzovic", 26, "CRO", 179, 74, RIGHT, CM, 0, 68, 71, 1_500_000, 7_500, 3000000, 2, 7);
        AddSimple(LAU, "Turpal", "Mindubaev", "T. Mindubaev", 22, "RUS", 173, 67, RIGHT, LW, 0, 63, 76, 1_000_000, 5_000, 2000000, 4, 18);
        AddSimple(LAU, "Simone", "Rapp", "S. Rapp", 32, "SUI", 190, 86, RIGHT, ST, 0, 69, 69, 1_200_000, 8_000, 2400000, 2, 19);

        // ── Team 180: SAMSUNSPOR ──────────────────────────────────────────
        int SAMS = 180;
        AddSimple(SAMS, "Kerem", "Karaer", "K. Karaer", 29, "TUR", 188, 82, RIGHT, GK, 0, 68, 70, 1_000_000, 6_000, 2000000, 2, 1);
        AddSimple(SAMS, "Fatih", "Ozturk", "F. Ozturk", 38, "TUR", 185, 80, RIGHT, GK, 0, 67, 67, 400_000, 5_000, 800000, 1, 25);
        AddSimple(SAMS, "Oussama", "Haddadi", "O. Haddadi", 33, "TUN", 175, 72, LEFT, LB, 0, 72, 72, 2_000_000, 12_000, 4000000, 2, 3);
        AddSimple(SAMS, "Filip", "Stojilkovic", "F. Stojilkovic", 25, "SUI", 185, 80, RIGHT, ST, 0, 70, 74, 2_500_000, 11_000, 5000000, 3, 19);
        AddSimple(SAMS, "Oghenekaro", "Etebo", "O. Etebo", 29, "NGA", 175, 74, RIGHT, CM, CDM_f, 74, 74, 4_500_000, 20_000, 9000000, 2, 10);
        AddSimple(SAMS, "Rodrigo", "Becao", "R. Becao", 28, "BRA", 191, 85, RIGHT, CB, 0, 75, 76, 6_000_000, 25_000, 12000000, 3, 4);
        AddSimple(SAMS, "Olarenwaju", "Kayode", "O. Kayode", 31, "NGA", 177, 74, RIGHT, ST, 0, 72, 72, 3_000_000, 15_000, 6000000, 2, 9);
        AddSimple(SAMS, "Emre", "Demir", "E. Demir", 21, "TUR", 175, 68, RIGHT, CAM, 0, 66, 80, 2_500_000, 8_000, 5000000, 4, 11);
        AddSimple(SAMS, "Dylan", "Bronn", "D. Bronn", 30, "TUN", 184, 80, RIGHT, CB, 0, 73, 73, 3_500_000, 16_000, 7000000, 2, 5);
        AddSimple(SAMS, "Matias", "Catalan", "M. Catalan", 32, "ARG", 178, 76, RIGHT, CB, 0, 71, 71, 2_000_000, 11_000, 4000000, 2, 6);
        AddSimple(SAMS, "Alvaro", "Vadillo", "A. Vadillo", 32, "ESP", 174, 70, LEFT, LW, 0, 70, 70, 2_000_000, 11_000, 4000000, 2, 17);
        AddSimple(SAMS, "Julien", "Ngoy", "J. Ngoy", 27, "BEL", 182, 78, RIGHT, ST, 0, 68, 71, 1_800_000, 9_000, 3600000, 2, 29);
        AddSimple(SAMS, "Amir", "Hadziahmetovic", "A. Hadziahmetovic", 28, "BIH", 180, 75, RIGHT, CM, 0, 74, 74, 4_500_000, 20_000, 9000000, 3, 8);
        AddSimple(SAMS, "Bryan", "Souza", "B. Souza", 25, "BRA", 176, 72, RIGHT, RB, 0, 67, 73, 1_500_000, 7_000, 3000000, 3, 22);
        AddSimple(SAMS, "Mamadou", "Coulibaly", "M. Coulibaly", 26, "SEN", 184, 80, RIGHT, CDM, 0, 69, 73, 2_000_000, 9_000, 4000000, 3, 6);
        AddSimple(SAMS, "Sabin", "Merino", "S. Merino", 33, "ESP", 177, 73, LEFT, LW, 0, 70, 70, 1_800_000, 10_000, 3600000, 2, 27);
        AddSimple(SAMS, "Guido", "Carrillo", "G. Carrillo", 34, "ARG", 188, 84, RIGHT, ST, 0, 73, 73, 3_000_000, 18_000, 6000000, 2, 18);
        AddSimple(SAMS, "Hakan", "Ozmert", "H. Ozmert", 26, "TUR", 176, 72, RIGHT, CM, 0, 67, 71, 1_400_000, 7_000, 2800000, 3, 20);
        AddSimple(SAMS, "Mustafa", "Kapi", "M. Kapi", 22, "TUR", 183, 78, RIGHT, CB, RB_f, 62, 75, 800_000, 4_000, 1600000, 4, 15);
        AddSimple(SAMS, "Serkan", "Kirintili", "S. Kirintili", 22, "TUR", 178, 73, RIGHT, CM, 0, 64, 76, 1_000_000, 5_000, 2000000, 4, 21);




        // ══════════════════════════════════════════════════════════════
        // FC Dynamo Kyiv (TeamId=181)
        // ══════════════════════════════════════════════════════════════
        int DYK = 181;
        AddSimple(DYK, "Heorhiy", "Bushchan", "Bushchan", 31, "UKR", 185, 82, RIGHT, GK, 0, 78, 80, 5000000, 5000, 10000000, 3, 1);
        AddSimple(DYK, "Denys", "Boyko", "Boyko", 37, "UKR", 194, 88, RIGHT, GK, 0, 72, 72, 800000, 3000, 1600000, 1, 12);
        AddSimple(DYK, "Vitaliy", "Mykolenko", "Mykolenko", 26, "UKR", 180, 76, LEFT, LB, RB_f, 80, 84, 15000000, 6000, 30000000, 5, 44);
        AddSimple(DYK, "Tomas", "Kalas", "Kalas", 32, "CZE", 184, 80, RIGHT, CB, 0, 76, 78, 3500000, 5000, 7000000, 3, 2);
        AddSimple(DYK, "Eduard", "Sobol", "Sobol", 30, "UKR", 180, 74, LEFT, LB, 0, 74, 78, 3000000, 4000, 6000000, 4, 13);
        AddSimple(DYK, "Serhiy", "Sydorchuk", "Sydorchuk", 34, "UKR", 184, 78, RIGHT, CM, CDM_f, 76, 78, 2500000, 5000, 5000000, 3, 5);
        AddSimple(DYK, "Oleksandr", "Karavayev", "Karavayev", 33, "UKR", 176, 72, RIGHT, RB, RW_f, 74, 76, 2200000, 4000, 4400000, 3, 8);
        AddSimple(DYK, "Mykola", "Shaparenko", "Shaparenko", 26, "UKR", 178, 74, RIGHT, CM, CAM_f, 80, 84, 12000000, 6000, 25000000, 5, 14);
        AddSimple(DYK, "Oleksandr", "Andriyevsky", "Andriyevsky", 31, "UKR", 182, 76, RIGHT, CM, 0, 72, 74, 1500000, 3000, 3000000, 3, 15);
        AddSimple(DYK, "Carlos", "de Pena", "De Pena", 33, "URU", 178, 74, LEFT, LW, CM_f, 76, 78, 2800000, 5000, 5600000, 3, 10);
        AddSimple(DYK, "Vladyslav", "Supryaha", "Supryaha", 25, "UKR", 184, 76, RIGHT, ST, RW_f, 74, 80, 5000000, 4000, 10000000, 5, 22);
        AddSimple(DYK, "Benjamin", "Verbic", "Verbic", 31, "SVN", 180, 74, RIGHT, LW, RW_f, 76, 78, 4000000, 5000, 8000000, 3, 7);
        AddSimple(DYK, "Viktor", "Tsyhankov", "Tsyhankov", 27, "UKR", 178, 72, LEFT, RW, CAM_f, 82, 86, 25000000, 8000, 50000000, 5, 15);
        AddSimple(DYK, "Vitaliy", "Buyalskyi", "Buyalskyi", 32, "UKR", 180, 76, RIGHT, CAM, CM_f, 78, 80, 5000000, 6000, 10000000, 4, 29);
        AddSimple(DYK, "Denys", "Harmash", "Harmash", 35, "UKR", 184, 78, RIGHT, CM, CDM_f, 72, 74, 1200000, 3000, 2400000, 2, 19);
        AddSimple(DYK, "Artem", "Shabanov", "Shabanov", 33, "UKR", 187, 82, RIGHT, CB, 0, 72, 74, 1000000, 3000, 2000000, 3, 26);
        AddSimple(DYK, "Milos", "Pantic", "Pantic", 28, "SRB", 180, 76, RIGHT, CM, 0, 72, 76, 1500000, 3000, 3000000, 4, 20);
        AddSimple(DYK, "Illya", "Zabarnyi", "Zabarnyi", 22, "UKR", 186, 80, RIGHT, CB, 0, 78, 86, 18000000, 4000, 35000000, 5, 3);
        AddSimple(DYK, "Artem", "Besedin", "Besedin", 29, "UKR", 186, 80, RIGHT, ST, 0, 74, 78, 2500000, 4000, 5000000, 4, 30);
        AddSimple(DYK, "Volodymyr", "Brazhko", "Brazhko", 23, "UKR", 180, 74, RIGHT, CM, CDM_f, 72, 80, 5000000, 2000, 10000000, 5, 17);
        AddSimple(DYK, "Dmytro", "Kryskiv", "Kryskiv", 24, "UKR", 178, 72, RIGHT, CM, 0, 70, 78, 3000000, 2000, 6000000, 5, 22);
        AddSimple(DYK, "Mohammad", "Mohebi", "Mohebi", 26, "IRN", 178, 72, RIGHT, LW, RW_f, 74, 80, 5000000, 3000, 10000000, 5, 11);

        // ══════════════════════════════════════════════════════════════
        // FC Shakhtar Donetsk (TeamId=182)
        // ══════════════════════════════════════════════════════════════
        int SHK = 182;
        AddSimple(SHK, "Dmytro", "Riznyk", "Riznyk", 25, "UKR", 188, 82, RIGHT, GK, 0, 76, 82, 7000000, 4000, 14000000, 5, 1);
        AddSimple(SHK, "Andriy", "Pyatov", "Pyatov", 41, "UKR", 190, 86, RIGHT, GK, 0, 74, 74, 500000, 3000, 1000000, 1, 30);
        AddSimple(SHK, "Dodo", "", "Dodo", 26, "BRA", 178, 72, RIGHT, RB, RW_f, 80, 84, 18000000, 6000, 35000000, 5, 2);
        AddSimple(SHK, "Mykola", "Matviyenko", "Matviyenko", 29, "UKR", 182, 76, LEFT, CB, LB_f, 80, 82, 12000000, 6000, 25000000, 4, 22);
        AddSimple(SHK, "Serhiy", "Kryvtsov", "Kryvtsov", 34, "UKR", 186, 82, RIGHT, CB, 0, 76, 78, 3000000, 5000, 6000000, 3, 4);
        AddSimple(SHK, "Marcos", "Antonio", "Marcos A.", 25, "BRA", 178, 72, RIGHT, CM, CDM_f, 78, 84, 15000000, 5000, 30000000, 5, 8);
        AddSimple(SHK, "Taras", "Stepanenko", "Stepanenko", 35, "UKR", 184, 78, RIGHT, CDM, CM_f, 78, 80, 2500000, 5000, 5000000, 3, 6);
        AddSimple(SHK, "Alan", "Patrick", "Alan P.", 34, "BRA", 178, 72, RIGHT, CAM, CM_f, 78, 80, 4000000, 6000, 8000000, 3, 7);
        AddSimple(SHK, "Marlos", "", "Marlos", 37, "UKR", 176, 70, LEFT, RW, CAM_f, 76, 78, 1500000, 5000, 3000000, 2, 11);
        AddSimple(SHK, "Fernando", "", "Fernando", 26, "BRA", 178, 74, RIGHT, ST, RW_f, 78, 82, 8000000, 5000, 16000000, 5, 19);
        AddSimple(SHK, "Tete", "", "Tete", 25, "BRA", 176, 68, LEFT, RW, LW_f, 80, 84, 20000000, 6000, 40000000, 5, 18);
        AddSimple(SHK, "Artur", "", "Artur", 24, "BRA", 176, 72, RIGHT, CAM, RW_f, 78, 84, 14000000, 4000, 28000000, 5, 10);
        AddSimple(SHK, "Pedrinho", "", "Pedrinho", 27, "BRA", 176, 70, RIGHT, CAM, RW_f, 78, 80, 10000000, 6000, 20000000, 4, 20);
        AddSimple(SHK, "Maycon", "", "Maycon", 27, "BRA", 178, 74, RIGHT, CM, CDM_f, 76, 80, 8000000, 5000, 16000000, 4, 15);
        AddSimple(SHK, "Lassina", "Traore", "Traore", 24, "BFA", 188, 84, RIGHT, ST, 0, 76, 82, 10000000, 4000, 20000000, 5, 14);
        AddSimple(SHK, "Danylo", "Sikan", "Sikan", 24, "UKR", 184, 78, RIGHT, ST, LW_f, 74, 80, 6000000, 3000, 12000000, 5, 9);
        AddSimple(SHK, "Valeriy", "Bondar", "Bondar", 25, "UKR", 186, 80, RIGHT, CB, 0, 74, 80, 4000000, 3000, 8000000, 5, 13);
        AddSimple(SHK, "Heorhiy", "Sudakov", "Sudakov", 22, "UKR", 178, 74, RIGHT, CAM, CM_f, 78, 86, 20000000, 4000, 40000000, 5, 21);
        AddSimple(SHK, "Vasyl", "Kravets", "Kravets", 27, "UKR", 180, 74, LEFT, LB, 0, 72, 78, 2000000, 3000, 4000000, 4, 16);
        AddSimple(SHK, "Bohdan", "Mykhaylichenko", "Mykhaylichenko", 28, "UKR", 182, 76, LEFT, LB, 0, 72, 78, 2000000, 3000, 4000000, 4, 26);
        AddSimple(SHK, "Dmytro", "Kryskiv", "Kryskiv", 24, "UKR", 178, 72, RIGHT, CM, 0, 70, 78, 3000000, 2000, 6000000, 5, 17);
        AddSimple(SHK, "Mohammad", "Mohebi", "Mohebi", 26, "IRN", 178, 72, RIGHT, LW, RW_f, 74, 80, 5000000, 3000, 10000000, 5, 77);

        // ══════════════════════════════════════════════════════════════
        // Stade de Reims (TeamId=183)
        // ══════════════════════════════════════════════════════════════
        int REI = 183;
        AddSimple(REI, "Yehvann", "Diouf", "Diouf", 25, "FRA", 185, 78, RIGHT, GK, 0, 78, 82, 10000000, 3000, 20000000, 5, 1);
        AddSimple(REI, "Patrick", "Pentz", "Pentz", 28, "AUT", 184, 80, RIGHT, GK, 0, 76, 80, 5000000, 3000, 10000000, 5, 28);
        AddSimple(REI, "Emmanuel", "Agbadou", "Agbadou", 28, "CIV", 188, 82, RIGHT, CB, 0, 76, 80, 8000000, 4000, 16000000, 5, 7);
        AddSimple(REI, "Ghislain", "Konan", "Konan", 29, "CIV", 180, 74, LEFT, LB, 0, 76, 80, 6000000, 4000, 12000000, 4, 3);
        AddSimple(REI, "Wout", "Faes", "Faes", 27, "BEL", 186, 80, RIGHT, CB, CDM_f, 78, 82, 12000000, 5000, 25000000, 5, 6);
        AddSimple(REI, "Yunis", "Abdelhamid", "Abdelhamid", 37, "MAR", 190, 84, LEFT, CB, 0, 76, 78, 1000000, 5000, 2000000, 1, 5);
        AddSimple(REI, "Thomas", "Foket", "Foket", 30, "BEL", 180, 74, RIGHT, RB, 0, 76, 80, 5000000, 5000, 10000000, 4, 32);
        AddSimple(REI, "Xavier", "Chavalerin", "Chavalerin", 34, "FRA", 182, 76, LEFT, CM, CDM_f, 74, 76, 2000000, 4000, 4000000, 2, 10);
        AddSimple(REI, "Dion", "Lopy", "Lopy", 23, "SEN", 186, 80, RIGHT, CDM, 0, 74, 82, 10000000, 3000, 20000000, 5, 18);
        AddSimple(REI, "Jens", "Cajuste", "Cajuste", 25, "SWE", 182, 76, RIGHT, CM, CDM_f, 76, 82, 10000000, 4000, 20000000, 5, 8);
        AddSimple(REI, "Kamory", "Doumbia", "Doumbia", 22, "MLI", 180, 74, RIGHT, CM, CAM_f, 74, 84, 12000000, 2000, 25000000, 5, 20);
        AddSimple(REI, "Arber", "Zeneli", "Zeneli", 30, "KVX", 176, 72, RIGHT, LW, RW_f, 76, 80, 5000000, 5000, 10000000, 4, 11);
        AddSimple(REI, "El", "Bilal Touré", "E. Touré", 23, "MLI", 186, 80, RIGHT, ST, LW_f, 78, 84, 15000000, 3000, 30000000, 5, 9);
        AddSimple(REI, "Mitchell", "van Bergen", "Van Bergen", 25, "NED", 174, 70, RIGHT, RW, LW_f, 74, 80, 5000000, 3000, 10000000, 5, 17);
        AddSimple(REI, "Junya", "Ito", "J. Ito", 32, "JPN", 176, 68, RIGHT, RW, 0, 80, 82, 8000000, 6000, 16000000, 4, 14);
        AddSimple(REI, "Patrick", "Hornemann", "Hornemann", 22, "GER", 178, 72, RIGHT, CM, 0, 68, 80, 4000000, 1000, 8000000, 5, 22);
        AddSimple(REI, "Folarin", "Balogun", "Balogun", 23, "USA", 180, 76, RIGHT, ST, 0, 78, 86, 25000000, 4000, 50000000, 5, 29);
        AddSimple(REI, "Cheick", "Keita", "Keita", 25, "MLI", 180, 74, LEFT, LB, 0, 74, 80, 6000000, 3000, 12000000, 5, 4);
        AddSimple(REI, "Maxime", "Busi", "Busi", 26, "BEL", 182, 76, RIGHT, RB, 0, 74, 80, 5000000, 3000, 10000000, 5, 24);
        AddSimple(REI, "Alexis", "Flips", "Flips", 25, "FRA", 176, 72, RIGHT, CAM, 0, 74, 80, 5000000, 3000, 10000000, 5, 21);
        AddSimple(REI, "Martin", "Hongla", "Hongla", 27, "CMR", 184, 78, RIGHT, CM, CDM_f, 76, 80, 6000000, 4000, 12000000, 5, 23);
        AddSimple(REI, "Emmanuel", "Leka", "Leka", 22, "ALB", 176, 72, RIGHT, ST, 0, 68, 80, 4000000, 1000, 8000000, 5, 19);
        db.InsertAll(players);
        Debug.Log($"[DB] {players.Count} jugadores insertados.");
    }
}
