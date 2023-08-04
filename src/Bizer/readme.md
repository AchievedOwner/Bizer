## Bizer
��Ϊ���塢������չ�ĺ�����

### ·��
�ӿڶ��� `ApiRouteAttribute`��ʵ�� HTTP ��·��ǰ׺��ͬ `Controller` ��� `HttpRouteAttribute`
```cs
[ApiRoute("api/users")]	//���� http://localhost/api/users
public interface IUserManager
{
}
```
**��֧�� mvc �е� `[controller]` �ؼ���**
> û�ж�������ԵĽӿڲ����Զ�ʶ��� API �ͷ��� HTTP ����


### Http ������HttpMethod)
�ӿڷ����϶��壬ͬ MVC ��ʽʹ�ã������Ƕ��ձ��

| Mvc | Bizer |
|---|---|
|HttpGet|Get|
|HttpPost|Post|
|HttpPut|Put|
|HttpDelete|Delete|
|HttpPatch|Patch|
|HttpOptions|Options|
|HttpTrace|Trace|

ʾ����
```cs
[ApiRoute("api/users")]
public interface IUserService
{
	[Post]
	Task CreateAsync()
}
```
### ����
����Ĭ���� `query string`���� `?arg1=value1&arg2=value2...`
������ mvc �� `FromQueryAttribute`��ӳ���ϵ���£�
|Mvc|Bizer|��ע|
|---|---|---|
|FromRoute|Path|·���п�ģ���������{id}|
|FromQuery|Query|
|FromHeader|Header|�Զ����뵽 Header ��
|FromForm|Form|���Զ�ʹ�� form/data ��ʽ|
|FromBody|Body|�� body �ύ��Ĭ��ʹ�� application/json �ķ�ʽ|
ʾ����
```cs
[ApiRoute("api/users")]
public interface IUserService
{
	[Post]
	Task CreateAsync([Body]User user)

	[Get("{id}")]
	Task<User> GetAsync([Path]int id)
}
```
### ����
�� `Program.cs` ��ע���������ã�
```cs
services.AddBizer(options=>{
	//�����Զ����򼯷��֣�Ŀ����Ϊ֮���ģ��ʹ��

	options.Assemblies.Add(typeof(xxx).Assembly); //����Զ����ֵĳ���

	options.AssemblyNames.Add("MyAssembly.*.Service");//ģ������ƥ�����Ƶĳ��򼯣�֧��ͨ���
});
```
### `Returns` �� `Resunts<TResult>` ����ֵ����
�����ͽ����� `Code` `Messages` `Succeed` `Data` �ĸ��������ԡ�
```cs
public Task<Returns> GetAsync() //�޷�������
{
	if(xxxx)
	{
		return Returns.Failed("������Ϣ");
	}
	return Returns.Success();
}

public Task<Returns<Data>> GetAsync() //�з�������
{
	if(xxxx)
	{
		return Returns<Data>.Failed("������Ϣ");
	}
	return Returns<Data>.Success(data);//һ���ǳɹ�����������ݷ���
}


public Task<Returns> GetAsync() //���Է��ض���Ϣ
{
	var returns = new Returns();
	if(xxxx)
	{
		returns.AppendMessages("....");
	}
	else if(xxx)
	{
		returns.AppendMessages("....");
	}
	else
	{
		returns.IsSuccess();
	}
	return returns;
}
```
