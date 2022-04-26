
# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.

Este é um exemplo real de sistema que utilizamos na Umbler.

Ex: Consultar os dados de registro do dominio `umbler.com`

**Retorno:**
- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Empresa que está hospedado (Umbler)

Essas informações são descobertas através de consultas nos servidores DNS e de WHOIS.

*Obs: WHOIS (pronuncia-se "ruís") é um protocolo específico para consultar informações de contato e DNS de domínios na internet.*

Nesta aplicação, os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessaria, caso seu TTL ainda não tenha expirado.

*Obs: O TTL é um valor em um registro DNS que determina o número de segundos antes que alterações subsequentes no registro sejam efetuadas. Ou seja, usamos este valor para determinar quando uma informação está velha e deve ser renovada.*

Tecnologias Backend utilizadas:

- C#
- Asp.Net Core
- MySQL
- Entity Framework

Tecnologias Frontend utilizadas:

- Webpack
- Babel
- ES7

Para rodar o projeto você vai precisar instalar:

- dotnet Core SDK (https://www.microsoft.com/net/download/windows dotnet Core 6.0.201 SDK)
- Um editor de código, acoselhamos o Visual Studio ou VisualStudio Code. (https://code.visualstudio.com/)
- NodeJs v17.6.0 para "buildar" o FrontEnd (https://nodejs.org/en/)
- Um banco de dados MySQL (vc pode rodar localmente ou criar um site PHP gratuitamente no app da Umbler https://app.umbler.com/ que lhe oferece o banco Mysql adicionamente)

Com as ferramentas devidamente instaladas, basta executar os seguintes comandos:

Para "buildar" o javascript basta executar:

`npm install`
`npm run build`

Para Rodar o projeto:

Execute a migration no banco mysql:

`dotnet tool update --global dotnet-ef`
`dotnet tool ef database update`

E após: 

`dotnet run` (ou clique em "play" no editor do vscode)

# Objetivos:

Se você rodar o projeto e testar um domínio, verá que ele já está funcionando. Porém, queremos melhorar varios pontos deste projeto:

# FrontEnd

 - Os dados retornados não estão formatados, e devem ser apresentados de uma forma legível.
 - Não há validação no frontend permitindo que seja submetido uma requsição inválida para o servidor (por exemplo, um domínio sem extensão).
 - Está sendo utilizado "vanilla-js" para fazer a requisição para o backend, apesar de já estar configurado o webpack. O ideal seria utilizar algum framework mais moderno como ReactJs ou Blazor.  

# BackEnd

 - Não há validação no backend permitindo que uma requisição inválida prossiga, o que ocasiona exceptions (erro 500).
 - A complexidade ciclomática do controller está muito alta, o ideal seria utilizar uma arquitetura em camadas.
 - O DomainController está retornando a própria entidade de domínio por JSON, o que faz com que propriedades como Id, Ttl e UpdatedAt sejam mandadas para o cliente web desnecessariamente. Retornar uma ViewModel (DTO) neste caso seria mais aconselhado.

# Testes

 - A cobertura de testes unitários está muito baixa, e o DomainController está impossível de ser testado pois não há como "mockar" a infraestrutura.
 - O Banco de dados já está sendo "mockado" graças ao InMemoryDataBase do EntityFramework, mas as consultas ao Whois e Dns não. 

# Dica

- Este teste não tem "pegadinha", é algo pensado para ser simples. Aconselhamos a ler o código, e inclusive algumas dicas textuais deixadas nos testes unitários. 
- Há um teste unitário que está comentado, que obrigatoriamente tem que passar.
- Diferencial: criar mais testes.

# Entrega

- Enviei o link do seu repositório com o código atualizado.
- O repositório deve estar público para que possamos acessar.
- Modifique Este readme adicionando informações sobre os motivos das mudanças realizadas.

# Modificações:

Front-End:
- Antes era mostrado um JSON com os dados da request. Como solicitado, para os dados de IP, DomainName, HostedAt e WhoIs foram criadas divs para cada um. Para este último, por ser maior (e possuir caracteres de escape) foi também formatado de forma a ficar mais legível.
- Quando o back retorna uma mensagem de erro, é mostrado uma caixa com a mensagem.
- Além da classe de Api já existente, criei a Domain e DOM. A classe de Domain seria equivalente com sua entidade presente no back-end, porém com algumas validações e campos próprios. Já a classe de Dom é responsável por gerenciar os resultados presentes na tela.
- Foi realizada validação do domínio digitado por meio de Regex.
- Eu não possuo alguma experiência com React ou Blazor (e web aplicações MVC, para falar a verdade) e como implementá-los nesse tipo de aplicação. Portanto, preferi me manter simples nesse quesito.

Back-End:
- Foram adicionados os try-catches necessários. Agora é possível retornar BadRequest() ao ocorrer algum erro.
- Da mesma forma, como solicitado, ao invés de retornar a própria entidade, criei o DomainDTO apenas com os dados necessários para a tela.
- A minha ideia era fazer a divisão do código em 3 áreas (Api, Service e Data) utilizando Class Libraries e afins. Contudo, meu zero conhecimento com Migrations me impediu de colocá-lo na pasta de Data. Por fim, acabei criando pastas simples no Desafio.Umbler mesmo. A pasta de Api, que conteria os controller, se manteve como Controllers mesmo. A pasta de Service, com as entidades e DTO, possuem mesmo nome. E Data, com os repositórios e o contexto do banco.
- Tentei codificar da forma mais organizada, retirando possíveis duplicidades e separando o código do controller nas pastas supracitadas. Dei uma atenção a mais para a entidade de Domain, tornando os métodos privados e fazendo algumas validações.

Testes:
- Talvez essa tenha sido a parte mais degenerada de todo o projeto. Como é possível perceber, eu também não possuo conhecimentos em teste unitário.
- Vi que era preciso fazer o último teste funcionar, mas já aviso que isso não acontece hahaha. Tentei criar um wrapper para o WhoIsClient, inclusive eles estão na parte de Entities junto do Domain. Algo que me deixou extremamente curiosa é como o Domain_Moking_LookupClient  passa pelo teste e o Domain_Moking_WhoisClient não, sendo que são extremamente parecidos (aliás, fica o estudo pra mim em como testar cada injeção de dependência individualmente).
