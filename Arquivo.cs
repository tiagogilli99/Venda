using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using VendaService.DTO;

namespace VendaService
{
    public class Arquivo
    {
        /// <summary>
        /// Método de processamento do arquivo
        /// </summary>
        /// <param name="textFile"></param>
        public void ProcessaArquivo(string textFile)
        {
            var linhas = textFile.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            var vendedores = CarregaVendedores(linhas.Where(t => t.Contains("001")));
            var clientes = CarregaClientes(linhas.Where(t => t.Contains("002")));
            var vendas = CarregaVendas(linhas.Where(t => t.Contains("003")));

            GeraArquivo(vendedores, clientes, vendas);
        }

        /// <summary>
        /// Método de geração do arquivo
        /// </summary>
        /// <param name="vendedores"></param>
        /// <param name="clientes"></param>
        /// <param name="vendas"></param>
        private void GeraArquivo(List<Vendedor> vendedores, List<Cliente> clientes, List<DTO.Venda> vendas)
        {
            StreamWriter file;
            List<MaiorValor> maiorValor = new List<MaiorValor>();

            string arquivo = ConfigurationManager.AppSettings["WritePath"] + "Venda.txt";

            file = File.CreateText(arquivo);

            //Quantidade de Clientes
            file.WriteLine("Quantidade de Clientes: " + clientes.Count());

            //Quantidade de Vendedores
            file.WriteLine("Quantidade de Vendedores: " + vendedores.Count());

            // ID da venda mais cara
            vendas.ForEach(venda =>
                venda.Itens.ForEach(item =>
                    maiorValor.Add(new MaiorValor { Id = item.ItemId, Valor = item.Quantidade * item.Preco })));

            file.WriteLine("Id da maior venda: " + maiorValor.OrderByDescending(x => x.Valor).First().Id);

            // Pior Vendedor
            file.WriteLine("Pior vendedor: " + vendas.OrderBy(x => x.TotalVenda).First().NomeVendedor);

            file.Close();
        }

        private List<Vendedor> CarregaVendedores(IEnumerable<string> vendedores)
        {
            List<Vendedor> lstVendedores = new List<Vendedor>();

            foreach (var vendedor in vendedores)
            {
                var dadosVendedor = vendedor.Split(new char[] { 'ç' });

                if (dadosVendedor.Count() > 4)
                    throw new Exception("Informações dos vendedores estão inválidas!");

                lstVendedores.Add(
                   new Vendedor()
                   {
                       Identificador = dadosVendedor[0],
                       Cpf = dadosVendedor[1],
                       Nome = dadosVendedor[2],
                       Salario = Convert.ToDecimal(dadosVendedor[3])
                   });
            }

            return lstVendedores;
        }

        /// <summary>
        /// Carrega os Clientes do arquivo
        /// </summary>
        /// <param name="clientes"></param>
        /// <returns></returns>
        private List<Cliente> CarregaClientes(IEnumerable<string> clientes)
        {
            List<Cliente> lstClientes = new List<Cliente>();

            foreach (var cliente in clientes)
            {
                var dadosCliente = cliente.Split(new char[] { 'ç' });

                if (dadosCliente.Count() > 4)
                    throw new Exception("Informações dos clientes estão inválidas!");

                lstClientes.Add(
                    new Cliente()
                    {
                        Identificador = dadosCliente[0],
                        Cnpj = dadosCliente[1],
                        Nome = dadosCliente[2],
                        AreaNegocio = dadosCliente[3]
                    });
            }

            return lstClientes;
        }

        /// <summary>
        /// Carrega as Vendas do arquivo
        /// </summary>
        /// <param name="vendas"></param>
        /// <returns></returns>
        private List<Venda> CarregaVendas(IEnumerable<string> vendas)
        {
            List<Venda> lstVendas = new List<Venda>();

            foreach (var venda in vendas)
            {
                List<Itens> lstItensVenda = new List<Itens>();
                var dadosVenda = venda.Split(new char[] { 'ç' });
                decimal valorTotal = 0;

                if (dadosVenda.Count() > 4)
                    throw new Exception("Informações das vendas estão inválidas!");

                var itensFile = dadosVenda[2].Replace("[", string.Empty).Replace("]", string.Empty).Split(new char[] { ',' });

                foreach (var item in itensFile)
                {
                    var valoresVendas = item.Split(new char[] { '-' });

                    int quantidade = Convert.ToInt32(valoresVendas[1]);
                    decimal preco = Convert.ToDecimal(valoresVendas[2].Replace(".", ","));

                    valorTotal += quantidade * preco;

                    lstItensVenda.Add(
                        new Itens()
                        {
                            ItemId = Convert.ToInt32(valoresVendas[0]),
                            Quantidade = quantidade,
                            Preco = preco
                        }
                    );
                }

                lstVendas.Add(
                   new Venda()
                   {
                       Identificador = dadosVenda[0],
                       NomeVendedor = dadosVenda[3],
                       VendaId = Convert.ToInt32(dadosVenda[1]),
                       Itens = lstItensVenda,
                       TotalVenda = valorTotal
                   });
            }

            return lstVendas;
        }
    }
}
