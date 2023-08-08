// See https://aka.ms/new-console-template for more information
using dgsGofSingleton;

 
MyLog log = MyLog.GetInstance();
log.GravarLog("Implementando padrão Singleton do GOF - Gang of four");
log.GravarLog("O singleton garante que a classe tera apenas uma unica instancia");
log.GravarLog("O construtor precisa ser privado");
log.GravarLog("Este é o padrao EAGER, tambem pode usar o LAZY que cria a instancia na primeira vez que for usada");