The database stores user's passwords in plaintext, if the database is compromised, the passwords are given to the hacker, which not only compromises this site, but compromises any other sites where the same username and password are used.

Passwords should be securely hashed (not just encrypted) which is an irreversible function. Once the password is hashed during creation of the user, the password can be hashed to check against the database, which allows you to still compare passwords without actually knowing the password.

More information can be found [here](https://crackstation.net/hashing-security.htm) including a secure implementation of industry standard hashing methods.

PHP also has a built-in methods as of 5.5, using [password_hash](http://www.php.net/manual/en/function.password-hash.phh) and [password_verify](http://www.php.net/manual/en/function.password-verify.php)

An example for how to use these functions. The first creates the password hash to store on the database:

    $username = $_POST['username']; //the username
    $pass = password_hash($_POST['password'],PASSWORD_DEFAULT); //the now secure username
    //store in database the username and password

    
The next uses the password_verify to log in.

    $username = $_POST['username']; //the username
    $pass = $_POST['password']; //the plaintext password
    $dbPassword = //get the password from the database for the user
    if (password_verify($pass,$dbPassword))
    {
        //the password was correct log in
    }
    else
    {
        //the password was incorrect display error saying invalid username or password
    }