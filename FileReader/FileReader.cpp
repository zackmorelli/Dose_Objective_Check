#include<vector>
#include<string>
#include<cmath>
#include<sstream>
#include<iostream>
#include<fstream>
#include<cstdio>
#include<cstdlib>


class ROI {

public:

	ROI();
	ROI(std::string name, std::string region, std::string limit,
		std::string limitstrict, int limitvalue, std::string limitunit,
		std::vector<std::string> treatsite);
	~ROI();



};


int main()
{
	std::string line;
	std::ifstream file;
	std::string token;
	std::stringstream ss(line);
	int index = 0;    //keeps track of columns in ROI list


	std::vector<ROI> ROIList;


	file.open("P:\\User Files\Zack\Programming\testf.txt");

	if (file.is_open())
	{

		while (getline(file, line))    //each loop pulls a line from the file
		{

			while (ss >> token)      //each loop pulls a word from the line
			{







				index++;

			}






		}
		





		std::cout << "Trigger" << std::endl;

		file.close();
	}
	else std::cout << "Unable to open file" << std::endl;

	return 0;  //returns main


}


