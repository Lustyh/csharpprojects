using System;
class Enemy
{
    public void Move()
    {
        Console.WriteLine("调用了 enemy的move方法");
    }
    public virtual void Attack()
    {
        Console.WriteLine("enemy attac");
    }
}


class Boss : Enemy
{
    public override void Attack()
    {
        Console.WriteLine("Boss Attac");
    }

    public new void Move()
    {
        Console.WriteLine("Boss move");
    }
}

class Program
{
    static void Main(string[] args)
    {

        Boss oneEnemy = new Boss();
        oneEnemy.Move();

        Enemy twoEnemy = new Boss();
        twoEnemy.Move();


        Enemy threeEnemy = new Enemy();
        threeEnemy.Attack();

        Enemy fourEnemy = new Boss();
        fourEnemy.Attack();

        Console.ReadKey();
    }
}
