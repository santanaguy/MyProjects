To work in this I set up a container with python to avoid installing stuff.

If you run:

`docker-container up -d`

you will have a running container with python. 

To develop I use vscode with the remote wsl extension. I attach to the running container python-dev

Then I open the /app folder in the container
Then I install the Python extension in there. VSCode will ask you to install pylint. To start debugging just press F5 and 
choose to debug the file.

The /app folder is a volume so changes inside it will reflect outside, so it's fine. The setup is only done once,
after that you will have your container and can run `docker-container up -d` and you get the same container