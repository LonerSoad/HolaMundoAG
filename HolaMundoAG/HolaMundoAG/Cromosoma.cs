using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;

namespace HolaMundoAG
{
    class Cromosoma : ChromosomeBase
    {
        char []genes;//cada cromosoma , tendra un gen por cada letra de la palabra 
        public Cromosoma(int length) : base(length)
        {
     
        }

        public override IChromosome CreateNew()
        {
            throw new NotImplementedException();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            throw new NotImplementedException();
        }
    }
}
