// See https://aka.ms/new-console-template for more information
Console.WriteLine("Loto facil");

int ant =0;
		long c=0;
		long tt11=0;
		long tt12=0;
		long tt13=0;
		long tt14=0;
		long tt15=0;
		//numeros jogados
		int j1=1;
		int j2=24;
		int j3=3;
		int j4=4;
		int j5=5;
		int j6=6;
		int j7=7;
		int j8=8;
		int j9=9;
		int j10=10;
		int j11=11;
		int j12=12;
		int j13=13;
		int j14=14;
		int j15=25;
		int kn =0;
		//numeros sorteados
    	int s1=4;
		int s2=5;
		int s3=6;
		int s4=7;
		int s5=8;
		int s6=9;
		int s7=10;
		int s8=12;
		int s9=13;
		int s10=16;
		int s11=19;
		int s12=20;
		int s13=22;
		int s14=24;
		int s15=25;
		
		List<int> s = new List<int>();
		s.Add(s1);
		s.Add(s2);
		s.Add(s3);
		s.Add(s4);
		s.Add(s5);
		s.Add(s6);
		s.Add(s7);
		s.Add(s8);
		s.Add(s9);
		s.Add(s10);
		s.Add(s11);
		s.Add(s12);
		s.Add(s13);
		s.Add(s14);
		s.Add(s15);
		
    	List<int> j = new List<int>();
		j.Add(j1);
		j.Add(j2);
		j.Add(j3);
		j.Add(j4);
		j.Add(j5);
		j.Add(j6);
		j.Add(j7);
		j.Add(j8);
		j.Add(j9);
		j.Add(j10);
		j.Add(j11);
		j.Add(j12);
		j.Add(j13);
		j.Add(j14);
		j.Add(j15);
		
		Console.WriteLine("Jogo:");
		
    	Console.WriteLine($"{j1}-{j2}-{j3}-{j4}-{j5}-{j6}-{j7}-{j8}-{j9}-{j10}-{j11}-{j12}-{j13}-{j14}-{j15}");
    	Console.WriteLine();
		
    	Console.WriteLine("Sorteio:");
		
    	Console.WriteLine($"{s1}-{s2}-{s3}-{s4}-{s5}-{s6}-{s7}-{s8}-{s9}-{s10}-{s11}-{s12}-{s13}-{s14}-{s15}");
    	Console.WriteLine();
		
		
		  
		for(int n1 = 1;n1 <26;n1++)
		{		
			for(int n2 = n1 +1;n2 <26;n2++)
			{
				if (n2 != n1)
				{
				   for(int n3 = n2+1;n3 <26;n3++)
				   {
				   	if(n3 != n2 && n3 != n1)
					   {
					   	for(int n4 = n3+ 1;n4 <26;n4++)
						   {
						   	if(n4 != n3 && n4 != n2 && n4!= n1)
							   {
						   	   for(int n5 = n4+1;n5 <26;n5++)
							      {
								  	if(n5 != n4 && n5 != n3 && n5 != n2 && n5 != n1)
									  {
									  	for(int n6 = n5+1;n6 <26;n6++)
										  {
										  	for(int n7 = n6+ 1;n7 <26;n7++)
											  {
											     for(int n8 = n7+ 1;n8<26;n8++)
												 {
												    for(int n9 = n8+ 1;n9 <26;n9++)
													{
													   for(int n10 = n9+ 1;n10 <26;n10++)
													   {
													      for(int n11 = n10+1;n11 <26;n11++)
														  {
														     for(int n12 = n11+1;n12 <26;n12++)
															 {
														   	 for(int n13 = n12+1;n13 <26;n13++)
																{
																  for(int n14 = n13+1;n14 <26;n14++)
																  {
																      for(int n15 = n14+1;n15 <26;n15++)
																	  {
																	  	
																	  	bool t1= (j1 == n1 || j1 == n2 || j1==n3 || j1==n4 || j1==n5 || j1==n6 || j1==n7 || j1==n8 || j1==n9 || j1==n10 || j1==n11 || j1==n12 || j1==n13 || j1==n14 || j1==n15);
																	 	 bool t2= (j2 == n1 || j2 == n2 || j2==n3 || j2==n4 || j2==n5 || j2==n6 || j2==n7 || j2==n8 || j2==n9 || j2==n10 || j2==n11 || j2==n12 || j2==n13 || j2==n14 || j2==n15);
																		  bool t3= (j3 == n1 || j3 == n2 || j3==n3 || j3==n4 || j3==n5 || j3==n6 || j3==n7 || j3==n8 || j3==n9 || j3==n10 || j3==n11 || j3==n12 || j3==n13 || j3==n14 || j3==n15);
																	  	bool t4= (j4 == n1 || j4 == n2 || j4==n3 || j4==n4 || j4==n5 || j4==n6 || j4==n7 || j4==n8 || j4==n9 || j4==n10 || j4==n11 || j4==n12 || j4==n13 || j4==n14 || j4==n15);
																	      bool t5= (j5 == n1 || j5 == n2 || j5==n3 || j5==n4 || j5==n5 || j5==n6 || j5==n7 || j5==n8 || j5==n9 || j5==n10 || j5==n11 || j5==n12 || j5==n13 || j5==n14 || j5==n15);
															          	bool t6= (j6 == n1 || j6 == n2 || j6==n3 || j6==n4 || j6==n5 || j6==n6 || j6==n7 || j6==n8 || j6==n9 || j6==n10 || j6==n11 || j6==n12 || j6==n13 || j6==n14 || j6==n15);
														              	bool t7= (j7 == n1 || j7 == n2 || j7==n3 || j7==n4 || j7==n5 || j7==n6 || j7==n7 || j7==n8 || j7==n9 || j7==n10 || j7==n11 || j7==n12 || j7==n13 || j7==n14 || j7==n15);
													                  	bool t8= (j8 == n1 || j8 == n2 || j8==n3 || j8==n4 || j8==n5 || j8==n6 || j8==n7 || j8==n8 || j8==n9 || j8==n10 || j8==n11 || j8==n12 || j8==n13 || j8==n14 || j8==n15);
												                      	bool t9= (j9 == n1 || j9 == n2 || j9==n3 || j9==n4 || j9==n5 || j9==n6 || j9==n7 || j9==n8 || j9==n9 || j9==n10 || j9==n11 || j9==n12 || j9==n13 || j9==n14 || j9==n15);
											                          	bool t10= (j10 == n1 || j10 == n2 || j10==n3 || j10==n4 || j10==n5 || j10==n6 || j10==n7 || j10==n8 || j10==n9 || j10==n10 || j10==n11 || j10==n12 || j10==n13 || j10==n14 || j10==n15);
										                              	bool t11= (j11 == n1 || j11 == n2 || j11==n3 || j11==n4 || j11==n5 || j11==n6 || j11==n7 || j11==n8 || j11==n9 || j11==n10 || j11==n11 || j11==n12 || j11==n13 || j11==n14 || j11==n15);
																	 	 bool t12= (j12 == n1 || j12 == n2 || j12==n3 || j12==n4 || j12==n5 || j12==n6 || j12==n7 || j12==n8 || j12==n9 || j12==n10 || j12==n11 || j12==n12 || j12==n13 || j12==n14 || j12==n15);
									                                  	bool t13= (j13 == n1 || j13 == n2 || j13==n3 || j13==n4 || j13==n5 || j13==n6 || j13==n7 || j13==n8 || j13==n9 || j13==n10 || j13==n11 || j13==n12 || j13==n13 || j13==n14 || j13==n15);							
																		  bool t14= (j14 == n1 || j14 == n2 || j14==n3 || j14==n4 || j14==n5 || j14==n6 || j14==n7 || j14==n8 || j14==n9 || j14==n10 || j14==n11 || j14==n12 || j14==n13 || j14==n14 || j14==n15);											
									                                  	bool t15= (j15 == n1 || j15 == n2 || j15==n3 || j15==n4 || j15==n5 || j15==n6 || j15==n7 || j15==n8 || j15==n9 || j15==n10 || j15==n11 || j15==n12 || j15==n13 || j15==n14 || j15==n15);
											
																	
											
																		  {
																		  	c++;
																			// if(j.Contains(n1) && j.Contains(n2) && j.Contains(n3) && j.Contains(n4) && j.Contains(n5) && j.Contains(n6) && j.Contains(n7) && j.Contains(n8) && j.Contains(n9) && j.Contains(n10) && j.Contains(n11) && j.Contains(n12) && j.Contains(n13) && j.Contains(n14) )
																			// if(t1 && t2 && t3 && t4 && t5 && t6 && t7 && t8 && t9 && t10 && t11 && t12 && t13 && t14  )
																			 // {
																			  	//kn++;
																				  int h=0;
																				  //baseado no sorteio
																				  int hs = 0;
																				  if (s.Contains(n1)) hs++;
																			 	 if (s.Contains(n2)) hs++;
																			  	if (s.Contains(n3)) hs++;
																		      	if (s.Contains(n4)) hs++;
																			 	 if (s.Contains(n5)) hs++;
																			  	if (s.Contains(n6)) hs++;
																		      	if (s.Contains(n7)) hs++;
																	          	if (s.Contains(n8)) hs++;
																              	if (s.Contains(n9)) hs++;
															                  	if (s.Contains(n10)) hs++;
														                      	if (s.Contains(n11)) hs++;
													                          	if (s.Contains(n12)) hs++;
													                          	if (s.Contains(n13)) hs++;
														                      	if (s.Contains(n14)) hs++;
														                      	if (s.Contains(n15)) hs++;
																				  
																				  
																				  //baseado no jogo
																				  
																				 if (j.Contains(n1)) h++;
																			 	 if (j.Contains(n2)) h++;
																			  	if (j.Contains(n3)) h++;
																		      	if (j.Contains(n4)) h++;
																			 	 if (j.Contains(n5)) h++;
																			  	if (j.Contains(n6)) h++;
																		      	if (j.Contains(n7)) h++;
																	          	if (j.Contains(n8)) h++;
																              	if (j.Contains(n9)) h++;
															                  	if (j.Contains(n10)) h++;
														                      	if (j.Contains(n11)) h++;
													                          	if (j.Contains(n12)) h++;
													                          	if (j.Contains(n13)) h++;
														                      	if (j.Contains(n14)) h++;
														                      	if (j.Contains(n15)) h++;																				  																				  
																		          
																				  if (h == 11) tt11++;
																				  if (h == 12) tt12++;
																				  if (h == 13) tt13++;
																				  if (h == 14) tt14++;
																				  if (h == 15) tt15++;
																				  
																				  if ((h == 11) && (ant != n11))
																				  {
																				  	kn++;																		  																			 
																				     // Console.WriteLine($"{n1}-{n2}-{n3}-{n4}-{n5}-{n6}-{n7}-{n8}-{n9}-{n10}-{n11}-{n12}-{n13}-{n14}-{n15}---{h}");
																			 	 }
																				  ant = n11;
																			 	 if (h>13)
																				 {
																				  //	Console.WriteLine("PREMiADO ***********************");
																				 	
																				 	
																				  }
																			  //}
																		  }
																		  
																	  }
																  }
																}
															 }
														  }
														  
													   }
													}
												 }
											  }
										  }
									  }
							     	
							      }
							   }
						   	
						   }
					   	
					   	
					   	
					   }
					   	
				   }
				}
				
					
					
				
			}
			
		//	Console.WriteLine(n1);
			
		}
		Console.WriteLine();
		Console.WriteLine($"gerei {c} jogos, {kn} do jogo 11:{tt11}, 12:{tt12}, 13:{tt13}, 14:{tt14}, 15:{tt15}");
		/*
	string filter="xxxDescription eq '341 - Ita";
			Regex r = new Regex("Description eq '[0-9]");
			Match m = r.Match(filter);
			Console.WriteLine(m);
			
			int i =3;
			Console.WriteLine($"i={i:000}");
			DateTime d = DateTime.Now;
			Console.WriteLine($"Hoje é {d:dd-MM-yyyy}");
			Console.WriteLine($"Agora sao {d:hh:mm}");
			
        	string text = "am0rtecedor";
        	Regex regex = new Regex("am[0-9]+r");
        	Match match = regex.Match(text);
        	
            Console.WriteLine(match);
            string cpf="253.574.578-26";
            Regex rcpf= new Regex(@"^([0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2})|([0-9]{11})$");
            var r = rcpf.Match(cpf);
            if(r.Success)
               Console.WriteLine("cpf valid");
            else
               Console.WriteLine("cpf invalid");
               
            var d = new DateTime(1997,10,07).AddDays(9999);
            Console.WriteLine(d);
			*/


